🚗 CMSSystems.StockManagementDemo v3.1

A demo stock management system for a car dealership, built with Ionic Angular and .NET 9. Manage vehicles, images, and accessories in a clean, functional, and user-friendly interface.

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

  Frontend: Ionic Angular, CSS3, Bootstrap
  
  Backend / API: C# .NET 9
  
  Database: MSSQL

⚡ Setup and Installation

  Clone the repository:
  
  git clone (https://github.com/ZinkyTinky/DealershipApp/)
  
  
  Backend Setup:

    Open the .NET 9 API project
    
    Update the connection string in appsettings.json for your MSSQL database
    
    Run the database creation and seed scripts included in /Database
  
  Frontend Setup:
  
    Navigate to the Ionic Angular project
    
    npm install
    ionic serve
  
  
  Running the API:
  
    dotnet run
  
  
  Access the application:
  
    🌐Frontend: http://localhost:8100 
    
    ⚙️API: http://localhost:<your-api-port> 

📝 Usage

  🔐 Login with your credentials, or register on the DB (Currently no safegaurding against who can register)
  
  🔍 Browse the stock list with search, sort, and pagination
  
  ➕ Add new vehicles with details and images
  
  ✏️ Edit or 🗑️ delete existing vehicles
  
  📄 Click a vehicle to view/edit full details
  
💡 Notes
  
  ⚠️ Ensure the backend API is running before accessing the frontend
  
  🖼️ Images are stored in the database as binary; maximum 3 images per vehicle
  
  🎨 User experience and styling are emphasized—error handling is implemented
  
  🌟 Optional: Hosted demo will demonstrate full functionality
