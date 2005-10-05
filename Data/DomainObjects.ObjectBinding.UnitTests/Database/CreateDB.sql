USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'DomainObjects_ObjectBinding_UnitTests')
  DROP DATABASE DomainObjects_ObjectBinding_UnitTests
GO  
  
CREATE DATABASE DomainObjects_ObjectBinding_UnitTests
ON PRIMARY (
	Name = 'DomainObjects_ObjectBinding_UnitTests_Data',
	Filename = 'C:\Databases\DomainObjects_ObjectBinding_UnitTests.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'DomainObjects_ObjectBinding_UnitTests_Log',
	Filename = 'C:\Databases\DomainObjects_ObjectBinding_UnitTests.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE DomainObjects_ObjectBinding_UnitTests SET RECOVERY SIMPLE
BACKUP LOG DomainObjects_ObjectBinding_UnitTests WITH TRUNCATE_ONLY
GO