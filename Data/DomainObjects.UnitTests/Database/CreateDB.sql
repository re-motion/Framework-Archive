
CREATE DATABASE TestDomain
ON PRIMARY (
	Name = 'TestDomain_Data',
	Filename = 'C:\Development\Libraries\Commons\Data\DomainObjects\UnitTests\Database\TestDomain.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'TestDomain_Log',
	Filename = 'C:\Development\Libraries\Commons\Data\DomainObjects\UnitTests\Database\TestDomain.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE TestDomain SET RECOVERY SIMPLE
GO