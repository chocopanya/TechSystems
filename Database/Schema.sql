USE [TechSystemsDB]
GO

-- ”даление таблиц в правильном пор€дке (сначала дочерние, затем родительские)
IF OBJECT_ID('ApplicationStatusHistory', 'U') IS NOT NULL DROP TABLE ApplicationStatusHistory;
IF OBJECT_ID('Payments', 'U') IS NOT NULL DROP TABLE Payments;
IF OBJECT_ID('Applications', 'U') IS NOT NULL DROP TABLE Applications;
IF OBJECT_ID('Tariffs', 'U') IS NOT NULL DROP TABLE Tariffs;
IF OBJECT_ID('Services', 'U') IS NOT NULL DROP TABLE Services;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('ServiceTypes', 'U') IS NOT NULL DROP TABLE ServiceTypes;
IF OBJECT_ID('ServiceCategories', 'U') IS NOT NULL DROP TABLE ServiceCategories;
IF OBJECT_ID('ApplicationStatuses', 'U') IS NOT NULL DROP TABLE ApplicationStatuses;
IF OBJECT_ID('PaymentStatuses', 'U') IS NOT NULL DROP TABLE PaymentStatuses;
IF OBJECT_ID('PaymentMethods', 'U') IS NOT NULL DROP TABLE PaymentMethods;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

-- —оздание таблиц
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE ServiceTypes (
    ServiceTypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(200),
    Phone NVARCHAR(20),
    CompanyName NVARCHAR(200),
    CreatedDate DATE DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE Tariffs (
    TariffID INT PRIMARY KEY IDENTITY(1,1),
    TariffName NVARCHAR(200) NOT NULL,
    ServiceTypeID INT NOT NULL,
    SubscriptionMonths INT NOT NULL,
    StartDate DATE NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    UserLimit INT NOT NULL,
    AvailableLicenses INT NOT NULL,
    Discount DECIMAL(5,2) DEFAULT 0,
    PhotoFileName NVARCHAR(255),
    Description NVARCHAR(MAX),
    FOREIGN KEY (ServiceTypeID) REFERENCES ServiceTypes(ServiceTypeID)
);

CREATE TABLE ApplicationStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);

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

-- —оздадим таблицу дл€ платежей (по желанию, если нужна)
CREATE TABLE PaymentMethods (
    MethodID INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(50) NOT NULL
);

CREATE TABLE PaymentStatuses (
    PaymentStatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);

CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    ClientID INT NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethodID INT NOT NULL,
    StatusID INT NOT NULL,
    FOREIGN KEY (ClientID) REFERENCES Users(UserID),
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods(MethodID),
    FOREIGN KEY (StatusID) REFERENCES PaymentStatuses(PaymentStatusID)
);

-- “аблица дл€ истории статусов (опционально)
CREATE TABLE ApplicationStatusHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    ApplicationID INT NOT NULL,
    OldStatusID INT,
    NewStatusID INT NOT NULL,
    ChangeDate DATETIME NOT NULL DEFAULT GETDATE(),
    ChangedByUserID INT,
    Comment NVARCHAR(500),
    FOREIGN KEY (ApplicationID) REFERENCES Applications(ApplicationID),
    FOREIGN KEY (OldStatusID) REFERENCES ApplicationStatuses(StatusID),
    FOREIGN KEY (NewStatusID) REFERENCES ApplicationStatuses(StatusID),
    FOREIGN KEY (ChangedByUserID) REFERENCES Users(UserID)
);
GO