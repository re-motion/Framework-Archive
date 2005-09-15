USE PerformanceTestDomain

DELETE FROM [dbo].[File]
DELETE FROM [dbo].[Client]


INSERT INTO [dbo].[Client] ([ID], [ClassID], [Name]) VALUES ('6F20355F-FA99-4c4e-B432-02C41F7BD390', 'Client', 'TestClient')

declare @number int
set @number = 0

while @number < 500
begin
  set @number = @number + 1
  INSERT INTO [dbo].[File] ([ID], [ClassID], [Number], [File]) VALUES (NEWID(), 'File', 'File '+ cast (@number as varchar), '6F20355F-FA99-4c4e-B432-02C41F7BD390')
end
