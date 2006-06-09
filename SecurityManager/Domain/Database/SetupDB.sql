USE SecurityService
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Client', 'Group', 'GroupType', 'ConcretePosition', 'Position', 'Role', 'User', 'EnumValueDefinition', 'SecurableClassDefinition', 'StatePropertyReference', 'StatePropertyDefinition', 'EnumValueDefinition', 'AccessTypeReference', 'EnumValueDefinition', 'EnumValueDefinition', 'StateCombination', 'StateUsage', 'AccessControlList', 'AccessControlEntry', 'Permission')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Client')
DROP TABLE [Client]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Group')
DROP TABLE [Group]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupType')
DROP TABLE [GroupType]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'ConcretePosition')
DROP TABLE [ConcretePosition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Position')
DROP TABLE [Position]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Role')
DROP TABLE [Role]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'User')
DROP TABLE [User]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition')
DROP TABLE [EnumValueDefinition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SecurableClassDefinition')
DROP TABLE [SecurableClassDefinition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyReference')
DROP TABLE [StatePropertyReference]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyDefinition')
DROP TABLE [StatePropertyDefinition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition')
DROP TABLE [EnumValueDefinition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessTypeReference')
DROP TABLE [AccessTypeReference]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition')
DROP TABLE [EnumValueDefinition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition')
DROP TABLE [EnumValueDefinition]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateCombination')
DROP TABLE [StateCombination]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateUsage')
DROP TABLE [StateUsage]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlList')
DROP TABLE [AccessControlList]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlEntry')
DROP TABLE [AccessControlEntry]
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Permission')
DROP TABLE [Permission]
GO

CREATE TABLE [Client]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Client columns
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Group]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Group columns
  [Name] nvarchar (100) NOT NULL,
  [ShortName] nvarchar (10) NOT NULL,
  [ClientID] uniqueidentifier NULL,
  [ParentID] uniqueidentifier NULL,
  [GroupTypeID] uniqueidentifier NULL,

  CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [GroupType]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- GroupType columns
  [Name] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,

  CONSTRAINT [PK_GroupType] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [ConcretePosition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- ConcretePosition columns
  [Name] nvarchar (100) NOT NULL,
  [GroupTypeID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,

  CONSTRAINT [PK_ConcretePosition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Position]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Position columns
  [Name] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,

  CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Role]
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
GO

CREATE TABLE [User]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- User columns
  [Title] nvarchar (100) NOT NULL,
  [FirstName] nvarchar (100) NOT NULL,
  [LastName] nvarchar (100) NOT NULL,
  [UserName] nvarchar (100) NOT NULL,
  [ClientID] uniqueidentifier NULL,
  [GroupID] uniqueidentifier NULL,

  CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [EnumValueDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- EnumValueDefinition columns
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [Value] bigint NOT NULL,

  CONSTRAINT [PK_EnumValueDefinition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [SecurableClassDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- SecurableClassDefinition columns
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (100) NOT NULL,
  [BaseClassID] uniqueidentifier NULL,

  CONSTRAINT [PK_SecurableClassDefinition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [StatePropertyReference]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StatePropertyReference columns
  [ClassID] uniqueidentifier NULL,
  [StatePropertyID] uniqueidentifier NULL,

  CONSTRAINT [PK_StatePropertyReference] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [StatePropertyDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StatePropertyDefinition columns
  [MetadataItemID] uniqueidentifier NOT NULL,
  [Name] nvarchar (100) NOT NULL,

  CONSTRAINT [PK_StatePropertyDefinition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [EnumValueDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateDefinition columns
  [StatePropertyID] uniqueidentifier NULL,

  CONSTRAINT [PK_EnumValueDefinition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [AccessTypeReference]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccesTypeReference columns
  [ClassID] uniqueidentifier NULL,
  [AccessTypeID] uniqueidentifier NULL,

  CONSTRAINT [PK_AccessTypeReference] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [EnumValueDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessTypeDefinition columns

  CONSTRAINT [PK_EnumValueDefinition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [EnumValueDefinition]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AbstractRoleDefinition columns

  CONSTRAINT [PK_EnumValueDefinition] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [StateCombination]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateCombination columns
  [ClassDefinitionID] uniqueidentifier NULL,
  [AccessControlListID] uniqueidentifier NULL,

  CONSTRAINT [PK_StateCombination] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [StateUsage]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- StateUsage columns
  [StateCombinationID] uniqueidentifier NULL,
  [StateDefinitionID] uniqueidentifier NULL,

  CONSTRAINT [PK_StateUsage] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [AccessControlList]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessControlList columns
  [ClassDefinitionID] uniqueidentifier NULL,

  CONSTRAINT [PK_AccessControlList] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [AccessControlEntry]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- AccessControlEntry columns
  [ClientSelection] int NOT NULL,
  [GroupSelection] int NOT NULL,
  [UserSelection] int NOT NULL,
  [AccessControlListID] uniqueidentifier NULL,
  [GroupID] uniqueidentifier NULL,
  [GroupTypeID] uniqueidentifier NULL,
  [PositionID] uniqueidentifier NULL,
  [UserID] uniqueidentifier NULL,
  [AbstractRoleID] uniqueidentifier NULL,

  CONSTRAINT [PK_AccessControlEntry] PRIMARY KEY CLUSTERED ([ID])
)
GO

CREATE TABLE [Permission]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,

  -- Permission columns
  [AccessControlEntryID] uniqueidentifier NULL,
  [AccessTypeDefinitionID] uniqueidentifier NULL,

  CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([ID])
)
GO

ALTER TABLE [Group] ADD
  CONSTRAINT [FK_ClientToGroup] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID]),
  CONSTRAINT [FK_ChildrenToParentGroup] FOREIGN KEY ([ParentID]) REFERENCES [Group] ([ID]),
  CONSTRAINT [FK_GroupTypeToGroup] FOREIGN KEY ([GroupTypeID]) REFERENCES [GroupType] ([ID])
GO

ALTER TABLE [GroupType] ADD
  CONSTRAINT [FK_ClientToGroupType] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
GO

ALTER TABLE [ConcretePosition] ADD
  CONSTRAINT [FK_GroupTypeToConcretePosition] FOREIGN KEY ([GroupTypeID]) REFERENCES [GroupType] ([ID]),
  CONSTRAINT [FK_PositionToConcretePosition] FOREIGN KEY ([PositionID]) REFERENCES [Position] ([ID])
GO

ALTER TABLE [Position] ADD
  CONSTRAINT [FK_ClientToPosition] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID])
GO

ALTER TABLE [Role] ADD
  CONSTRAINT [FK_GroupToRole] FOREIGN KEY ([GroupID]) REFERENCES [Group] ([ID]),
  CONSTRAINT [FK_PositionToRole] FOREIGN KEY ([PositionID]) REFERENCES [Position] ([ID]),
  CONSTRAINT [FK_UserToRole] FOREIGN KEY ([UserID]) REFERENCES [User] ([ID])
GO

ALTER TABLE [User] ADD
  CONSTRAINT [FK_ClientToUser] FOREIGN KEY ([ClientID]) REFERENCES [Client] ([ID]),
  CONSTRAINT [FK_GroupToUser] FOREIGN KEY ([GroupID]) REFERENCES [Group] ([ID])
GO

ALTER TABLE [SecurableClassDefinition] ADD
  CONSTRAINT [FK_DerivedToBaseClass] FOREIGN KEY ([BaseClassID]) REFERENCES [SecurableClassDefinition] ([ID])
GO

ALTER TABLE [StatePropertyReference] ADD
  CONSTRAINT [FK_ClassToStatePropertyReference] FOREIGN KEY ([ClassID]) REFERENCES [SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_StatePropertyToStatePropertyReference] FOREIGN KEY ([StatePropertyID]) REFERENCES [StatePropertyDefinition] ([ID])
GO

ALTER TABLE [EnumValueDefinition] ADD
  CONSTRAINT [FK_StatePropertyToState] FOREIGN KEY ([StatePropertyID]) REFERENCES [StatePropertyDefinition] ([ID])
GO

ALTER TABLE [AccessTypeReference] ADD
  CONSTRAINT [FK_ClassToAccessTypeReference] FOREIGN KEY ([ClassID]) REFERENCES [SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_AccessTypeToAccessTypeReference] FOREIGN KEY ([AccessTypeID]) REFERENCES [EnumValueDefinition] ([ID])
GO

ALTER TABLE [StateCombination] ADD
  CONSTRAINT [FK_ClassToStateCombination] FOREIGN KEY ([ClassDefinitionID]) REFERENCES [SecurableClassDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlListToStateCombination] FOREIGN KEY ([AccessControlListID]) REFERENCES [AccessControlList] ([ID])
GO

ALTER TABLE [StateUsage] ADD
  CONSTRAINT [FK_StateDefinitionToStateUsage] FOREIGN KEY ([StateDefinitionID]) REFERENCES [EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_StateCombinationToStateUsage] FOREIGN KEY ([StateCombinationID]) REFERENCES [StateCombination] ([ID])
GO

ALTER TABLE [AccessControlList] ADD
  CONSTRAINT [FK_ClassToAccessControlList] FOREIGN KEY ([ClassDefinitionID]) REFERENCES [SecurableClassDefinition] ([ID])
GO

ALTER TABLE [AccessControlEntry] ADD
  CONSTRAINT [FK_GroupToAccessControlEntry] FOREIGN KEY ([GroupID]) REFERENCES [Group] ([ID]),
  CONSTRAINT [FK_GroupTypeToAccessControlEntry] FOREIGN KEY ([GroupTypeID]) REFERENCES [GroupType] ([ID]),
  CONSTRAINT [FK_PositionToAccessControlEntry] FOREIGN KEY ([PositionID]) REFERENCES [Position] ([ID]),
  CONSTRAINT [FK_UserToAccessControlEntry] FOREIGN KEY ([UserID]) REFERENCES [User] ([ID]),
  CONSTRAINT [FK_AbstractRoleToAccessControlEntry] FOREIGN KEY ([AbstractRoleID]) REFERENCES [EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlListToAccessControlEntries] FOREIGN KEY ([AccessControlListID]) REFERENCES [AccessControlList] ([ID])
GO

ALTER TABLE [Permission] ADD
  CONSTRAINT [FK_AccessTypeDefinitionToPermission] FOREIGN KEY ([AccessTypeDefinitionID]) REFERENCES [EnumValueDefinition] ([ID]),
  CONSTRAINT [FK_AccessControlEntryToPermission] FOREIGN KEY ([AccessControlEntryID]) REFERENCES [AccessControlEntry] ([ID])
GO

