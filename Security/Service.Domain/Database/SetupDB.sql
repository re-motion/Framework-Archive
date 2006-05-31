USE SecurityService
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('Client', 'Group', 'GroupType', 'ConcretePosition', 'Position', 'Role', 'User')
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

