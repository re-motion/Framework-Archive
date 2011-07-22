// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ObjectIDReaderTest : SqlProviderBaseTest
  {
    private IDataReader _dataReaderStub;
    private ObjectIDReader _reader;
    private IRdbmsStoragePropertyDefinition _idPropertyStub;
    private ObjectID _objectID;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _idPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();

      _reader = new ObjectIDReader (_idPropertyStub, _columnOrdinalProviderStub);

      _objectID = new ObjectID ("Order", Guid.NewGuid());
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStub.Stub (stub => stub.Read()).Return (false);

     var result = _reader.Read (_dataReaderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).Repeat.Once ();
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (_objectID);

      var result = _reader.Read (_dataReaderStub);

      Assert.That (result, Is.SameAs (_objectID));
    }

    [Test]
    public void Read_DataReaderReadTrue_Null ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).Repeat.Once ();
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (null);

      var result = _reader.Read (_dataReaderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ReadSequence ()
    {
      var objectID2 = new ObjectID ("OrderItem", Guid.NewGuid ());
      _dataReaderStub.Stub (stub => stub.Read ()).Return (true).Repeat.Times (3);
      _dataReaderStub.Stub (stub => stub.Read ()).Return (false).Repeat.Once ();
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (_objectID).Repeat.Once ();
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (null).Repeat.Once ();
      _idPropertyStub.Stub (stub => stub.Read (_dataReaderStub, _columnOrdinalProviderStub)).Return (objectID2).Repeat.Once ();

      var result = _reader.ReadSequence (_dataReaderStub).ToArray ();

      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.SameAs (_objectID));
      Assert.That (result[1], Is.Null);
      Assert.That (result[2], Is.SameAs (objectID2));
    }

    [Test]
    public void ReadSequence_NoData ()
    {
      _dataReaderStub.Stub (stub => stub.Read ()).Return (false).Repeat.Once ();

      var result = _reader.ReadSequence (_dataReaderStub);

      Assert.That (result, Is.Empty);
    }
    
  }
}