using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DealershipBackEnd.Data;
using DealershipBackEnd.Models;
using DealershipBackEnd.Interfaces;
using DealershipBackEnd.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Create the WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// ==============================
// 1. Configure SQL Server DbContext
// ==============================
// This registers the ApplicationDbContext with the DI container
// and sets it up to use SQL Server with the connection string from appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==============================
// 2. Add ASP.NET Core Identity
// ==============================
// Adds support for user management, authentication, roles, and security tokens.
// We're using our custom 'User' class and IdentityRole for roles.
// Entity Framework is used to store identity data in the database.
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ==============================
// 3. Dependency Injection (Repositories)
// ==============================
// Registers interfaces with their concrete implementations
// so we can inject them into controllers easily.
builder.Services.AddScoped<IUserAuthInterface, UserAuthRepository>();
builder.Services.AddScoped<IStockInterface, StockRepository>();

// ==============================
// 4. Add Controllers
// ==============================
// Enables the use of API controllers in this project.
builder.Services.AddControllers();

// ==============================
// 5. JWT Authentication
// ==============================
// Read JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

// Configure authentication middleware to use JWT bearer tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure how JWT tokens are validated
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // validate the token issuer
        ValidateAudience = true, // validate the token audience
        ValidateLifetime = true, // validate token expiration
        ValidateIssuerSigningKey = true, // validate the signing key
        ValidIssuer = jwtSettings["Issuer"], // expected issuer
        ValidAudience = jwtSettings["Audience"], // expected audience
        IssuerSigningKey = new SymmetricSecurityKey(key) // key used to sign the token
    };
});

// ==============================
// 6. Swagger / OpenAPI with JWT support
// ==============================
// Swagger allows us to explore and test the API endpoints via a web UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add a JWT Bearer security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // Header name
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    // Require JWT token in Swagger UI for authorized endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                } 
            },
            new string[] {} // No specific scopes required
        }
    });
});

// ==============================
// 7. CORS (Cross-Origin Resource Sharing)
// ==============================
// Allows the Angular frontend (running on localhost:8100) to make requests to this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:8100") // frontend URL
                  .AllowAnyHeader() // allow any HTTP headers
                  .AllowAnyMethod(); // allow GET, POST, PUT, DELETE, etc.
        });
});

// ==============================
// 8. Build the WebApplication
// ==============================
var app = builder.Build();

// ==============================
// 9. Configure Middleware Pipeline
// ==============================

// Enable Swagger only in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable CORS policy for Angular dev server
app.UseCors("AllowAngularDev");

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Start the application
app.Run();
