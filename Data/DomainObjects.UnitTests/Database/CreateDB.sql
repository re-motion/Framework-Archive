
CREATE DATABASE TestDomain
ON PRIMARY (
	Name = 'TestDomain_Data',
	Filename = 'C:\Databases\TestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'TestDomain_Log',
	Filename = 'C:\Databases\TestDomain.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE TestDomain SET RECOVERY SIMPLE
GO