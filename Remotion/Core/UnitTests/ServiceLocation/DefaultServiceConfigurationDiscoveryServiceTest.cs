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
using System.Runtime.InteropServices;
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
    private DefaultServiceConfigurationDiscoveryService _defaultServiceConfigurationDiscoveryService;
    private ITypeDiscoveryService _typeDiscoveryServiceStub;

    [SetUp]
    public void SetUp ()
    {
      _typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      _defaultServiceConfigurationDiscoveryService = new DefaultServiceConfigurationDiscoveryService (_typeDiscoveryServiceStub);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestSingletonConcreteImplementationAttributeType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestSingletonConcreteImplementationAttributeType), true))
          .Return (new ArrayList { typeof (TestConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single ();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos, 
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithMultipleConcreteImplementationAttributes ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesType), true))
          .Return (
              new ArrayList
              {
                  typeof (TestMultipleConcreteImplementationAttributesType1),
                  typeof (TestMultipleConcreteImplementationAttributesType2),
                  typeof (TestMultipleConcreteImplementationAttributesType3)
              });

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray ();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single ();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestMultipleConcreteImplementationAttributesType)));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos,
          Is.EqualTo (
              new[]
              {
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType2), LifetimeKind.Instance),
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType3), LifetimeKind.Instance),
                  new ServiceImplementationInfo (typeof (TestMultipleConcreteImplementationAttributesType1), LifetimeKind.Singleton),
              }));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithTypeWithDuplicatePosition ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType), true))
          .Return (new ArrayList { typeof (TestMultipleConcreteImplementationAttributesWithDuplicatePositionType1), typeof (TestMultipleConcreteImplementationAttributesWithDuplicatePositionType2) });

      Assert.That (
          () => _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType'. "
              + "Ambiguous ConcreteImplementationAttribute: Position must be unique.")
              .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithTypeWithDuplicateImplementation ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType), true))
          .Return (new ArrayList { typeof (TestMultipleConcreteImplementationAttributesWithDuplicateImplementationType) });

      Assert.That (
          () => _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray (),
          Throws.InvalidOperationException
            .With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType'. "
              + "Ambiguous ConcreteImplementationAttribute: Implementation type must be unique.")
            .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_WithNoImplementations ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (null, false)).IgnoreArguments().Return (new Type[0]);

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeof (ICollection));

      Assert.That (serviceConfigurationEntries, Is.Not.Null);
      Assert.That (serviceConfigurationEntries.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void GetDefaultConfiguration_Types ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (typeof (ITestSingletonConcreteImplementationAttributeType), true))
        .Return (new [] { typeof (TestConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) }).ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var entry = serviceConfigurationEntries.Single();
      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));
      Assert.That (
          entry.ImplementationInfos,
          Is.EqualTo (new[] { new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton) }));
    }

    [Test]
    public void GetDefaultConfiguration_Types_Unresolvable ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (typeof (ITestConcreteImplementationAttributeWithUnresolvableImplementationType), true))
        .Return (new Type[0]);
      
      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestConcreteImplementationAttributeWithUnresolvableImplementationType) }).ToArray();

      Assert.That (serviceConfigurationEntries, Is.Empty);
    }

    [Test]
    public void GetDefaultConfiguration_Assembly ()
    {
      var defaultServiceConfigurationDiscoveryService = DefaultServiceConfigurationDiscoveryService.Create();

      // Because the TestDomain contains test classes with ambiguous attributes, we expect an exception here.
      Assert.That (
          () => defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { GetType().Assembly }).ToArray(), 
            Throws.InvalidOperationException.With.Message.Contains ("Ambiguous"));
    }
  }
}