use DomainObjects_ObjectBinding_UnitTests

delete from [OrderTicket]
delete from [OrderItem]
delete from [Order]

-- Order
insert into [Order] (ID, ClassID, OrderNo, DeliveryDate) 
    values ('{5682F032-2F0B-494b-A31C-C97F02B89C36}', 'Order', 1, '2005/01/01')
    
insert into [Order] (ID, ClassID, OrderNo, DeliveryDate) 
    values ('{83445473-844A-4d3f-A8C3-C27F8D98E8BA}', 'Order', 3, '2005/03/01')

insert into [Order] (ID, ClassID, OrderNo, DeliveryDate) 
    values ('{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}', 'Order', 4, '2006/02/01')

insert into [Order] (ID, ClassID, OrderNo, DeliveryDate) 
    values ('{90E26C86-611F-4735-8D1B-E1D0918515C2}', 'Order', 5, '2006/03/01')


-- OrderItem
insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}', 'OrderItem', 
    '{5682F032-2F0B-494b-A31C-C97F02B89C36}', 1, 'Mainboard')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{AD620A11-4BC4-4791-BCF4-A0770A08C5B0}', 'OrderItem', 
    '{5682F032-2F0B-494b-A31C-C97F02B89C36}', 2, 'CPU Fan')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{0D7196A5-8161-4048-820D-B1BBDABE3293}', 'OrderItem', 
    '{83445473-844A-4d3f-A8C3-C27F8D98E8BA}', 1, 'Harddisk')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}', 'OrderItem', 
    '{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}', 1, 'Hitchhiker''s guide')

insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) 
    values ('{EA505094-770A-4505-82C1-5A4F94F56FE2}', 'OrderItem', 
    '{90E26C86-611F-4735-8D1B-E1D0918515C2}', 1, 'Blumentopf')


-- OrderTicket
insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{058EF259-F9CD-4cb1-85E5-5C05119AB596}', 'OrderTicket', 'C:\order1.png', '{5682F032-2F0B-494b-A31C-C97F02B89C36}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{BCF6C5F6-323F-4471-9CA5-7DF0A48C7A59}', 'OrderTicket', 'C:\order3.png', '{83445473-844A-4d3f-A8C3-C27F8D98E8BA}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{6768DB2B-9C66-4e2f-BBA2-89C56718FF2B}', 'OrderTicket', 'C:\order4.png', '{3C0FB6ED-DE1C-4e70-8D80-218E0BF58DF3}')

insert into [OrderTicket] (ID, ClassID, FileName, OrderID) 
    values ('{DC20E0EB-4B55-4f23-89CF-6D6478F96D3B}', 'OrderTicket', 'C:\order5.png', '{90E26C86-611F-4735-8D1B-E1D0918515C2}')

