USE <Database>
GO

ALTER TABLE [User]
  ADD CONSTRAINT UniqueUserName UNIQUE NONCLUSTERED ([UserName])

ALTER TABLE [Group]
  ADD CONSTRAINT UniqueIdentifier UNIQUE NONCLUSTERED ([UniqueIdentifier])
