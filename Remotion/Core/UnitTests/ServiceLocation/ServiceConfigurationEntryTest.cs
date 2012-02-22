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
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class ServiceConfigurationEntryTest
  {
    [Test]
    public void Initialize_WithSingleValue ()
    {
      var implementationInfo = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), implementationInfo);

      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos, Is.EqualTo (new[] { implementationInfo }));
    }

    [Test]
    public void Initialize_WithEnumerable ()
    {
      var info1 = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var info2 = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);

      var entry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), new[] { info1, info2 });

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (entry.ImplementationInfos, Is.EqualTo (new[] { info1, info2 }));
    }

    [Test]
    [ExpectedException(typeof(ArgumentEmptyException), ExpectedMessage = 
      "Parameter 'implementationInfos' cannot be empty.\r\nParameter name: implementationInfos")]
    public void Initialize_WithEnumerable_Empty ()
    {
      new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), new ServiceImplementationInfo[0]);
    }

    [Test]
    public void CreateFromAttribute ()
    {
      var template = "Remotion.UnitTests.ServiceLocation.TestDomain.TestConcreteImplementationAttributeType, Remotion.UnitTests, Version = <version>";
      var attribute = new ConcreteImplementationAttribute (template) { Lifetime = LifetimeKind.Singleton };

      var entry = ServiceConfigurationEntry.CreateFromAttribute (typeof (ITestSingletonConcreteImplementationAttributeType), attribute);

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          entry.ImplementationInfos, 
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }
  }
}