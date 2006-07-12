USE RubiconSecurityManager
GO

ALTER TABLE [User]
  ADD CONSTRAINT UniqueUserName UNIQUE NONCLUSTERED ([UserName])
