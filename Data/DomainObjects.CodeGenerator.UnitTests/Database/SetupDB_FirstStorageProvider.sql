USE CodeGeneratorUnitTests1
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Customer', 'DevelopmentPartner', 'Order', 'OrderItem', 'Ceo', 'TableWithAllDataTypes', 'Employee')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer')
  DROP TABLE [Customer]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DevelopmentPartner')
  DROP TABLE [DevelopmentPartner]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order')
  DROP TABLE [Order]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem')
  DROP TABLE [OrderItem]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo')
  DROP TABLE [Ceo]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes')
  DROP TABLE [TableWithAllDataTypes]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee')
  DROP TABLE [Employee]
GO

-- Create all tables
CREATE TABLE [Customer]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Company columns
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,

  -- Customer columns
  [CustomerType] int NOT NULL,
  [PrimaryOfficialID] varchar (255) NULL,

  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [DevelopmentPartner]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Company columns
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,

  -- Partner columns
  [Description] nvarchar (255) NOT NULL,

  -- DevelopmentPartner columns
  [Competences] nvarchar (255) NOT NULL,

  CONSTRAINT [PK_DevelopmentPartner] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [Order]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Order columns
  [Number] int NOT NULL,
  [Priority] int NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  [OfficialID] varchar (255) NULL,

  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [OrderItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- OrderItem columns
  [Position] int NOT NULL,
  [Product] nvarchar (100) NOT NULL,
  [OrderID] uniqueidentifier NULL,

  CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [Ceo]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Ceo columns
  [Name] nvarchar (100) NOT NULL,
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_Ceo] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [TableWithAllDataTypes]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithAllDataTypes columns
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] datetime NOT NULL,
  [DateTime] datetime NOT NULL,
  [Decimal] decimal (38, 3) NOT NULL,
  [Double] float NOT NULL,
  [Enum] int NOT NULL,
  [Guid] uniqueidentifier NOT NULL,
  [Int16] smallint NOT NULL,
  [Int32] int NOT NULL,
  [Int64] bigint NOT NULL,
  [Single] real NOT NULL,
  [String] nvarchar (100) NOT NULL,
  [Binary] image NOT NULL,
  [NaBoolean] bit NULL,
  [NaByte] tinyint NULL,
  [NaDate] datetime NULL,
  [NaDateTime] datetime NULL,
  [NaDecimal] decimal (38, 3) NULL,
  [NaDouble] float NULL,
  [NaGuid] uniqueidentifier NULL,
  [NaInt16] smallint NULL,
  [NaInt32] int NULL,
  [NaInt64] bigint NULL,
  [NaSingle] real NULL,
  [StringWithNullValue] nvarchar (100) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaByteWithNullValue] tinyint NULL,
  [NaDateWithNullValue] datetime NULL,
  [NaDateTimeWithNullValue] datetime NULL,
  [NaDecimalWithNullValue] decimal (38, 3) NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaGuidWithNullValue] uniqueidentifier NULL,
  [NaInt16WithNullValue] smallint NULL,
  [NaInt32WithNullValue] int NULL,
  [NaInt64WithNullValue] bigint NULL,
  [NaSingleWithNullValue] real NULL,
  [NullableBinary] image NULL,

  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [Employee]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Employee columns
  [Name] nvarchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,

  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [Order] ADD
  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [Customer] ([ID])

ALTER TABLE [OrderItem] ADD
  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [Order] ([ID])

ALTER TABLE [Employee] ADD
  CONSTRAINT [FK_EmployeeToEmployee] FOREIGN KEY ([SupervisorID]) REFERENCES [Employee] ([ID])
GO
