USE CodeGeneratorUnitTests1
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CompanyView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CustomerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CustomerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DevelopmentPartnerView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DevelopmentPartnerView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AddressView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AddressView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderItemView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OrderItemView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CeoView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CeoView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithAllDataTypesView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmployeeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EmployeeView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClassWithRelationsView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ClassWithRelationsView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ConcreteClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[ConcreteClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecondDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecondDerivedClassView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'DerivedOfDerivedClassView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[DerivedOfDerivedClassView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Customer', 'DevelopmentPartner', 'Address', 'Order', 'OrderItem', 'Ceo', 'TableWithAllDataTypes', 'Employee', 'ClassWithRelations', 'ConcreteClass')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Customer' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Customer]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'DevelopmentPartner' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[DevelopmentPartner]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Address' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Address]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Order' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Order]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'OrderItem' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[OrderItem]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Ceo' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Ceo]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[TableWithAllDataTypes]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Employee' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Employee]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ClassWithRelations' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ClassWithRelations]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcreteClass' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[ConcreteClass]
GO

-- Create all tables
CREATE TABLE [dbo].[Customer]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Company columns
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,
  [AddressID] uniqueidentifier NULL,

  -- Customer columns
  [CustomerType] int NOT NULL,
  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,
  [PrimaryOfficialID] varchar (255) NULL,

  CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[DevelopmentPartner]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Company columns
  [Name] nvarchar (100) NOT NULL,
  [PhoneNumber] nvarchar (100) NULL,
  [AddressID] uniqueidentifier NULL,

  -- Partner columns
  [Description] nvarchar (255) NOT NULL,
  [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,

  -- DevelopmentPartner columns
  [Competences] nvarchar (255) NOT NULL,

  CONSTRAINT [PK_DevelopmentPartner] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Address]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Address columns
  [Street] nvarchar (100) NOT NULL,
  [Zip] nvarchar (100) NOT NULL,
  [City] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Order]
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

CREATE TABLE [dbo].[OrderItem]
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

CREATE TABLE [dbo].[Ceo]
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

CREATE TABLE [dbo].[TableWithAllDataTypes]
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

CREATE TABLE [dbo].[Employee]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Employee columns
  [Name] nvarchar (100) NOT NULL,
  [SupervisorID] uniqueidentifier NULL,

  CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[ClassWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ClassWithRelations columns
  [DerivedClassID] uniqueidentifier NULL,
  [DerivedClassIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_ClassWithRelations] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[ConcreteClass]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ConcreteClass columns
  [PropertyInConcreteClass] nvarchar (100) NOT NULL,

  -- DerivedClass columns
  [PropertyInDerivedClass] nvarchar (100) NULL,

  -- DerivedOfDerivedClass columns
  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,
  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,

  -- SecondDerivedClass columns
  [PropertyInSecondDerivedClass] nvarchar (100) NULL,
  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,

  CONSTRAINT [PK_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[Customer] ADD
  CONSTRAINT [FK_AddressToCompany] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])

ALTER TABLE [dbo].[DevelopmentPartner] ADD
  CONSTRAINT [FK_AddressToCompany_1] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])

ALTER TABLE [dbo].[Order] ADD
  CONSTRAINT [FK_CustomerToOrder] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])

ALTER TABLE [dbo].[OrderItem] ADD
  CONSTRAINT [FK_OrderToOrderItem] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])

ALTER TABLE [dbo].[Employee] ADD
  CONSTRAINT [FK_EmployeeToEmployee] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[Employee] ([ID])

ALTER TABLE [dbo].[ClassWithRelations] ADD
  CONSTRAINT [FK_DerivedClassToClassWithRelations] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])

ALTER TABLE [dbo].[ConcreteClass] ADD
  CONSTRAINT [FK_ClassWithRelationsToDerivedOfDerivedClass] FOREIGN KEY ([ClassWithRelationsInDerivedOfDerivedClassID]) REFERENCES [dbo].[ClassWithRelations] ([ID]),
  CONSTRAINT [FK_ClassWithRelationsToSecondDerivedClass] FOREIGN KEY ([ClassWithRelationsInSecondDerivedClassID]) REFERENCES [dbo].[ClassWithRelations] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], null, null, null
    FROM [dbo].[Customer]
    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')
  UNION ALL
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], null, null, null, [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]
    FROM [dbo].[DevelopmentPartner]
    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')
GO

CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID]
    FROM [dbo].[Customer]
    WHERE [ClassID] IN ('Customer')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]
    FROM [dbo].[DevelopmentPartner]
    WHERE [ClassID] IN ('DevelopmentPartner')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DevelopmentPartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]
    FROM [dbo].[DevelopmentPartner]
    WHERE [ClassID] IN ('DevelopmentPartner')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AddressView] ([ID], [ClassID], [Timestamp], [Street], [Zip], [City])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Street], [Zip], [City]
    FROM [dbo].[Address]
    WHERE [ClassID] IN ('Address')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]
    FROM [dbo].[Order]
    WHERE [ClassID] IN ('Order')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[OrderItemView] ([ID], [ClassID], [Timestamp], [Position], [Product], [OrderID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Position], [Product], [OrderID]
    FROM [dbo].[OrderItem]
    WHERE [ClassID] IN ('OrderItem')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CeoView] ([ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [CompanyID], [CompanyIDClassID]
    FROM [dbo].[Ceo]
    WHERE [ClassID] IN ('Ceo')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithAllDataTypesView] ([ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [Binary], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Boolean], [Byte], [Date], [DateTime], [Decimal], [Double], [Enum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [Binary], [NaBoolean], [NaByte], [NaDate], [NaDateTime], [NaDecimal], [NaDouble], [NaGuid], [NaInt16], [NaInt32], [NaInt64], [NaSingle], [StringWithNullValue], [NaBooleanWithNullValue], [NaByteWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue], [NaDecimalWithNullValue], [NaDoubleWithNullValue], [NaGuidWithNullValue], [NaInt16WithNullValue], [NaInt32WithNullValue], [NaInt64WithNullValue], [NaSingleWithNullValue], [NullableBinary]
    FROM [dbo].[TableWithAllDataTypes]
    WHERE [ClassID] IN ('ClassWithAllDataTypes')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[EmployeeView] ([ID], [ClassID], [Timestamp], [Name], [SupervisorID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID]
    FROM [dbo].[Employee]
    WHERE [ClassID] IN ('Employee')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ClassWithRelationsView] ([ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [DerivedClassID], [DerivedClassIDClassID]
    FROM [dbo].[ClassWithRelations]
    WHERE [ClassID] IN ('ClassWithRelations')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('DerivedClass', 'DerivedOfDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SecondDerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('SecondDerivedClass')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[DerivedOfDerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID]
    FROM [dbo].[ConcreteClass]
    WHERE [ClassID] IN ('DerivedOfDerivedClass')
  WITH CHECK OPTION
GO
