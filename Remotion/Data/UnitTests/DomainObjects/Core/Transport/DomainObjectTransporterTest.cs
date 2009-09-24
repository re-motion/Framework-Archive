// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
{
  [TestFixture]
  public class DomainObjectTransporterTest : StandardMappingTest
  {
    private DomainObjectTransporter _transporter;
    private MemoryStream _stream;

    public override void SetUp ()
    {
      base.SetUp();
      _transporter = new DomainObjectTransporter();
      _stream = new MemoryStream();
    }

    public override void TearDown ()
    {
      _stream.Dispose();
      base.TearDown();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreEqual (0, _transporter.ObjectIDs.Count);
      Assert.IsEmpty (_transporter.ObjectIDs);
    }

    [Test]
    public void Load ()
    {
      DomainObject domainObject = _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EqualTo (new[] { DomainObjectIDs.Order1 }));

      Assert.AreSame (domainObject, _transporter.GetTransportedObject (domainObject.ID));
    }

    [Test]
    public void Load_Twice ()
    {
      DomainObject domainObject1 = _transporter.Load (DomainObjectIDs.Order1);
      DomainObject domainObject2 = _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EqualTo (new[] { DomainObjectIDs.Order1 }));
      Assert.AreSame (domainObject1, domainObject2);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException), ExpectedMessage = "Object 'Order|.*|System.Guid' could not be found.",
        MatchType = MessageMatch.Regex)]
    public void Load_Inexistent ()
    {
      _transporter.Load (new ObjectID (typeof (Order), Guid.NewGuid()));
    }

    [Test]
    public void Load_Multiple ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      Assert.AreEqual (1, _transporter.ObjectIDs.Count);
      _transporter.Load (DomainObjectIDs.Order2);
      Assert.AreEqual (2, _transporter.ObjectIDs.Count);
      _transporter.Load (DomainObjectIDs.OrderItem1);
      Assert.AreEqual (3, _transporter.ObjectIDs.Count);
      Assert.That (_transporter.ObjectIDs, Is.EqualTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 }));
    }

    [Test]
    public void LoadWithRelatedObjects ()
    {
      IEnumerable<DomainObject> loadedObjects = _transporter.LoadWithRelatedObjects (DomainObjectIDs.Order1);
      Assert.AreEqual (6, _transporter.ObjectIDs.Count);
      Assert.That (
          _transporter.ObjectIDs,
          Is.EquivalentTo (
              new[]
              {
                  DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2,
                  DomainObjectIDs.OrderTicket1, DomainObjectIDs.Customer1, DomainObjectIDs.Official1
              }));
      Assert.That (
          EnumerableUtility.ToArray (loadedObjects),
          Is.EquivalentTo (
              new object[]
              {
                  _transporter.GetTransportedObject (DomainObjectIDs.Order1), _transporter.GetTransportedObject (DomainObjectIDs.OrderItem1),
                  _transporter.GetTransportedObject (DomainObjectIDs.OrderItem2),
                  _transporter.GetTransportedObject (DomainObjectIDs.OrderTicket1), _transporter.GetTransportedObject (DomainObjectIDs.Customer1),
                  _transporter.GetTransportedObject (DomainObjectIDs.Official1)
              }));
    }

    [Test]
    public void LoadRecursive ()
    {
      IEnumerable<DomainObject> loadedObjects = _transporter.LoadRecursive (DomainObjectIDs.Employee1);
      Assert.AreEqual (5, _transporter.ObjectIDs.Count);
      Assert.That (
          _transporter.ObjectIDs,
          Is.EquivalentTo (
              new[]
              {
                  DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Computer2,
                  DomainObjectIDs.Employee5, DomainObjectIDs.Computer3
              }));
      Assert.That (
          EnumerableUtility.ToArray (loadedObjects),
          Is.EquivalentTo (
              new object[]
              {
                  _transporter.GetTransportedObject (DomainObjectIDs.Employee1), _transporter.GetTransportedObject (DomainObjectIDs.Employee4),
                  _transporter.GetTransportedObject (DomainObjectIDs.Computer2),
                  _transporter.GetTransportedObject (DomainObjectIDs.Employee5), _transporter.GetTransportedObject (DomainObjectIDs.Computer3)
              }));
    }

    [Test]
    public void LoadRecursive_WithStrategy_ShouldFollow ()
    {
      var strategy = new FollowOnlyOneLevelStrategy();
      IEnumerable<DomainObject> loadedObjects = _transporter.LoadRecursive (DomainObjectIDs.Employee1, strategy);
      Assert.That (
          _transporter.ObjectIDs, Is.EquivalentTo (new[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5 }));
      Assert.That (
          EnumerableUtility.ToArray (loadedObjects),
          Is.EquivalentTo (
              new object[]
              {
                  _transporter.GetTransportedObject (DomainObjectIDs.Employee1), _transporter.GetTransportedObject (DomainObjectIDs.Employee4),
                  _transporter.GetTransportedObject (DomainObjectIDs.Employee5)
              }));
    }

    [Test]
    public void LoadRecursive_WithStrategy_ShouldProcess ()
    {
      var strategy = new OnlyProcessComputersStrategy();
      IEnumerable<DomainObject> loadedObjects = _transporter.LoadRecursive (DomainObjectIDs.Employee1, strategy);
      Assert.That (_transporter.ObjectIDs, Is.EquivalentTo (new[] { DomainObjectIDs.Computer2, DomainObjectIDs.Computer3 }));
      Assert.That (
          EnumerableUtility.ToArray (loadedObjects),
          Is.EquivalentTo (
              new object[]
              {
                  _transporter.GetTransportedObject (DomainObjectIDs.Computer2), _transporter.GetTransportedObject (DomainObjectIDs.Computer3)
              }));
    }

    [Test]
    public void LoadNew ()
    {
      var order = (Order) _transporter.LoadNew (typeof (Order), ParamList.Empty);
      Assert.IsNotNull (order);
      Assert.IsTrue (_transporter.IsLoaded (order.ID));
    }

    [Test]
    public void LoadTransportData ()
    {
      _transporter.Load (DomainObjectIDs.Employee1);
      _transporter.Load (DomainObjectIDs.Employee2);

      TransportedDomainObjects transportedObjects = ExportAndLoadTransportData();

      Assert.IsNotNull (transportedObjects);
      var domainObjects = new List<DomainObject> (transportedObjects.TransportedObjects);
      Assert.AreEqual (2, domainObjects.Count);
      Assert.That (domainObjects.ConvertAll (obj => obj.ID), Is.EquivalentTo (new[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }));
    }

    [Test]
    public void LoadTransportData_XmlStrategy ()
    {
      _transporter.Load (DomainObjectIDs.Employee1);
      _transporter.Load (DomainObjectIDs.Employee2);

      TransportedDomainObjects transportedObjects = ExportAndLoadTransportData (XmlImportStrategy.Instance, XmlExportStrategy.Instance);

      Assert.IsNotNull (transportedObjects);
      var domainObjects = new List<DomainObject> (transportedObjects.TransportedObjects);
      Assert.AreEqual (2, domainObjects.Count);
      Assert.That (domainObjects.ConvertAll (obj => obj.ID), Is.EquivalentTo (new[] { DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }));
    }

    [Test]
    [ExpectedException (typeof (TransportationException),
        ExpectedMessage = "Invalid data specified: End of Stream encountered before parsing was completed.")]
    public void LoadTransportData_InvalidData ()
    {
      using (var stream = new MemoryStream (new byte[] { 1, 2, 3 }))
      {
        DomainObjectTransporter.LoadTransportData (stream);
      }
    }

    [Test]
    public void IsLoaded_True ()
    {
      _transporter.Load (DomainObjectIDs.Employee1);
      Assert.IsTrue (_transporter.IsLoaded (DomainObjectIDs.Employee1));
    }

    [Test]
    public void IsLoaded_False ()
    {
      Assert.IsFalse (_transporter.IsLoaded (DomainObjectIDs.Employee1));
    }

    [Test]
    public void TransactionContainsMoreObjects_ThanAreTransported ()
    {
      _transporter.LoadRecursive (DomainObjectIDs.Employee1, new FollowAllProcessNoneStrategy());
      Assert.AreEqual (0, _transporter.ObjectIDs.Count);
      Assert.IsEmpty (new List<ObjectID> (_transporter.ObjectIDs));

      TransportedDomainObjects transportedObjects = ExportAndLoadTransportData();
      Assert.IsEmpty (transportedObjects.TransportedObjects);
    }

    [Test]
    public void Export ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      _transporter.Export (_stream);
      Assert.That (_stream.Position > 0);
    }

    [Test]
    public void Export_SpecialStrategy ()
    {
      DomainObject loadedObject1 = _transporter.Load (DomainObjectIDs.Order1);
      DomainObject loadedObject2 = _transporter.Load (DomainObjectIDs.Order2);

      var repository = new MockRepository();
      var strategyMock = repository.StrictMock<IExportStrategy>();

      strategyMock.Expect (
          mock => mock.Export (
                      Arg.Is (_stream),
                      Arg<TransportItem[]>.Matches (items => items.Length == 2 && items[0].ID == loadedObject1.ID && items[1].ID == loadedObject2.ID)));

      strategyMock.Replay();
      _transporter.Export (_stream, strategyMock);
      strategyMock.VerifyAllExpectations();
    }

    [Test]
    public void GetTransportedObject_ReturnsCorrectObject ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      var order = (Order) _transporter.GetTransportedObject (DomainObjectIDs.Order1);
      Assert.IsNotNull (order);
      Assert.AreEqual (DomainObjectIDs.Order1, order.ID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be "
                                                                      +
                                                                      "retrieved, it hasn't been loaded yet. Load it first, then retrieve it for editing.\r\nParameter name: loadedObjectID"
        )]
    public void GetTransportedObject_ThrowsOnUnloadedObject ()
    {
      _transporter.GetTransportedObject (DomainObjectIDs.Order1);
    }

    [Test]
    public void GetTransportedObject_ReturnsBoundObject ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      var order = (Order) _transporter.GetTransportedObject (DomainObjectIDs.Order1);
      Assert.IsTrue (order.HasBindingTransaction);
    }

    [Test]
    public void GetTransportedObject_GetSetPropertyValue ()
    {
      _transporter.Load (DomainObjectIDs.Order1);
      var order = (Order) _transporter.GetTransportedObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Assert.AreEqual (2, order.OrderNumber);
    }

    [Test]
    public void GetTransportedObject_GetSetRelatedObject_RealSide ()
    {
      _transporter.Load (DomainObjectIDs.Computer1);
      var computer = (Computer) _transporter.GetTransportedObject (DomainObjectIDs.Computer1);
      computer.Employee = null;
      Assert.IsNull (computer.Employee);
    }

    [Test]
    public void GetTransportedObject_GetSetRelatedObject_VirtualSide_Loaded ()
    {
      _transporter.Load (DomainObjectIDs.Computer1);
      _transporter.Load (DomainObjectIDs.Employee3);
      var employee = (Employee) _transporter.GetTransportedObject (DomainObjectIDs.Employee3);
      employee.Computer = null;
      Assert.IsNull (employee.Computer);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Object 'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid' "
                                                                              +
                                                                              "cannot be modified for transportation because it hasn't been loaded yet. Load it before manipulating it."
        )]
    public void GetTransportedObject_GetSetRelatedObject_VirtualSide_Unloaded ()
    {
      _transporter.Load (DomainObjectIDs.Employee3);
      var employee = (Employee) _transporter.GetTransportedObject (DomainObjectIDs.Employee3);
      employee.Computer = null;
    }

    private TransportedDomainObjects ExportAndLoadTransportData ()
    {
      _transporter.Export (_stream);
      _stream.Seek (0, SeekOrigin.Begin);
      return DomainObjectTransporter.LoadTransportData (_stream);
    }

    private TransportedDomainObjects ExportAndLoadTransportData (IImportStrategy importStrategy, IExportStrategy exportStrategy)
    {
      _transporter.Export (_stream, exportStrategy);
      _stream.Seek (0, SeekOrigin.Begin);
      return DomainObjectTransporter.LoadTransportData (_stream, importStrategy);
    }
  }
}
