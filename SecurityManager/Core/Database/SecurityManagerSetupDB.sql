USE RubiconSecurityManager
GO

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'ClientView')
  DROP VIEW [dbo].[ClientView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupView')
  DROP VIEW [dbo].[GroupView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupTypeView')
  DROP VIEW [dbo].[GroupTypeView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupTypePositionView')
  DROP VIEW [dbo].[GroupTypePositionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PositionView')
  DROP VIEW [dbo].[PositionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'RoleView')
  DROP VIEW [dbo].[RoleView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'UserView')
  DROP VIEW [dbo].[UserView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MetadataObjectView')
  DROP VIEW [dbo].[MetadataObjectView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EnumValueDefinitionView')
  DROP VIEW [dbo].[EnumValueDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecurableClassDefinitionView')
  DROP VIEW [dbo].[SecurableClassDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatePropertyReferenceView')
  DROP VIEW [dbo].[StatePropertyReferenceView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatePropertyDefinitionView')
  DROP VIEW [dbo].[StatePropertyDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateDefinitionView')
  DROP VIEW [dbo].[StateDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessTypeReferenceView')
  DROP VIEW [dbo].[AccessTypeReferenceView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessTypeDefinitionView')
  DROP VIEW [dbo].[AccessTypeDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AbstractRoleDefinitionView')
  DROP VIEW [dbo].[AbstractRoleDefinitionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateCombinationView')
  DROP VIEW [dbo].[StateCombinationView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateUsageView')
  DROP VIEW [dbo].[StateUsageView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessControlListView')
  DROP VIEW [dbo].[AccessControlListView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessControlEntryView')
  DROP VIEW [dbo].[AccessControlEntryView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PermissionView')
  DROP VIEW [dbo].[PermissionView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CultureView')
  DROP VIEW [dbo].[CultureView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'LocalizedNameView')
  DROP VIEW [dbo].[LocalizedNameView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Client', 'Group', 'GroupType', 'GroupTypePosition', 'Position', 'Role', 'User', 'EnumValueDefinition', 'SecurableClassDefinition', 'StatePropertyReference', 'StatePropertyDefinition', 'AccessTypeReference', 'StateCombination', 'StateUsage', 'AccessControlList', 'AccessControlEntry', 'Permission', 'Culture', 'LocalizedName')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client')
  DROP TABLE [dbo].[Client]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Group')
  DROP TABLE [dbo].[Group]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupType')
  DROP TABLE [dbo].[GroupType]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupTypePosition')
  DROP TABLE [dbo].[GroupTypePosition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Position')
  DROP TABLE [dbo].[Position]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Role')
  DROP TABLE [dbo].[Role]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'User')
  DROP TABLE [dbo].[User]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition')
  DROP TABLE [dbo].[EnumValueDefinition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SecurableClassDefinition')
  DROP TABLE [dbo].[SecurableClassDefinition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyReference')
  DROP TABLE [dbo].[StatePropertyReference]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyDefinition')
  DROP TABLE [dbo].[StatePropertyDefinition]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessTypeReference')
  DROP TABLE [dbo].[AccessTypeReference]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateCombination')
  DROP TABLE [dbo].[StateCombination]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateUsage')
  DROP TABLE [dbo].[StateUsage]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlList')
  DROP TABLE [dbo].[AccessControlList]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlEntry')
  DROP TABLE [dbo].[AccessControlEntry]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Permission')
  DROP TABLE [dbo].[Permission]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Culture')
  DROP TABLE [dbo].[Culture]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'LocalizedName')
  DROP TABLE [dbo].[LocalizedName]
GO

-- Create all tables
CREATE TABLE [dbo].[Client]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Client columns
  [Name] nvarchar (100) NOT NULL,
  [UniqueIdentifier] nvarchar (100) NOT NULL,
  [ParentID] uniqueidentifier NULL,

  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Group]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Group columns
  [Name] nvarchar (100) NOT NULL,
  [ShortName] nvarchar (10) NULL,
  [UniqueIdentifier] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,
  [ParentID] uniqueidentifier NULL,
  [GroupTypeID] uniqueidentifier NULL,

  CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[GroupType]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- GroupType columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_GroupType] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[GroupTypePosition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- GroupTypePosition columns
  [GroupTypeID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,

  CONSTRAINT [PK_GroupTypePosition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Position]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Position columns
  [Name] nvarchar (100) NOT NULL,
  [Delegation] int NOT NULL,

  CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Role]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Role columns
  [PositionID] uniqueidentifier NULL,
  [GroupID] uniqueidentifier NULL,
  [UserID] uniqueidentifier NULL,

  CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[User]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- User columns
  [Title] nvarchar (100) NULL,
  [FirstName] nvarchar (100) NULL,
  [LastName] nvarchar (100) NOT NULL,
  [UserName] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,
  [GroupID] uniqueidentifier NULL,

  CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[EnumValueDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- MetadataObject columns
  [Index] int NOT NULL,
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (200) NOT NULL,

  -- EnumValueDefinition columns
  [Value] int NOT NULL,

  -- StateDefinition columns
  [StatePropertyID] uniqueidentifier NULL,
  [StatePropertyIDClassID] varchar (100) NULL,

  -- AccessTypeDefinition columns

  -- AbstractRoleDefinition columns

  CONSTRAINT [PK_EnumValueDefinition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[SecurableClassDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- MetadataObject columns
  [Index] int NOT NULL,
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (200) NOT NULL,

  -- SecurableClassDefinition columns
  [ChangedAt] datetime NOT NULL,
  [BaseSecurableClassID] uniqueidentifier NULL,
  [BaseSecurableClassIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_SecurableClassDefinition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StatePropertyReference]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StatePropertyReference columns
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,
  [StatePropertyID] uniqueidentifier NULL,
  [StatePropertyIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_StatePropertyReference] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StatePropertyDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- MetadataObject columns
  [Index] int NOT NULL,
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (200) NOT NULL,

  -- StatePropertyDefinition columns

  CONSTRAINT [PK_StatePropertyDefinition] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[AccessTypeReference]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessTypeReference columns
  [Index] int NOT NULL,
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,
  [AccessTypeID] uniqueidentifier NULL,
  [AccessTypeIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_AccessTypeReference] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StateCombination]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateCombination columns
  [Index] int NOT NULL,
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,
  [AccessControlListID] uniqueidentifier NULL,

  CONSTRAINT [PK_StateCombination] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[StateUsage]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateUsage columns
  [StateCombinationID] uniqueidentifier NULL,
  [StateDefinitionID] uniqueidentifier NULL,
  [StateDefinitionIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_StateUsage] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[AccessControlList]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessControlList columns
  [ChangedAt] datetime NOT NULL,
  [Index] int NOT NULL,
  [SecurableClassID] uniqueidentifier NULL,
  [SecurableClassIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_AccessControlList] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[AccessControlEntry]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessControlEntry columns
  [ChangedAt] datetime NOT NULL,
  [Index] int NOT NULL,
  [ClientSelection] int NOT NULL,
  [GroupSelection] int NOT NULL,
  [UserSelection] int NOT NULL,
  [Priority] int NULL,
  [AccessControlListID] uniqueidentifier NULL,
  [ClientID] uniqueidentifier NULL,
  [GroupID] uniqueidentifier NULL,
  [GroupTypeID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,
  [UserID] uniqueidentifier NULL,
  [AbstractRoleID] uniqueidentifier NULL,
  [AbstractRoleIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_AccessControlEntry] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Permission]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Permission columns
  [Index] int NOT NULL,
  [Allowed] bit NULL,
  [AccessControlEntryID] uniqueidentifier NULL,
  [AccessTypeDefinitionID] uniqueidentifier NULL,
  [AccessTypeDefinitionIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[Culture]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Culture columns
  [CultureName] nvarchar (10) NOT NULL,

  CONSTRAINT [PK_Culture] PRIMARY KEY CLUSTERED ([ID])
)

CREATE TABLE [dbo].[LocalizedName]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- LocalizedName columns
  [Text] text NOT NULL,
  [CultureID] uniqueidentifier NULL,
  [MetadataObjectID] uniqueidentifier NULL,
  [MetadataObjectIDClassID] varchar (100) NULL,

  CONSTRAINT [PK_LocalizedName] PRIMARY KEY CLUSTERED ([ID])
)
GO

-- Create constraints for tables that were created above
ALTER TABLE [dbo].[Client] ADD
  CONSTRAINT [FK_ChildrenToParentClient] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Client] ([ID])

ALTER TABLE [dbo].[Group] ADD
  CONSTRAINT [FK_ClientToGroup] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID]),
  CONSTRAINT [FK_ChildrenToParentGroup] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Group] ([ID]),
  CONSTRAINT [FK_GroupTypeToGroup] FOREIGN KEY ([GroupTypeID]) REFERENCES [dbo].[GroupType] ([ID])

ALTER TABLE [dbo].[GroupTypePosition] ADD
  CONSTRAINT [FK_GroupTypeToGroupTypePosition] FOREIGN KEY ([GroupTypeID]) REFERENCES [dbo].[GroupType] ([ID]),
  CONSTRAINT [FK_PositionToGroupTypePosition] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Position] ([ID])

ALTER TABLE [dbo].[Role] ADD
  CONSTRAINT [FK_GroupToRole] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Group] ([ID]),
  CONSTRAINT [FK_PositionToRole] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Position] ([ID]),
  CONSTRAINT [FK_UserToRole] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([ID])

ALTER TABLE [dbo].[User] ADD
  CONSTRAINT [FK_ClientToUser] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID]),
  CONSTRAINT [FK_GroupToUser] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Group] ([ID])

ALTER TABLE [dbo].[EnumValueDefinition] ADD
  CONSTRAINT [FK_StatePropertyToState] FOREIGN KEY ([StatePropertyID]) REFERENCES [dbo].[StatePropertyDefinition] ([ID])

ALTER TABLE [dbo].[SecurableClassDefinition] ADD
  CONSTRAINT [FK_DerivedToBaseClass] FOREIGN KEY ([BaseSecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID])

ALTER TABLE [dbo].[StatePropertyReference] ADD
  CONSTRAINT [FK_ClassToStatePropertyReference] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_StatePropertyToStatePropertyReference] FOREIGN KEY ([StatePropertyID]) REFERENCES [dbo].[StatePropertyDefinition] ([ID])

ALTER TABLE [dbo].[AccessTypeReference] ADD
  CONSTRAINT [FK_ClassToAccessTypeReference] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_AccessTypeToAccessTypeReference] FOREIGN KEY ([AccessTypeID]) REFERENCES [dbo].[EnumValueDefinition] ([ID])

ALTER TABLE [dbo].[StateCombination] ADD
  CONSTRAINT [FK_ClassToStateCombination] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlListToStateCombination] FOREIGN KEY ([AccessControlListID]) REFERENCES [dbo].[AccessControlList] ([ID])

ALTER TABLE [dbo].[StateUsage] ADD
  CONSTRAINT [FK_StateDefinitionToStateUsage] FOREIGN KEY ([StateDefinitionID]) REFERENCES [dbo].[EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_StateCombinationToStateUsage] FOREIGN KEY ([StateCombinationID]) REFERENCES [dbo].[StateCombination] ([ID])

ALTER TABLE [dbo].[AccessControlList] ADD
  CONSTRAINT [FK_ClassToAccessControlList] FOREIGN KEY ([SecurableClassID]) REFERENCES [dbo].[SecurableClassDefinition] ([ID])

ALTER TABLE [dbo].[AccessControlEntry] ADD
  CONSTRAINT [FK_GroupToAccessControlEntry] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Group] ([ID]),
  CONSTRAINT [FK_GroupTypeToAccessControlEntry] FOREIGN KEY ([GroupTypeID]) REFERENCES [dbo].[GroupType] ([ID]),
  CONSTRAINT [FK_PositionToAccessControlEntry] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Position] ([ID]),
  CONSTRAINT [FK_UserToAccessControlEntry] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([ID]),
  CONSTRAINT [FK_AbstractRoleToAccessControlEntry] FOREIGN KEY ([AbstractRoleID]) REFERENCES [dbo].[EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlListToAccessControlEntries] FOREIGN KEY ([AccessControlListID]) REFERENCES [dbo].[AccessControlList] ([ID]),
  CONSTRAINT [FK_ClientToAccessControlEntry] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ID])

ALTER TABLE [dbo].[Permission] ADD
  CONSTRAINT [FK_AccessTypeDefinitionToPermission] FOREIGN KEY ([AccessTypeDefinitionID]) REFERENCES [dbo].[EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlEntryToPermission] FOREIGN KEY ([AccessControlEntryID]) REFERENCES [dbo].[AccessControlEntry] ([ID])

ALTER TABLE [dbo].[LocalizedName] ADD
  CONSTRAINT [FK_CultureToLocalizedName] FOREIGN KEY ([CultureID]) REFERENCES [dbo].[Culture] ([ID])
GO

-- Create a view for every class
CREATE VIEW [dbo].[ClientView] ([ID], [ClassID], [Timestamp], [Name], [UniqueIdentifier], [ParentID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [UniqueIdentifier], [ParentID]
    FROM [dbo].[Client]
    WHERE [ClassID] IN ('Client')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[GroupView] ([ID], [ClassID], [Timestamp], [Name], [ShortName], [UniqueIdentifier], [ClientID], [ParentID], [GroupTypeID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [ShortName], [UniqueIdentifier], [ClientID], [ParentID], [GroupTypeID]
    FROM [dbo].[Group]
    WHERE [ClassID] IN ('Group')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[GroupTypeView] ([ID], [ClassID], [Timestamp], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name]
    FROM [dbo].[GroupType]
    WHERE [ClassID] IN ('GroupType')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[GroupTypePositionView] ([ID], [ClassID], [Timestamp], [GroupTypeID], [PositionID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [GroupTypeID], [PositionID]
    FROM [dbo].[GroupTypePosition]
    WHERE [ClassID] IN ('GroupTypePosition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PositionView] ([ID], [ClassID], [Timestamp], [Name], [Delegation])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Name], [Delegation]
    FROM [dbo].[Position]
    WHERE [ClassID] IN ('Position')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[RoleView] ([ID], [ClassID], [Timestamp], [PositionID], [GroupID], [UserID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [PositionID], [GroupID], [UserID]
    FROM [dbo].[Role]
    WHERE [ClassID] IN ('Role')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[UserView] ([ID], [ClassID], [Timestamp], [Title], [FirstName], [LastName], [UserName], [ClientID], [GroupID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Title], [FirstName], [LastName], [UserName], [ClientID], [GroupID]
    FROM [dbo].[User]
    WHERE [ClassID] IN ('User')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[MetadataObjectView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID], [ChangedAt], [BaseSecurableClassID], [BaseSecurableClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID], null, null, null
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
  UNION
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], null, null, null, [ChangedAt], [BaseSecurableClassID], [BaseSecurableClassIDClassID]
    FROM [dbo].[SecurableClassDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
  UNION
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], null, null, null, null, null, null
    FROM [dbo].[StatePropertyDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition', 'SecurableClassDefinition', 'StatePropertyDefinition')
GO

CREATE VIEW [dbo].[EnumValueDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('EnumValueDefinition', 'StateDefinition', 'AccessTypeDefinition', 'AbstractRoleDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[SecurableClassDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [ChangedAt], [BaseSecurableClassID], [BaseSecurableClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [ChangedAt], [BaseSecurableClassID], [BaseSecurableClassIDClassID]
    FROM [dbo].[SecurableClassDefinition]
    WHERE [ClassID] IN ('SecurableClassDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StatePropertyReferenceView] ([ID], [ClassID], [Timestamp], [SecurableClassID], [SecurableClassIDClassID], [StatePropertyID], [StatePropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [SecurableClassID], [SecurableClassIDClassID], [StatePropertyID], [StatePropertyIDClassID]
    FROM [dbo].[StatePropertyReference]
    WHERE [ClassID] IN ('StatePropertyReference')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StatePropertyDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name]
    FROM [dbo].[StatePropertyDefinition]
    WHERE [ClassID] IN ('StatePropertyDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StateDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value], [StatePropertyID], [StatePropertyIDClassID]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('StateDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessTypeReferenceView] ([ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessTypeID], [AccessTypeIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessTypeID], [AccessTypeIDClassID]
    FROM [dbo].[AccessTypeReference]
    WHERE [ClassID] IN ('AccessTypeReference')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessTypeDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('AccessTypeDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AbstractRoleDefinitionView] ([ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [MetadataItemID], [Name], [Value]
    FROM [dbo].[EnumValueDefinition]
    WHERE [ClassID] IN ('AbstractRoleDefinition')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StateCombinationView] ([ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessControlListID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [SecurableClassID], [SecurableClassIDClassID], [AccessControlListID]
    FROM [dbo].[StateCombination]
    WHERE [ClassID] IN ('StateCombination')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[StateUsageView] ([ID], [ClassID], [Timestamp], [StateCombinationID], [StateDefinitionID], [StateDefinitionIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [StateCombinationID], [StateDefinitionID], [StateDefinitionIDClassID]
    FROM [dbo].[StateUsage]
    WHERE [ClassID] IN ('StateUsage')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessControlListView] ([ID], [ClassID], [Timestamp], [ChangedAt], [Index], [SecurableClassID], [SecurableClassIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ChangedAt], [Index], [SecurableClassID], [SecurableClassIDClassID]
    FROM [dbo].[AccessControlList]
    WHERE [ClassID] IN ('AccessControlList')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[AccessControlEntryView] ([ID], [ClassID], [Timestamp], [ChangedAt], [Index], [ClientSelection], [GroupSelection], [UserSelection], [Priority], [AccessControlListID], [ClientID], [GroupID], [GroupTypeID], [PositionID], [UserID], [AbstractRoleID], [AbstractRoleIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [ChangedAt], [Index], [ClientSelection], [GroupSelection], [UserSelection], [Priority], [AccessControlListID], [ClientID], [GroupID], [GroupTypeID], [PositionID], [UserID], [AbstractRoleID], [AbstractRoleIDClassID]
    FROM [dbo].[AccessControlEntry]
    WHERE [ClassID] IN ('AccessControlEntry')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[PermissionView] ([ID], [ClassID], [Timestamp], [Index], [Allowed], [AccessControlEntryID], [AccessTypeDefinitionID], [AccessTypeDefinitionIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Index], [Allowed], [AccessControlEntryID], [AccessTypeDefinitionID], [AccessTypeDefinitionIDClassID]
    FROM [dbo].[Permission]
    WHERE [ClassID] IN ('Permission')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[CultureView] ([ID], [ClassID], [Timestamp], [CultureName])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [CultureName]
    FROM [dbo].[Culture]
    WHERE [ClassID] IN ('Culture')
  WITH CHECK OPTION
GO

CREATE VIEW [dbo].[LocalizedNameView] ([ID], [ClassID], [Timestamp], [Text], [CultureID], [MetadataObjectID], [MetadataObjectIDClassID])
  WITH SCHEMABINDING AS
  SELECT [ID], [ClassID], [Timestamp], [Text], [CultureID], [MetadataObjectID], [MetadataObjectIDClassID]
    FROM [dbo].[LocalizedName]
    WHERE [ClassID] IN ('LocalizedName')
  WITH CHECK OPTION
GO
