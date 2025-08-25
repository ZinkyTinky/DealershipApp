CMSSystems.StockManagementDemo v3.1

A demo stock management system for a car dealership, built with Ionic Angular and .NET 9. This project allows users to manage vehicles, images, and accessories in a clean, functional, and user-friendly interface.

Table of Contents

Project Overview

Features

Data Entities

Technology Stack

Setup and Installation

Usage

Notes

Project Overview

The system is designed to manage a dealership’s stock-on-hand, serving as the primary source for vehicles advertised on platforms like AutoTrader, Cars.co.za, and Carfind.co.za.

It allows dealership staff to add, update, and delete stock items, including associated images and accessories. Clicking on a stock item opens a detailed view for editing.

This demo focuses on styling, presentation, error handling, and functional completeness.

Out of scope:

REST API serving stock with images to third parties

Sales activities like quoting or OTP generation

Features

User authentication and login

Searchable, paginated, and sortable stock list

Vehicle thumbnails in the stock list

Add new vehicles (up to 3 images per vehicle)

Update vehicle details and images

Delete vehicles and associated images

Detailed view/edit mode for individual stock items

Data Entities
Stock Item

Reg No.: Vehicle registration number

Make: Vehicle manufacturer

Model: Vehicle model description

Model Year: Vehicle model year

KMS: Current kilometre reading

Colour: Vehicle color

VIN: Vehicle Identification Number

Retail Price: Selling price

Cost Price: Cost price

Accessories: Collection of accessories

Images: Collection of images

DTCreated: Date record created

DTUpdated: Date record updated

Stock Accessory

Name: Accessory name

Description: Optional description

Image

Name: E.g., “front”, “side”, etc.

Binary: Image stored as binary data

Technology Stack

Frontend: Ionic Angular, CSS3, Bootstrap / Material Design / PrimeNG

Backend / API: C# .NET 9

Database: MSSQL

Hosting: Firebase or AWS free tier (optional)

Setup and Installation

Clone the repository:

git clone <your-repo-url>
cd CMSSystems.StockManagementDemo


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

Frontend: http://localhost:8100 (default Ionic port)

API: http://localhost:<your-api-port>

Usage

Login with your credentials

Browse the stock list with search, sort, and pagination

Add new vehicles with details and images

Edit or delete existing vehicles

Click a vehicle to view/edit full details

Notes

Ensure the backend API is running before accessing the frontend

Images are stored in the database as binary; maximum 3 images per vehicle

User experience and styling are emphasized—error handling is implemented

Optional: Hosted demo will demonstrate functionality
