USE DomainObjects_ObjectBinding_UnitTests
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

CREATE TABLE [Order] (
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  
  [OrderNo] int NOT NULL,
  [DeliveryDate] datetime NOT NULL,
  
  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([ID])
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