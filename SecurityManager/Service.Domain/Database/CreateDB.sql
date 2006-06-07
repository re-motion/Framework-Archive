CREATE DATABASE SecurityService
ON PRIMARY (
	Name = 'SecurityService_Data',
	Filename = 'C:\Databases\SecurityService.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'SecurityService_Log',
	Filename = 'C:\Databases\SecurityService.ldf',
	Size = 10MB
)
GO

ALTER DATABASE SecurityService SET RECOVERY SIMPLE
GO