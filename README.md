# Ecommerce-App
 # E-Commerce Vertical Slice - Full Stack Application

A complete e-commerce platform with user authentication, product catalog, shopping cart, and checkout functionality built with Angular 17 frontend and .NET 6+ C# backend.

##Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** (v18+) - [Download](https://nodejs.org/)
- **npm** (comes with Node.js)
- **.NET 6 SDK or later** - [Download](https://dotnet.microsoft.com/download)
- **SQL Server** (Express or full version) - [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Visual Studio 2026** (optional but recommended) - [Download](https://visualstudio.microsoft.com/)
- **Git** - [Download](https://git-scm.com/)

##Quick Start

### 1.Clone the Repository

```bash
git clone
cd ecommerce-vertical-slice
```

### 2. Database Setup

#### Option A: Using Database Backup File

1. Open **SQL Server Management Studio**
2. Right-click on **Databases** → **Restore Database**
3. Select **Device** and browse to `database.bak`
4. Click **OK** to restore

#### Option B: Create Database Manually

1. Open **SQL Server Management Studio**
2. Connect to your SQL Server instance
3. Run the following SQL script:

```sql
CREATE DATABASE EcommerceDB;

USE EcommerceDB;

-- Create Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Create Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Create Orders Table
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    ShippingAddress NVARCHAR(500),
    ShippingCity NVARCHAR(100),
    ShippingState NVARCHAR(50),
    ShippingZip NVARCHAR(20),
    State NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Create OrderItems Table
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Insert Sample Products
INSERT INTO Products (Name, Price, Stock, Description) VALUES
('Mechanical Keyboard', 149.99, 10, 'RGB Mechanical Keyboard'),
('Wireless Mouse', 29.99, 20, 'USB Wireless Mouse'),
('USB Hub', 39.99, 15, '4-Port USB Hub'),
('Monitor 27"', 299.99, 8, '27 inch 4K Monitor'),
('Laptop Stand', 49.99, 12, 'Adjustable Laptop Stand');
```

### 3. Backend Setup (.NET)

```bash
# Navigate to backend folder
cd Backend

# Restore NuGet packages
dotnet restore

# Update connection string (if needed)
# Edit appsettings.json and update:
# "DefaultConnection": "Server=YOUR_SERVER;Database=EcommerceDB;Trusted_Connection=true;"

# Run migrations (if not using backup)
dotnet ef database update

# Build the project
dotnet build

# Run the backend
dotnet run
```

Backend will start at: **http://localhost:5054**

### 4. Frontend Setup (Angular)

```bash
# Navigate to frontend folder
cd Frontend

# Install dependencies
npm install

# Start the development server
ng serve
```

Frontend will start at: **http://localhost:4200**

Open your browser and navigate to `http://localhost:4200`

## Running Tests

### Backend Tests (C#)

```bash
cd Backend
dotnet test
```
Expected Output:

32 tests passed, 0 failed

### Frontend Tests (Angular)

```bash
cd Frontend

# Run tests in watch mode
npm test

# Run tests once (headless)
npm test -- --watch=false --browsers=ChromeHeadless

# With code coverage
npm test -- --code-coverage
```

Expected output:
63 specs, 0 failures
