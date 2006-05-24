USE TestDomain
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Location') 
DROP TABLE [Location]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client') 
DROP TABLE [Client]
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

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithOptionalOneToOneRelationAndOppositeDerivedClass') 
DROP TABLE [TableWithOptionalOneToOneRelationAndOppositeDerivedClass]
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

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FileSystemItem')
DROP TABLE [FileSystemItem]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithValidRelations') 
DROP TABLE [TableWithValidRelations]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithInvalidRelation') 
DROP TABLE [TableWithInvalidRelation]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithRelatedClassIDColumnAndNoInheritance') 
DROP TABLE [TableWithRelatedClassIDColumnAndNoInheritance]
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

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Order') 
DROP TABLE [TableInheritance_Order]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Address') 
DROP TABLE [TableInheritance_Address]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_HistoryEntry') 
DROP TABLE [TableInheritance_HistoryEntry]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Person') 
DROP TABLE [TableInheritance_Person]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Region') 
DROP TABLE [TableInheritance_Region]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_OrganizationalUnit') 
DROP TABLE [TableInheritance_OrganizationalUnit]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableInheritance_Client') 
DROP TABLE [TableInheritance_Client]
GO

IF OBJECT_ID ('rpf_testSPQuery', 'P') IS NOT NULL 
  DROP PROCEDURE rpf_testSPQuery;
GO

IF OBJECT_ID ('rpf_testSPQueryWithParameter', 'P') IS NOT NULL 
  DROP PROCEDURE rpf_testSPQueryWithParameter;
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

CREATE TABLE [Client] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ParentClientID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_ParentClient_ChildClient] FOREIGN KEY ([ParentClientID]) REFERENCES [Client] ([ID])
) 
GO

CREATE TABLE [Location] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [ClientID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_Client_Location] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
) 
GO

CREATE TABLE [TableWithoutRelatedClassIDColumn] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [DistributorID] uniqueidentifier NULL,
  
  CONSTRAINT [PK_TableWithoutRelatedClassIDColumn] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithoutRelatedClassIDColumn_Distributor] FOREIGN KEY ([DistributorID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [FileSystemItem]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- FileSystemItem columns
  [ParentFolderID] uniqueidentifier NULL,
  [ParentFolderIDClassID] varchar (100) NULL,

  -- Folder columns

  -- File columns

  CONSTRAINT [PK_FileSystemItem] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_FolderToFileSystemItem] FOREIGN KEY ([ParentFolderID]) REFERENCES [FileSystemItem] ([ID])
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

CREATE TABLE [TableWithOptionalOneToOneRelationAndOppositeDerivedClass] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [CompanyID] uniqueidentifier NULL,
  [CompanyIDClassID] varchar (100) NULL,
 
  CONSTRAINT [PK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableWithOptionalOneToOneRelationAndOppositeDerivedClass_Company] FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
) 
GO

CREATE TABLE [TableWithAllDataTypes] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Date] dateTime NOT NULL,
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
  [Binary] image NOT NULL,
  
  [NaBoolean] bit NULL,
  [NaByte] tinyint NULL,
  [NaDate] dateTime NULL,
  [NaDateTime] dateTime NULL,
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
  [NaDateWithNullValue] dateTime NULL,
  [NaDateTimeWithNullValue] dateTime NULL,
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

CREATE TABLE [TableWithRelatedClassIDColumnAndNoInheritance] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [TableWithGuidKeyID] uniqueidentifier NULL,
  [TableWithGuidKeyIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableWithRelatedClassIDColumnAndNoInheritance] PRIMARY KEY CLUSTERED ([ID]),
  
  CONSTRAINT [FK_TableWithGuidKey_TableWithRelatedClassIDColumnAndNoInheritance] 
      FOREIGN KEY ([TableWithGuidKeyID]) REFERENCES [TableWithGuidKey] ([ID])
) 
GO

CREATE TABLE [TableInheritance_Client] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableInheritance_Client] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_OrganizationalUnit] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- DomainBase columns
  [CreatedBy] varchar (100) NOT NULL,
  [CreatedAt] datetime NOT NULL,
  [ClientID] uniqueidentifier NULL,
  
  -- OrganizationalUnit columns
  [Name] varchar (100) NOT NULL,
    
  CONSTRAINT [PK_TableInheritance_OrganizationalUnit] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_OrganizationalUnit] FOREIGN KEY ([ClientID]) REFERENCES [TableInheritance_Client] ([ID])
) 
GO

CREATE TABLE [TableInheritance_Region] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] varchar (100) NOT NULL,
  
  CONSTRAINT [PK_TableInheritance_Region] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_Person] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- DomainBase columns
  [CreatedBy] varchar (100) NOT NULL,
  [CreatedAt] datetime NOT NULL,
  [ClientID] uniqueidentifier NULL,
  
  -- Person columns
  [FirstName] varchar (100) NOT NULL,
  [LastName] varchar (100) NOT NULL,
  [DateOfBirth] datetime NOT NULL,     

  -- Customer columns
  [CustomerType] int NULL,
  [CustomerSince] datetime NULL,
  [RegionID] uniqueidentifier NULL,
    
  CONSTRAINT [PK_TableInheritance_Person] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Client_TableInheritance_Person] FOREIGN KEY ([ClientID]) REFERENCES [TableInheritance_Client] ([ID]),
  CONSTRAINT [FK_TableInheritance_Region_TableInheritance_Customer] FOREIGN KEY ([RegionID]) REFERENCES [TableInheritance_Region] ([ID])
) 
GO

CREATE TABLE [TableInheritance_HistoryEntry] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  -- Person columns
  [HistoryDate] datetime NOT NULL,
  [Text] varchar (250) NOT NULL,
  [OwnerID] uniqueidentifier NULL, -- Note: OwnerID has no FK, because it refers to multiple tables (concrete table inheritance).
  [OwnerIDClassID] varchar (100) NULL,
   
  CONSTRAINT [PK_TableInheritance_HistoryEntry] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableInheritance_Address] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Street] nvarchar (100) NOT NULL,
  [Zip] nvarchar (10) NOT NULL,
  [City] nvarchar (100) NOT NULL,
  [Country] nvarchar (100) NOT NULL,
  [PersonID] uniqueidentifier NULL,
  [PersonIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_Address] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Person_TableInheritance_Address] FOREIGN KEY ([PersonID]) REFERENCES [TableInheritance_Person] ([ID])  
) 
GO

CREATE TABLE [TableInheritance_Order] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Number] int NOT NULL,
  [OrderDate] datetime NOT NULL,
  [CustomerID] uniqueidentifier NULL,
  [CustomerIDClassID] varchar (100) NULL,
  
  CONSTRAINT [PK_TableInheritance_Order] PRIMARY KEY CLUSTERED ([ID]),
  CONSTRAINT [FK_TableInheritance_Customer_TableInheritance_Order] FOREIGN KEY ([CustomerID]) REFERENCES [TableInheritance_Person] ([ID])  
) 
GO


CREATE PROCEDURE rpf_testSPQuery
AS
  SELECT * FROM [Order] WHERE [Order].[OrderNo] = 1 OR [Order].[OrderNo] = 3 ORDER BY OrderNo ASC
GO

CREATE PROCEDURE rpf_testSPQueryWithParameter
  @customerID uniqueidentifier
AS
  SELECT * FROM [Order] WHERE [Order].[CustomerID] = @customerID ORDER BY OrderNo ASC
GO
