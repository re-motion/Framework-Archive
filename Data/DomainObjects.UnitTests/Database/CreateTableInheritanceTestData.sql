use TestDomain

delete from [TableInheritance_Address]
delete from [TableInheritance_Person]
delete from [TableInheritance_Client]


-- TableInheritance_Client
insert into [TableInheritance_Client] (ID, ClassID, [Name]) values ('{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'Client', 'rubicon')

-- TableInheritance_Person
insert into [TableInheritance_Person] (ID, ClassID, [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth])
    values ('{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'Person', 'UnitTests', GETDATE(), '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'Max', 'Mustermann', '1980/6/9')

-- TableInheritance_Address
insert into [TableInheritance_Address] (ID, ClassID, [Street], [Zip], [City], [Country], [PersonID], [PersonIDClassID]) 
    values ('{5D5AA233-7371-44bc-807F-7849E8B08302}', 'Address', 'Werdertorgasse 14', '1010', 'Wien', 'Österreich', 
    '{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'Person')
