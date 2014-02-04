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
using System.ComponentModel.Design;
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Attributes;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DiscoveryServiceBasedTypeCollectorReflectorTest
  {
    private ITypeDiscoveryService _typeDescoveryServiceStub;

    [SetUp]
    public void SetUp ()
    {
      _typeDescoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
    }

    [Test]
    public void GetComponentValidationCollectors_WithFakeTypeDiscoveryService ()
    {
      var appliedWithAttributeTypes = new[]
                                      {
                                          typeof (CustomerMixinTargetValidationCollector1), typeof (CustomerMixinIntroducedValidationCollector1),
                                          typeof (CustomerMixinIntroducedValidationCollector2),
                                          typeof (IPersonValidationCollector1), typeof (IPersonValidationCollector2),
                                          typeof (PersonValidationCollector1),
                                          typeof (CustomerValidationCollector1), typeof (CustomerValidationCollector2)
                                      };
      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (appliedWithAttributeTypes);

      var typeCollectorProvider = new DiscoveryServiceBasedTypeCollectorReflector (_typeDescoveryServiceStub);

      Assert.That (typeCollectorProvider.GetCollectorsForType (typeof (IPerson)), Is.EqualTo (new[] { typeof (IPersonValidationCollector1) }));

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (Person)),
          Is.EquivalentTo (new[] { typeof (IPersonValidationCollector2), typeof (PersonValidationCollector1) })); //ApplyWithClass(typeof(Person))!

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (Customer)),
          Is.EqualTo (new[] { typeof (CustomerValidationCollector1), typeof (CustomerValidationCollector2) }));

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (ICustomerIntroduced)),
          Is.EqualTo (new[] { typeof (CustomerMixinIntroducedValidationCollector2) }));

      Assert.That (
          typeCollectorProvider.GetCollectorsForType (typeof (CustomerMixin)),
          Is.EquivalentTo (new[] { typeof (CustomerMixinTargetValidationCollector1), typeof (CustomerMixinIntroducedValidationCollector1) }));
      //ApplyWithMixin attribute!
    }

    [Test]
    public void GetComponentValidationCollectors_InvalidCollectorWithoutGenericArgument ()
    {
      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (new[] { typeof (Person) });

      Assert.That (
          () => new DiscoveryServiceBasedTypeCollectorReflector (_typeDescoveryServiceStub),
          Throws.InvalidOperationException.And.Message.EqualTo (
              "Type 'Remotion.Validation.UnitTests.TestDomain.Person' has no generic arguments."));
    }

    [Test]
    public void GetComponentValidationCollectors_AbstractAndInterfaceAndOpenGenericCollectorsAndProgrammaticallyCollectorsAreFiltered ()
    {
      var programmaticallyCollectorType = TypeUtility.CreateDynamicTypeWithCustomAttribute (
          typeof (AttributeBasedValidationCollectorProviderBase.AttributeValidationCollector<>).MakeGenericType (typeof (Address)),
          "Remotion.Validation.UnitTests.DynamicInvalidCollector1",
          typeof (ApplyProgrammaticallyAttribute),
          new Type[0],
          new object[0]);

      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true))
          .Return (
              new[]
              {
                  typeof (IPerson),
                  typeof (ComponentValidationCollector<>),
                  typeof (AttributeBasedValidationCollectorProviderBase.AttributeValidationCollector<>),
                  programmaticallyCollectorType
              });

      var typeCollectorProvider = new DiscoveryServiceBasedTypeCollectorReflector (_typeDescoveryServiceStub);

      var result =
          typeCollectorProvider.GetCollectorsForType (typeof (Person))
              .Concat (typeCollectorProvider.GetCollectorsForType (typeof (Person)))
              .ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetComponentValidationCollectors_GenericTypeNotAssignableFromClassType ()
    {
      var collectorType = TypeUtility.CreateDynamicTypeWithCustomAttribute (
          typeof (AttributeBasedValidationCollectorProviderBase.AttributeValidationCollector<>).MakeGenericType (typeof (Address)),
          "Remotion.Validation.UnitTests.DynamicInvalidCollector2",
          typeof (ApplyWithClassAttribute),
          new[] { typeof (Type) },
          new[] { typeof (IPerson) });

      _typeDescoveryServiceStub.Stub (stub => stub.GetTypes (typeof (IComponentValidationCollector), true)).Return (new[] { collectorType });

      Assert.That (
          () => new DiscoveryServiceBasedTypeCollectorReflector (_typeDescoveryServiceStub),
          Throws.TypeOf<InvalidOperationException>().And.Message.EqualTo (
              "Invalid 'ApplyWithClassAttribute'-definition for collector 'Remotion.Validation.UnitTests.DynamicInvalidCollector2': "
              + "type 'Remotion.Validation.UnitTests.TestDomain.Address' "
              + "is not assignable from 'Remotion.Validation.UnitTests.TestDomain.IPerson'."));
    }

    [Test]
    public void GetComponentValidationCollectors_WithRemotionDiscoveryService ()
    {
      var typeCollectorProvider = new DiscoveryServiceBasedTypeCollectorReflector();

      var result = typeCollectorProvider.GetCollectorsForType (typeof (Person)).ToArray();

      Assert.That (result.Count(), Is.EqualTo (2));
      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              {
                  typeof (IPersonValidationCollector2), typeof (PersonValidationCollector1)
              }));
    }
  }
}