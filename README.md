🚗 CMSSystems.StockManagementDemo v3.1

    A demo stock management system for a car dealership, built with Ionic Angular and .NET 8. Manage vehicles, images, and accessories in a clean, functional, and user-friendly interface.

📖 Table of Contents

    🚀 Project Overview
  
    ✨ Features
  
    🗂️ Data Entities
  
    🛠️ Technology Stack
  
    ⚡ Setup and Installation
  
    📝 Usage
  
    💡 Notes

🚀 Project Overview

    The system manages a dealership’s stock-on-hand,
  
    It allows dealership staff to add, update, and delete stock items, including associated images and accessories. Clicking on a stock item opens a detailed view for editing.

  
✨ Features

    🔐 User authentication and login
    
    🔍 Searchable, paginated, and sortable stock list
    
    🖼️ Vehicle thumbnails in the stock list
    
    ➕ Add new vehicles (up to 3 images per vehicle)
    
    ✏️ Update vehicle details and images
    
    🗑️ Delete vehicles and associated images
    
    📄 Detailed view/edit mode for individual stock items

🛠️ Technology Stack

  Frontend:

    Angular 17
    
    Ionic 7
    
    TypeScript
    
  Backend:
  
    .NET 8 Web API
    
    Entity Framework Core
    
    ASP.NET Identity for user management
    
    SQL Server for database
    
    JWT for authentication
      
    Database: MSSQL
  Other Tools:

    Swagger for API testing
    
    CORS configured for local Angular dev

⚡ Setup and Installation
 *Note, DB is already hossted using AWS free tier
 
 Backend:
  1. Clone the repository:
  
    git clone https://github.com/ZinkyTinky/DealershipApp.git
    cd DealershipApp/DealershipBackEnd

  2. Run the backend

    dotnet run


  The backend will be available at https://localhost:5001 (or the port shown in the console)
  
  Frontend Setup:
  
  1. Navigate to frontend folder

    cd ../DealershipFrontEnd


  2. Install dependencies

    npm install


  3. Configure environment.ts
  Ensure API URL points to your backend:

    export const environment = {
      production: false,
      apiUrl: 'https://localhost:5001/api' //<--- Or change 5001 to the port given in the backend
    };


  4. Run the frontend

    ionic serve


  The frontend will be available at http://localhost:8100 or the port given in the console
  
  

📝 Usage

    🔐 Login with your credentials, or register on the DB (Currently no safegaurding against who can register, view or add stock, user must just be logged in)
    
    🔍 Browse the stock list with search, sort, and pagination (Only after 10 stock-items have been added)
    
    ➕ Add new vehicles with details and images
    
    ✏️ Edit or delete existing vehicles
    
    📄 Click a vehicle to view/edit full details
  
💡 Notes
  
    ⚠️ Ensure the backend API is running before accessing the frontend
    
    🖼️ Images are stored in the database as binary; maximum 3 images per vehicle
