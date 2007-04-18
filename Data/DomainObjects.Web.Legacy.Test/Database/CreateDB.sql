
CREATE DATABASE RpaTestLegacy
ON PRIMARY (
	Name = 'RpaTestLegacy_Data',
	Filename = 'C:\Databases\RpaTestLegacy.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RpaTest_Log',
	Filename = 'C:\Databases\RpaTestLegacy.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE RpaTestLegacy SET RECOVERY SIMPLE
GO