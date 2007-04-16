USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'TestDomainLegacy')
  DROP DATABASE TestDomainLegacy
GO
  
CREATE DATABASE TestDomainLegacy
ON PRIMARY (
	Name = 'TestDomainLegacy_Data',
	Filename = 'C:\Databases\TestDomainLegacy.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'TestDomainLegacy_Log',
	Filename = 'C:\Databases\TestDomainLegacy.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE TestDomainLegacy SET RECOVERY SIMPLE
BACKUP LOG TestDomainLegacy WITH TRUNCATE_ONLY
GO
