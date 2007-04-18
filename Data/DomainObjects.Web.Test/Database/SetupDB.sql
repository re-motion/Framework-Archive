USE RpaTest
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TableForRelationTestView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TableForRelationTestView]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TableWithAllDataTypesView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TableWithAllDataTypesView]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithAllDataTypes') 
DROP TABLE [TableWithAllDataTypes]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableForRelationTest') 
DROP TABLE [TableForRelationTest]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithoutColumns') 
DROP TABLE [TableWithoutColumns]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'TableWithUndefinedEnum') 
DROP TABLE [TableWithUndefinedEnum]
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
  [StringWithoutMaxLength] text NOT NULL,
  [DelimitedStringArray] nvarchar (1000) NOT NULL,
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
  [DelimitedNullStringArray] nvarchar (1000) NULL,
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

CREATE TABLE [TableWithoutColumns] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  CONSTRAINT [PK_TableWithoutColumns] PRIMARY KEY CLUSTERED ([ID])
) 
GO

CREATE TABLE [TableWithUndefinedEnum] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [UndefinedEnum] int NOT NULL,

  CONSTRAINT [PK_TableWithUndefinedEnum] PRIMARY KEY CLUSTERED ([ID])
) 
GO


CREATE VIEW [TableWithAllDataTypesView] (
  [ID],
  [ClassID],
  [Timestamp],
  
  [TableWithAllDataTypes_Boolean],
  [TableWithAllDataTypes_Byte],
  [TableWithAllDataTypes_Date],
  [TableWithAllDataTypes_DateTime],
  [TableWithAllDataTypes_Decimal],
  [TableWithAllDataTypes_Double],
  [TableWithAllDataTypes_Enum],
  [TableWithAllDataTypes_Guid],
  [TableWithAllDataTypes_Int16],
  [TableWithAllDataTypes_Int32],
  [TableWithAllDataTypes_Int64],
  [TableWithAllDataTypes_Single],
  [TableWithAllDataTypes_String],
  [TableWithAllDataTypes_StringWithoutMaxLength],
  [TableWithAllDataTypes_DelimitedStringArray],
  [TableWithAllDataTypes_Binary],

  [TableWithAllDataTypes_NaBoolean],
  [TableWithAllDataTypes_NaByte],
  [TableWithAllDataTypes_NaDate],
  [TableWithAllDataTypes_NaDateTime],
  [TableWithAllDataTypes_NaDecimal],
  [TableWithAllDataTypes_NaDouble],
  [TableWithAllDataTypes_NaGuid],
  [TableWithAllDataTypes_NaInt16],
  [TableWithAllDataTypes_NaInt32],
  [TableWithAllDataTypes_NaInt64],
  [TableWithAllDataTypes_NaSingle],
  
  [TableWithAllDataTypes_StringWithNullValue],
  [TableWithAllDataTypes_DelimitedNullStringArray],
  [TableWithAllDataTypes_NaBooleanWithNullValue],
  [TableWithAllDataTypes_NaByteWithNullValue],
  [TableWithAllDataTypes_NaDateWithNullValue],
  [TableWithAllDataTypes_NaDateTimeWithNullValue],
  [TableWithAllDataTypes_NaDecimalWithNullValue],
  [TableWithAllDataTypes_NaDoubleWithNullValue],
  [TableWithAllDataTypes_NaGuidWithNullValue],
  [TableWithAllDataTypes_NaInt16WithNullValue],
  [TableWithAllDataTypes_NaInt32WithNullValue],
  [TableWithAllDataTypes_NaInt64WithNullValue],
  [TableWithAllDataTypes_NaSingleWithNullValue],
  [TableWithAllDataTypes_NullableBinary],
  
  [TableWithAllDataTypes_TableForRelationTestMandatory],
  [TableWithAllDataTypes_TableForRelationTestOptional])      
  WITH SCHEMABINDING AS
  SELECT 
    [ID],
    [ClassID],
    [Timestamp],
    
    [Boolean],
    [Byte],
    [Date],
    [DateTime],
    [Decimal],
    [Double],
    [Enum],
    [Guid],
    [Int16],
    [Int32],
    [Int64],
    [Single],
    [String],
    [StringWithoutMaxLength],
    [DelimitedStringArray],
    [Binary],

    [NaBoolean],
    [NaByte],
    [NaDate],
    [NaDateTime],
    [NaDecimal],
    [NaDouble],
    [NaGuid],
    [NaInt16],
    [NaInt32],
    [NaInt64],
    [NaSingle],
    
    [StringWithNullValue],
    [DelimitedNullStringArray],
    [NaBooleanWithNullValue],
    [NaByteWithNullValue],
    [NaDateWithNullValue],
    [NaDateTimeWithNullValue],
    [NaDecimalWithNullValue],
    [NaDoubleWithNullValue],
    [NaGuidWithNullValue],
    [NaInt16WithNullValue],
    [NaInt32WithNullValue],
    [NaInt64WithNullValue],
    [NaSingleWithNullValue],
    [NullableBinary],
    
    [TableForRelationTestMandatory],
    [TableForRelationTestOptional]
    FROM [dbo].[TableWithAllDataTypes]
    WHERE [ClassID] IN ('ClassWithAllDataTypes')
  WITH CHECK OPTION
GO

CREATE VIEW [TableForRelationTestView] (
  [ID],
  [ClassID],
  [Timestamp],
  
  [TableForRelationTest_Name],
  
  [TableForRelationTest_TableWithAllDataTypesMandatory],
  [TableForRelationTest_TableWithAllDataTypesOptional])      
  WITH SCHEMABINDING AS
  SELECT 
    [ID],
    [ClassID],
    [Timestamp],
    
    [Name],
    
    [TableWithAllDataTypesMandatory],
    [TableWithAllDataTypesOptional]    
    FROM [dbo].[TableForRelationTest]
    WHERE [ClassID] IN ('ClassForRelationTest')
  WITH CHECK OPTION
GO
