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
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.Implementation.TestDomain;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class InvolvedTypeProviderTest
  {
    private InvolvedTypeProvider _inheritanceAndMxinBasedStrategy;

    [SetUp]
    public void SetUp ()
    {
      _inheritanceAndMxinBasedStrategy = new InvolvedTypeProvider (col => col.OrderBy (t => t.Name), LoadFilteredValidationTypeFilter.Instance);
    }

    [Test]
    public void GetAffectedType_NoBaseTypesAndNoInterfacesAndMixins ()
    {
      var result = _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithoutBaseType)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof (TypeWithoutBaseType) }));
    }

    [Test]
    public void GetAffectedType_WithBaseTypesAndNoInterfacesAndMixins ()
    {
      var result = _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithSeveralBaseTypes)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof (BaseType2), typeof (BaseType1), typeof (TypeWithSeveralBaseTypes) }));
    }

    [Test]
    public void GetAffectedType_WithOneInterfacesAndNoBaseTypesAndMixins ()
    {
      var result = _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithOneInterface)).SelectMany (t => t).ToList();

      Assert.That (result, Is.EqualTo (new[] { typeof (ITypeWithOneInterface), typeof (TypeWithOneInterface) }));
    }

    [Test]
    public void GetAffectedType_WithSeveralInterfacesAndNoBaseTypesAndMixins ()
    {
      var result = _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithSeveralInterfaces)).SelectMany (t => t).ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfaces2), typeof (ITypeWithSeveralInterfaces3),
                  typeof (TypeWithSeveralInterfaces)
              }));
    }

    [Test]
    public void GetAffectedType_WithSeveralInterfacesHavingBaseInterfacesAndNoBaseTypesAndMixins ()
    {
      var result =
          _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithSeveralInterfacesImplementingInterface)).SelectMany (t => t)
                                          .ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (IBaseBaseInterface1), typeof (IBaseInterface1), typeof (IBaseInterface2), typeof (IBaseInterface3),
                  typeof (ITypeWithSeveralInterfacesImplementingInterface1), typeof (ITypeWithSeveralInterfacesImplementingInterface2),
                  typeof (ITypeWithSeveralInterfacesImplementingInterface3), typeof (TypeWithSeveralInterfacesImplementingInterface)
              }));
    }

    [Test]
    public void GetAffectedType_WithTwoInterfacesHavingCommonBaseInterface ()
    {
      var result =
          _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithTwoInterfacesHavingCommingBaseInterface)).SelectMany (t => t)
                                          .ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ICommonBaseBaseInterface), typeof (ITypeWithTwoInterfacesBaseInterface1),
                  typeof (ITypeWithTwoInterfacesBaseInterface2), typeof (TypeWithTwoInterfacesHavingCommingBaseInterface)
              }));
    }

    [Test]
    public void GetAffectedType_WithBaseTypeAndSeveralInterfacesAndNoMixins ()
    {
      var result =
          _inheritanceAndMxinBasedStrategy.GetTypes (typeof (TypeWithSeveralInterfacesAndBaseType)).SelectMany (t => t).ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfaces2), typeof (ITypeWithSeveralInterfaces3),
                  typeof (TypeWithSeveralInterfaces), typeof (ITypeWithSeveralInterfacesAndBaseTypes1),
                  typeof (ITypeWithSeveralInterfacesAndBaseTypes2),
                  typeof (ITypeWithSeveralInterfacesAndBaseTypes3), typeof (TypeWithSeveralInterfacesAndBaseType)
              }));
    }

    [Test]
    [Ignore ("Include reimplemented interfaces (with Mono.Cecil ??)")]
    public void GetAffectedType_WithBaseTypeAndSeveralInterfacesReImplementingInterfaceAndNoMixins ()
    {
      var result =
          _inheritanceAndMxinBasedStrategy.GetTypes (
              typeof (TypeWithSeveralInterfacesAndBaseTypeReImplementingInterface)).SelectMany (t => t).ToList();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfaces2), typeof (ITypeWithSeveralInterfaces3),
                  typeof (TypeWithSeveralInterfaces), typeof (ITypeWithServeralInterfaces1), typeof (ITypeWithSeveralInterfacesAndBaseTypes1),
                  typeof (ITypeWithSeveralInterfacesAndBaseTypes2), typeof (ITypeWithSeveralInterfacesAndBaseTypes3),
                  typeof (TypeWithSeveralInterfacesAndBaseTypeReImplementingInterface)
              }));
    }

    [Test]
    public void GetAffectedType_WithMixinHierarchy ()
    {
      var result = _inheritanceAndMxinBasedStrategy.GetTypes (typeof (DerivedConcreteTypeForMixin)).SelectMany (t => t).ToList();

      Assert.That (result[0], Is.EqualTo (typeof (IBaseConcreteTypeForMixin)));
      Assert.That (result[1], Is.EqualTo (typeof (BaseConcreteTypeForMixin)));
      Assert.That (result[2], Is.EqualTo (typeof (IDerivedConcreteTypeForMixin)));
      Assert.That (result[3], Is.EqualTo (typeof (DerivedConcreteTypeForMixin)));
      Assert.That (result[4], Is.EqualTo (typeof (IBaseBaseIntroducedFromMixinForDerivedType1)));
      Assert.That (result[5], Is.EqualTo (typeof (IBaseIntroducedFromMixinForDerivedTypeA1)));
      Assert.That (result[6], Is.EqualTo (typeof (IBaseIntroducedFromMixinForDerivedTypeB1)));
      Assert.That (result[7], Is.EqualTo (typeof (IIntroducedFromMixinForBaseType)));
      Assert.That (result[8], Is.EqualTo (typeof (IIntroducedFromMixinForDerivedType1)));
      Assert.That (result[9], Is.EqualTo (typeof (IIntroducedFromMixinForDerivedType2)));
      Assert.That (result[10].Name.StartsWith("DerivedConcreteTypeForMixin_Proxy_"), Is.True);
      Assert.That (result[11], Is.EqualTo (typeof (BaseMixinForDerivedType)));
      Assert.That (result[12], Is.EqualTo (typeof (MixinForBaseType)));
      Assert.That (result[13], Is.EqualTo (typeof (MixinForDerivedType1)));
      Assert.That (result[14], Is.EqualTo (typeof (MixinForDerivedType2)));
    }
  }
}