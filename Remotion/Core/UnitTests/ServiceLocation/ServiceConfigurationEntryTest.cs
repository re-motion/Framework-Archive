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

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class ServiceConfigurationEntryTest
  {
    [Test]
    public void Initialize ()
    {
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);

      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void CreateFromAttribute ()
    {
      var attribute = new ConcreteImplementationAttribute (
          "Remotion.UnitTests.ServiceLocation.TestDomain.TestConcreteImplementationAttributeType, Remotion.UnitTests, Version = <version>")
                      { Lifetime = LifetimeKind.Singleton };

      var serviceConfigurationEntry = ServiceConfigurationEntry.CreateFromAttribute (
          typeof (ITestSingletonConcreteImplementationAttributeType),
          attribute);

      Assert.That (serviceConfigurationEntry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }
  }
}