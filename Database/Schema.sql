USE master;
GO

-- Создаем базу данных, если она не существует
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TechSystemsDB')
BEGIN
    CREATE DATABASE [TechSystemsDB];
END
GO

USE [TechSystemsDB];
GO

-- Удаление старых таблиц в правильном порядке (учитывая внешние ключи)
IF OBJECT_ID('Applications', 'U') IS NOT NULL DROP TABLE Applications;
IF OBJECT_ID('Payments', 'U') IS NOT NULL DROP TABLE Payments;
IF OBJECT_ID('Tariffs', 'U') IS NOT NULL DROP TABLE Tariffs;
IF OBJECT_ID('Services', 'U') IS NOT NULL DROP TABLE Services;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('ApplicationStatuses', 'U') IS NOT NULL DROP TABLE ApplicationStatuses;
IF OBJECT_ID('PaymentMethods', 'U') IS NOT NULL DROP TABLE PaymentMethods;
IF OBJECT_ID('PaymentStatuses', 'U') IS NOT NULL DROP TABLE PaymentStatuses;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

-- 1. Роли пользователей
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL
);
GO

-- 2. Статусы заявок
CREATE TABLE ApplicationStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);
GO

-- 3. Способы оплаты
CREATE TABLE PaymentMethods (
    MethodID INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(50) NOT NULL
);
GO

-- 4. Статусы платежей
CREATE TABLE PaymentStatuses (
    PaymentStatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);
GO

-- 5. Категории услуг
CREATE TABLE ServiceCategories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);
GO

-- 6. Услуги
CREATE TABLE Services (
    ServiceID INT PRIMARY KEY IDENTITY(1,1),
    ServiceName NVARCHAR(200) NOT NULL,
    CategoryID INT NOT NULL,
    Description NVARCHAR(1000),
    FOREIGN KEY (CategoryID) REFERENCES ServiceCategories(CategoryID)
);
GO

-- 7. Пользователи
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Login NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

-- 8. Тарифы
CREATE TABLE Tariffs (
    TariffID INT PRIMARY KEY IDENTITY(1,1),
    TariffName NVARCHAR(200) NOT NULL,
    ServiceID INT NOT NULL,
    SubscriptionPeriodMonths INT NOT NULL,
    StartDate DATE NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Discount DECIMAL(5,2) NOT NULL DEFAULT 0,
    UserLimit INT NOT NULL,
    AvailableLicenses INT NOT NULL,
    PhotoFileName NVARCHAR(255),
    Description NVARCHAR(1000),
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID)
);
GO

-- 9. Заявки
CREATE TABLE Applications (
    ApplicationID INT PRIMARY KEY IDENTITY(1,1),
    TariffID INT NOT NULL,
    ClientID INT NOT NULL,
    ApplicationDate DATE NOT NULL DEFAULT GETDATE(),
    StatusID INT NOT NULL,
    LicenseCount INT NOT NULL,
    TotalCost DECIMAL(10,2) NOT NULL,
    Comment NVARCHAR(1000),
    FOREIGN KEY (TariffID) REFERENCES Tariffs(TariffID),
    FOREIGN KEY (ClientID) REFERENCES Users(UserID),
    FOREIGN KEY (StatusID) REFERENCES ApplicationStatuses(StatusID)
);
GO

-- 10. Платежи
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    ClientID INT NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethodID INT NOT NULL,
    PaymentStatusID INT NOT NULL,
    ApplicationID INT NULL,
    FOREIGN KEY (ClientID) REFERENCES Users(UserID),
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods(MethodID),
    FOREIGN KEY (PaymentStatusID) REFERENCES PaymentStatuses(PaymentStatusID),
    FOREIGN KEY (ApplicationID) REFERENCES Applications(ApplicationID)
);
GO

-- Создание индексов для улучшения производительности
CREATE INDEX IX_Tariffs_ServiceID ON Tariffs(ServiceID);
CREATE INDEX IX_Applications_TariffID ON Applications(TariffID);
CREATE INDEX IX_Applications_ClientID ON Applications(ClientID);
CREATE INDEX IX_Applications_StatusID ON Applications(StatusID);
CREATE INDEX IX_Payments_ClientID ON Payments(ClientID);
CREATE INDEX IX_Users_RoleID ON Users(RoleID);
GO

