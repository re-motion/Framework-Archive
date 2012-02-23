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
using System.Collections;
using System.ComponentModel.Design;
using NUnit.Framework;
using Remotion.ServiceLocation;
using System.Linq;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceConfigurationDiscoveryServiceTest
  {
    [Test]
    public void GetDefaultConfiguration ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub
          .Expect (stub => stub.GetTypes (null, false))
          .Return (new[] { typeof (ITestSingletonConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub);

      Assert.That (serviceConfigurationEntries, Is.Not.Null);
      Assert.That (serviceConfigurationEntries.Count (), Is.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single ();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos, 
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    public void GetDefaultConfiguration_DiscoveryServiceReturnsArrayList ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Expect (stub => stub.GetTypes (null, false)).Return (
          new ArrayList(new[] { typeof(ITestSingletonConcreteImplementationAttributeType)}));

      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeDiscoveryServiceStub);

      Assert.That (serviceConfigurationEntries, Is.Not.Null);
      Assert.That (serviceConfigurationEntries.Count (), Is.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single ();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos,
          Is.EqualTo(new[] { new ServiceImplementationInfo(typeof(TestConcreteImplementationAttributeType), LifetimeKind.Singleton)}));
    }

    [Test]
    public void GetDefaultConfiguration_ServiceHasNoConcreteImplementationAttributeDefined ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { typeof (ICollection) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (0));
    }

    [Test]
    public void GetDefaultConfiguration_ServiceHasConcreteImplementationAttributeDefined ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) });

      Assert.That (serviceConfigurationEntries.Count(), Is.EqualTo (1));
      var resultEntry = serviceConfigurationEntries.Single();
      Assert.That (resultEntry.ServiceType, Is.EqualTo(typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          resultEntry.ImplementationInfos,
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    [Ignore ("TODO 4652: Need to support multiple attributes")]
    public void GetDefaultConfiguration_UnitTestAssembly ()
    {
      var serviceConfigurationEntries = DefaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { GetType().Assembly });

      var testDomainEntries = serviceConfigurationEntries.Where (entry => entry.ServiceType.Namespace.StartsWith (GetType().Namespace + ".TestDomain"));
      Assert.That (testDomainEntries.Count(), Is.EqualTo (10));
    }
  }
}