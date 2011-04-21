USE SchemaGenerationTestDomain2
GO

-- Create all tables
CREATE TABLE [dbo].[Official]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [ResponsibleForOrderPriority] int NOT NULL,
  [ResponsibleForCustomerType] int NOT NULL,
  [Speciality] nvarchar (255) NULL,
  CONSTRAINT [PK_Official] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
GO

-- Create a view for every class
CREATE VIEW [dbo].[OfficialView] ([ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality]
    FROM [dbo].[Official]
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SpecialOfficialView] ([ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ResponsibleForOrderPriority], [ResponsibleForCustomerType], [Speciality]
    FROM [dbo].[Official]
    WHERE [ClassID] IN ('SpecialOfficial')
  WITH CHECK OPTION
GO

-- Create indexes for tables that were created above
GO

-- Create synonyms for tables that were created above
GO

