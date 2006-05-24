use TestDomain

delete from [TableInheritance_Order]
delete from [TableInheritance_Address]
delete from [TableInheritance_HistoryEntry]
delete from [TableInheritance_Person]
delete from [TableInheritance_Region]
delete from [TableInheritance_OrganizationalUnit]
delete from [TableInheritance_Client]


-- TableInheritance_Client
insert into [TableInheritance_Client] (ID, ClassID, [Name]) values ('{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'Client', 'rubicon')

-- TableInheritance_OrganizationalUnit
insert into [TableInheritance_OrganizationalUnit] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [Name]) 
    values ('{C6F4E04D-0465-4a9e-A944-C9FD26E33C44}', 'OrganizationalUnit', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'UnitTests', GETDATE(), 'Entwicklung')

-- Note: This OrganizationalUnit has the same ID as PersonWithSameIDAsOrganizationalUnit.
--       A SqlProvider test checks that RdbmsProvider.LoadDataContainerByRelatedID uses the classID column.
--       This OrganizationalUnit must have an associated HistoryEntry.
insert into [TableInheritance_OrganizationalUnit] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [Name]) 
    values ('{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'OrganizationalUnit', null, 'UnitTests', GETDATE(), 'OrganizationalUnitWithSameIDAsPerson')

-- TableInheritance_Region
insert into [TableInheritance_Region] (ID, ClassID, [Name]) values ('{7905CF32-FBC2-47fe-AC40-3E398BEEA5AB}', 'Region', 'NÖ')

-- TableInheritance_Person 
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth])
    values ('{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'Person', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', 'UnitTests', GETDATE(), 'Max', 'Mustermann', '1980/6/9')

-- Note: This person has an OrganizationalUnit with the same ID. It needs at least one HistoryEntry.
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth])
    values ('{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'Person', null, 'UnitTests', GETDATE(), '', 'PersonWithSameIDAsOrganizationalUnit', '1980/6/9')

-- TableInheritance_Customer
insert into [TableInheritance_Person] (ID, ClassID, [ClientID], [RegionID], [CreatedBy], [CreatedAt], [FirstName], [LastName], [DateOfBirth], [CustomerType], [CustomerSince])
    values ('{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'Customer', '{F7AD91EF-AC75-4fe3-A427-E40312B12917}', '{7905CF32-FBC2-47fe-AC40-3E398BEEA5AB}',
    'UnitTests', GETDATE(), 'Zaphod', 'Beeblebrox', '1950/1/1', 1, '1992/12/24')

-- TableInheritance_HistoryEntry
insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{0A2A6302-9CCB-4ab2-B006-2F1D89526435}', 'HistoryEntry', '{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'Customer', '2006/5/24', 'Kunde angelegt')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{02D662F0-ED50-49b4-8A26-BB6025EDCA8C}', 'HistoryEntry', '{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'Customer', '2006/5/25', 'Name geändert')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{9CCCB590-B765-4bc3-B481-AAC67AEEAD7E}', 'HistoryEntry', '{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'Person', '2006/5/26', 'Person angelegt')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{840EC009-03FB-4221-85B9-72E480A47373}', 'HistoryEntry', '{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'Person', '2006/5/26', 'Person angelegt')

insert into [TableInheritance_HistoryEntry] (ID, ClassID, [OwnerID], [OwnerIDClassID], [HistoryDate], [Text])
    values ('{2C7FB7B3-EB16-43f9-BDDE-B8B3F23A93D2}', 'HistoryEntry', '{B969AFCB-2CDA-45ff-8490-EB52A86D5464}', 'OrganizationalUnit', '2006/5/27', 'OU angelegt')

-- TableInheritance_Address
insert into [TableInheritance_Address] (ID, ClassID, [PersonID], [PersonIDClassID], [Street], [Zip], [City], [Country]) 
    values ('{5D5AA233-7371-44bc-807F-7849E8B08302}', 'Address', '{21E9BEA1-3026-430a-A01E-E9B6A39928A8}', 'Person', 'Werdertorgasse 14', '1010', 'Wien', 'Österreich')

-- TableInheritance_Order
insert into [TableInheritance_Order] (ID, ClassID, [CustomerID], [CustomerIDClassID], [Number], [OrderDate]) 
    values ('{6B88B60C-1C91-4005-8C60-72053DB48D5D}', 'Order', '{623016F9-B525-4CAE-A2BD-D4A6155B2F33}', 'Customer', 1, '2006/05/24')
