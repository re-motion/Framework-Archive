// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class DataContainersPendingRegistrationCollectorTest : StandardMappingTest
  {
    private DataContainer _dataContainer1;
    private DataContainer _dataContainer2;
    private DataContainersPendingRegistrationCollector _collector;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataContainer1 = DataContainerObjectMother.Create (DomainObjectIDs.Order1);
      _dataContainer2 = DataContainerObjectMother.Create (DomainObjectIDs.Order2);
      _collector = new DataContainersPendingRegistrationCollector();
    }

    [Test]
    public void AddDataContainers_AddsObjectsToList ()
    {
      _collector.AddDataContainers (new[] { _dataContainer1, _dataContainer2 });

      Assert.That (_collector.DataContainersPendingRegistration, Is.EquivalentTo (new[] { _dataContainer1, _dataContainer2 }));
    }

    [Test]
    public void AddDataContainers_MultipleTimes_ObjectsAreAddedToList ()
    {
      _collector.AddDataContainers (new[] { _dataContainer1 });
      _collector.AddDataContainers (new[] { _dataContainer2 });

      Assert.That (_collector.DataContainersPendingRegistration, Is.EquivalentTo (new[] { _dataContainer1, _dataContainer2 }));
    }

    [Test]
    public void AddDataContainers_MultipleTimes_SameObject_IsOnlyAddedOnce ()
    {
      _collector.AddDataContainers (new[] { _dataContainer1 });
      _collector.AddDataContainers (new[] { _dataContainer1, _dataContainer2 });

      Assert.That (_collector.DataContainersPendingRegistration, Is.EquivalentTo (new[] { _dataContainer1, _dataContainer2 }));
    }

    [Test]
    public void AddDataContainers_MultipleTimes_DifferentObjectWithSameObjectID_FirstObjectWins ()
    {
      var alternativeDataContainer = DataContainerObjectMother.Create (_dataContainer1.ID);

      _collector.AddDataContainers (new[] { _dataContainer1 });
      _collector.AddDataContainers (new[] { alternativeDataContainer });

      Assert.That (_collector.DataContainersPendingRegistration, Is.EquivalentTo (new[] { _dataContainer1 }));
    }
  }
}