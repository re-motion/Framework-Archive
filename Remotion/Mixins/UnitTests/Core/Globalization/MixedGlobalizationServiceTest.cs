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
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Remotion.Mixins.UnitTests.Core.Globalization.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;
using System.Linq;


namespace Remotion.Mixins.UnitTests.Core.Globalization
{
  [TestFixture]
  public class MixedGlobalizationServiceTest
  {
    private MixedGlobalizationService _globalizationService;

    [SetUp]
    public void SetUp ()
    {
        _globalizationService = new MixedGlobalizationService();
    }

    [Test]
    public void GetResourceManager_WithTypeNotSupportingConversionFromITypeInformationToType ()
    {
      var typeInformation = MockRepository.GenerateStub<ITypeInformation> ();

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf(typeof(NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithResourceForType ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithMultiLingualResourcesAttributes));

      var result = _globalizationService.GetResourceManager (typeInformation) as ResourceManagerSet;

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result.ElementAt (0), Is.TypeOf (typeof (ResourceManagerWrapper)));
    }

    [Test]
    public void GetResourceManager_TypeWithoutResourceForType ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf(typeof(NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithoutResourceForType_ResourceAddedByMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = _globalizationService.GetResourceManager (typeInformation) as ResourceManagerSet;

        Assert.That (result, Is.Not.Null);
        Assert.That (result.Count, Is.EqualTo (1));
        Assert.That (result.ElementAt(0), Is.TypeOf(typeof(ResourceManagerWrapper)));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithoutResourceForType_ResourceAddedByTwoMixins ()
    {
      using (MixinConfiguration.BuildFromActive ()
        .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
        .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
        .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
        .EnterScope ())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = _globalizationService.GetResourceManager (typeInformation) as ResourceManagerSet;

        Assert.That (result, Is.Not.Null);
        Assert.That (result.Count, Is.EqualTo (3));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithResourceAndMixinWithResource_TypeResourceOverridesMixinResource ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassWithResources>().AddMixin<MixinAddingResources>().EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithResources));
        var resourceManager = _globalizationService.GetResourceManager (typeInformation);

        Assert.That (resourceManager.GetString ("property:Value1"), Is.EqualTo ("Value 1"));
      }
    }

  }
}