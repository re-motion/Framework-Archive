USE TestDomain
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Computer') 
DROP TABLE [Computer]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee') 
DROP TABLE [Employee]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes') 
DROP TABLE [TableWithAllDataTypes]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutRelatedClassIDColumnAndDerivation') 
DROP TABLE [TableWithoutRelatedClassIDColumnAndDerivation]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutRelatedClassIDColumn') 
DROP TABLE [TableWithoutRelatedClassIDColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo') 
DROP TABLE [Ceo]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderTicket') 
DROP TABLE [OrderTicket]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem') 
DROP TABLE [OrderItem]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order') 
DROP TABLE [Order]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Company') 
DROP TABLE [Company]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'IndustrialSector') 
DROP TABLE [IndustrialSector]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Person') 
DROP TABLE [Person]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithValidRelations') 
DROP TABLE [TableWithValidRelations]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithInvalidRelation') 
DROP TABLE [TableWithInvalidRelation]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithGuidKey') 
DROP TABLE [TableWithGuidKey]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithKeyOfInvalidType') 
DROP TABLE [TableWithKeyOfInvalidType]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutIDColumn') 
DROP TABLE [TableWithoutIDColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutClassIDColumn') 
DROP TABLE [TableWithoutClassIDColumn]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutTimestampColumn') 
DROP TABLE [TableWithoutTimestampColumn]
GO

CREATE TABLE [Employee] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Supervisor_Employee] FOREIGN KEY ([SupervisorID]) REFERENCES [Employee] ([ID]),
) 
GO

CREATE TABLE [Computer] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [SerialNumber] varchar (20) NOT NULL,
  [EmployeeID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Computer] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Computer_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [Employee] ([ID]),
) 
GO

CREATE TABLE [Person] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [IndustrialSector] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_IndustrialSector] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [Company] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- Company columns
  [Name] varchar (100) NULL,
  [IndustrialSectorID] uniqueidentifier NULL,
  
  -- Customer columns
  CustomerSince datetime NULL,
  CustomerType int NULL,
  
  -- Partner columns
  ContactPersonID uniqueidentifier NULL,
  
  -- Supplier columns
  SupplierQuality int NULL, 
  
  -- Distributor columns
  NumberOfShops int NULL
  
  CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Partner_Person] FOREIGN KEY ([ContactPersonID]) REFERENCES [Person] ([ID]),
  CONSTRAINT [FK_Company_IndustrialSector] FOREIGN KEY ([IndustrialSectorID]) REFERENCES [IndustrialSector] ([ID])
) 
GO

CREATE TABLE [Order] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderNo] int NOT NULL,
  [DeliveryDate] datetime NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  [OfficialID] varchar (255) NULL,
  
  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Order_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [OrderItem] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderID] uniqueidentifier NULL,
  [Position] int NOT NULL,
  [Product] varchar (100) NOT NULL DEFAULT (''),
  
  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID]),
  
  -- A foreign key cannot be part of a unique constraint:
  -- CONSTRAINT [UN_OrderItem_Position] UNIQUE ([OrderID], [Position]),
  
  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])
) 
GO

CREATE TABLE [OrderTicket] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [FileName] nvarchar (255) NOT NULL,
  [OrderID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_OrderTicket] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_OrderTicket_Order] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])
) 
GO

CREATE TABLE [Ceo] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] nvarchar (100) NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Ceo_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [TableWithoutRelatedClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [PartnerID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumn] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumn_Partner] FOREIGN KEY ([PartnerID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [TableWithoutRelatedClassIDColumnAndDerivation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [CompanyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumnAndDerivation] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumnAndDerivation_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [TableWithAllDataTypes] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Char] char (1) NOT NULL,
  [DateTime] dateTime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float (53) NOT NULL,
  [Enum] int NOT NULL,
  [Guid] uniqueidentifier NOT NULL,
  [Int16] smallint NOT NULL,
  [Int32] int NOT NULL,
  [Int64] bigint NOT NULL,
  [Single] real NOT NULL,
  [String] nvarchar (100) NOT NULL,
  
  [NaBoolean] bit NULL,
  [NaDateTime] dateTime NULL,
  [NaDouble] float NULL,
  [NaInt32] int NULL,
  
  [StringWithNullValue] nvarchar (100) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaDateTimeWithNullValue] dateTime NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaInt32WithNullValue] int NULL,
      
  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithGuidKey] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithGuidKey] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithKeyOfInvalidType] (
  [ID] datetime NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithKeyOfInvalidType] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithoutIDColumn] (
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL
) 
GO

CREATE TABLE [TableWithoutClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  CONSTRAINT [PK_TableWithoutClassIDColumn] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithoutTimestampColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableWithoutTimestampColumn] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithValidRelations] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyOptionalID] uniqueidentifier NULL,
  [TableWithGuidKeyNonOptionalID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithValidRelations] PRIMARY KEY CLUSTERED ([ID]),
  
  CONSTRAINT [FK_TableWithGuidKey_TableWithValidRelations_Optional] 
      FOREIGN KEY ([TableWithGuidKeyOptionalID]) REFERENCES [TableWithGuidKey] ([ID]),
      
  CONSTRAINT [FK_TableWithGuidKey_TableWithValidRelations_NonOptional] 
      FOREIGN KEY ([TableWithGuidKeyNonOptionalID]) REFERENCES [TableWithGuidKey] ([ID])      
) 
GO

CREATE TABLE [TableWithInvalidRelation] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithInvalidRelation] PRIMARY KEY CLUSTERED ([ID])
  -- Note the lack of a foreign key referring to TableWithGuidKey
) 
GO


