USE RpaTest
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes') 
DROP TABLE [TableWithAllDataTypes]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableForRelationTest') 
DROP TABLE [TableForRelationTest]
GO

CREATE TABLE [TableWithAllDataTypes] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Boolean] bit NOT NULL,
  [Byte] tinyint NOT NULL,
  [Char] char (1) NOT NULL,
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
  
  [NaBoolean] bit NULL,
  [NaDate] dateTime NULL,
  [NaDateTime] dateTime NULL,
  [NaDouble] float NULL,
  [NaInt32] int NULL,
  
  [StringWithNullValue] nvarchar (100) NULL,
  [NaBooleanWithNullValue] bit NULL,
  [NaDateWithNullValue] dateTime NULL,
  [NaDateTimeWithNullValue] dateTime NULL,
  [NaDoubleWithNullValue] float NULL,
  [NaInt32WithNullValue] int NULL,
  
  [TableForRelationTestMandatory] uniqueidentifier NULL,
  [TableForRelationTestOptional] uniqueidentifier NULL,
      
  CONSTRAINT [PK_TableWithAllDataTypes] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableForRelationTest] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [Name] nvarchar (100) NOT NULL,
  
  [TableWithAllDataTypesMandatory] uniqueidentifier NULL,
  [TableWithAllDataTypesOptional] uniqueidentifier NULL,
      
  CONSTRAINT [PK_TableForRelationTest] PRIMARY KEY CLUSTERED ([ID])
) 
GO