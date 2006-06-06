USE CodeGeneratorUnitTests2
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Official')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Official')
  DROP TABLE [Official]
GO

-- Create all tables
CREATE TABLE [Official]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Official columns
  [Name] nvarchar (100) NOT NULL,
  [ResponsibleForOrderPriority] int NOT NULL,
  [ResponsibleForCustomerType] int NOT NULL,

  CONSTRAINT [PK_Official] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
GO
