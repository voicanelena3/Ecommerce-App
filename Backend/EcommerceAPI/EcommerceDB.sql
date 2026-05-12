-- EcommerceDB Database Creation Script

-- Create the Database
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'EcommerceDB')
BEGIN
    DROP DATABASE EcommerceDB;
END
GO

CREATE DATABASE EcommerceDB;
GO

USE EcommerceDB;
GO

-- Create Tables

-- Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL
);

-- Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Stock INT NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    ImageUrl NVARCHAR(500),
    CreatedAt DATETIME2(7) NOT NULL
);

-- Orders Table
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TotalPrice DECIMAL(10, 2) NOT NULL,
    ShippingAddress NVARCHAR(255) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    ZipCode NVARCHAR(20) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    State NVARCHAR(100),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- OrderItems Table
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    Subtotal DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Insert Sample Data

-- Insert Users
INSERT INTO Users (Email, FirstName, LastName, PasswordHash, CreatedAt)
VALUES
    ('test@example.com', 'Test', 'User', '$2a$11$9qo8uLQickgz2MRZoMyejZAcg7b3XeKsUxWde...', '2026-04-29 17:47:07.8500000'),
    ('test1234@gmail.com', 'Test', 'User', '$2a$11$vIeAchdqRchlf54WWF7QC7qRQ7cRN0C5MkLqSEv3O...', '2026-05-06 20:52:15.7033333');

-- Insert Products
INSERT INTO Products (Name, Description, Price, Stock, Category, ImageUrl, CreatedAt)
VALUES
    ('Laptop Pro', 'High-performance laptop with 16GB RAM and SSD', 1299.99, 49, 'Electronics', 'https://via.placeholder.com/300?text=Laptop+Pro', '2026-04-29 14:28:41.1500000'),
    ('Wireless Mouse', 'Ergonomic wireless mouse with 2.4GHz connection', 29.99, 200, 'Electronics', 'https://via.placeholder.com/300?text=Wireless+Mo...', '2026-04-29 14:28:41.1500000'),
    ('USB-C Cable', 'High-speed USB 3.2 charging and data cable', 19.99, 500, 'Accessories', 'https://via.placeholder.com/300?text=USB-C+Cable', '2026-04-29 14:28:41.1500000'),
    ('Monitor 4K', '27-inch 4K UHD Monitor with HDR support', 449.99, 30, 'Electronics', 'https://via.placeholder.com/300?text=Monitor-4K', '2026-04-29 14:28:41.1500000'),
    ('Mechanical Keyboard', 'RGB Mechanical Keyboard with Cherry MX Switches', 149.99, 74, 'Electronics', 'https://via.placeholder.com/300?text=Keyboard', '2026-04-29 14:28:41.1500000'),
    ('Laptop Stand', 'Adjustable aluminum laptop stand for ergonomic s...', 39.99, 120, 'Accessories', 'https://via.placeholder.com/300?text=Laptop+Stand', '2026-04-29 14:28:41.1500000'),
    ('External SSD', '1TB External SSD with USB 3.1', 119.99, 80, 'Storage', 'https://via.placeholder.com/300?text=External+SSD', '2026-04-29 14:28:41.1500000'),
    ('Webcam 1080p', 'Full HD Webcam with autofocus for video calls', 59.99, 90, 'Electronics', 'https://via.placeholder.com/300?text=Webcam', '2026-04-29 14:28:41.1500000');

-- Insert Orders
INSERT INTO Orders (UserId, TotalPrice, ShippingAddress, City, ZipCode, Status, CreatedAt, State)
VALUES
    (1002, 149.99, 'Strada colonei scarlet demetriade Nr7', 'Craiova', '200167', 'Pending', '2026-05-06 21:05:23.9600000', 'Dolj'),
    (1002, 1299.99, 'Strada colonei scarlet demetriade Nr7', 'Craiova', '200167', 'Pending', '2026-05-06 21:05:39.5100000', 'Dolj');

-- Insert OrderItems
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, Subtotal)
VALUES
    (1, 5, 1, 149.99, 149.99),
    (2, 1, 1, 1299.99, 1299.99);

-- Verify Data

PRINT 'Database and tables created successfully!';
PRINT '';
PRINT 'Users:';
SELECT * FROM Users;
PRINT '';
PRINT 'Products:';
SELECT * FROM Products;
PRINT '';
PRINT 'Orders:';
SELECT * FROM Orders;
PRINT '';
PRINT 'OrderItems:';
SELECT * FROM OrderItems;