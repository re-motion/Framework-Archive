use RpaTest

delete from [TableWithAllDataTypes]
delete from [TableForRelationTest]

-- TableWithAllDataTypes
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Char], [Date], [DateTime], [Decimal], [Double], 
    [Enum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [NaBoolean], [NaDate], [NaDateTime],
    [NaDouble], [NaInt32], [StringWithNullValue], [NaBooleanWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue],
    [NaDoubleWithNullValue], [NaInt32WithNullValue], [TableForRelationTestMandatory], [TableForRelationTestOptional]) 
    values ('{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', 'ClassWithAllDataTypes', 0, 85, 'a', '2005/01/01', '2005/01/01 17:00', 123456.789, 987654.321,
    1, '{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}', 32767, 2147483647, 9223372036854775807, 6789.321, 'abcdef���', 1, '2005/02/01', '2005/02/01 05:00',
    654321.789, -2147483647, null, null, null, null, null, null, '{0498C3E5-30FA-45f1-A9A1-966D4655C3C7}', '{EDCFDFDB-C7B8-4f9b-BC75-72DDB8E19C42}')
    
insert into [TableWithAllDataTypes] (ID, ClassID, [Boolean], [Byte], [Char], [Date], [DateTime], [Decimal], [Double], 
    [Enum], [Guid], [Int16], [Int32], [Int64], [Single], [String], [NaBoolean], [NaDate], [NaDateTime],
    [NaDouble], [NaInt32], [StringWithNullValue], [NaBooleanWithNullValue], [NaDateWithNullValue], [NaDateTimeWithNullValue],
    [NaDoubleWithNullValue], [NaInt32WithNullValue], [TableForRelationTestMandatory], [TableForRelationTestOptional]) 
    values ('{583EC716-8443-4b55-92BF-09F7C8768529}', 'ClassWithAllDataTypes', 1, 86, 'b', '2005/01/02', '2005/01/02 01:00', 654321.987, 456789.123,
    0, '{D2146236-FBD4-4b93-A835-26563FE3F043}', -32767, -2147483647, -9223372036854775807, -6789.321, '���fedcba', 1, '2005/02/02', '2005/02/02 15:00',
    -654321.789, 2147483647, null, null, null, null, null, null, '{7E86BD49-6D7D-4d73-87AC-9F7C5F46DEFB}', null)
    
-- TableForRelationTest
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{ED85682D-CD85-4af7-972B-FAFCBD3C7699}', 'ClassForRelationTest', 'RelationTest1', '{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', null)
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{F1EAB10F-D3B4-4eb2-8F23-51CE855DB3CC}', 'ClassForRelationTest', 'RelationTest2', '{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}', '{D2146236-FBD4-4b93-A835-26563FE3F043}')
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{EDCFDFDB-C7B8-4f9b-BC75-72DDB8E19C42}', 'ClassForRelationTest', 'RelationTest3', '{D2146236-FBD4-4b93-A835-26563FE3F043}', '{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}')    
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{0498C3E5-30FA-45f1-A9A1-966D4655C3C7}', 'ClassForRelationTest', 'RelationTest4', '{D2146236-FBD4-4b93-A835-26563FE3F043}', null)
insert into [TableForRelationTest] (ID, ClassID, [Name], [TableWithAllDataTypesMandatory], [TableWithAllDataTypesOptional])
    values ('{7E86BD49-6D7D-4d73-87AC-9F7C5F46DEFB}', 'ClassForRelationTest', 'RelationTest5', '{D2146236-FBD4-4b93-A835-26563FE3F043}', null)            
