USE <Database>
GO

ALTER TABLE [Client]
  ADD CONSTRAINT ClientUniqueIdentifier UNIQUE NONCLUSTERED ([UniqueIdentifier])

ALTER TABLE [User]
  ADD CONSTRAINT UniqueUserName UNIQUE NONCLUSTERED ([UserName])

ALTER TABLE [Group]
  ADD CONSTRAINT GroupUniqueIdentifier UNIQUE NONCLUSTERED ([UniqueIdentifier])
