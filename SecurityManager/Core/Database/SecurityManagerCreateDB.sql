USE master

IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'RubiconSecurityManager')
BEGIN
  ALTER DATABASE RubiconSecurityManager SET SINGLE_USER WITH ROLLBACK IMMEDIATE
  DROP DATABASE RubiconSecurityManager
END
GO

CREATE DATABASE RubiconSecurityManager
ON PRIMARY (
	Name = 'RubiconSecurityManager_Data',
	Filename = 'C:\Databases\RubiconSecurityManager.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RubiconSecurityManager_Log',
	Filename = 'C:\Databases\RubiconSecurityManager.ldf',
	Size = 10MB
)
GO

ALTER DATABASE RubiconSecurityManager SET RECOVERY SIMPLE
BACKUP LOG RubiconSecurityManager WITH TRUNCATE_ONLY
GO
