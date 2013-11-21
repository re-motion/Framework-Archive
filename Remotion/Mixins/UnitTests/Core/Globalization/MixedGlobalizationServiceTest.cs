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
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Remotion.Mixins.UnitTests.Core.Globalization.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;


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
      var typeInformation = MockRepository.GenerateStub<ITypeInformation>();

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf (typeof (NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithoutMixin_ReturnsNullResourceManager ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithResources));

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf (typeof (NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_NoResourceAttribute_ReturnsResourceManagerSetWithEmptyResourceManagerCollection ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.Any(), Is.False);
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_WithResourceAttribute_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        var innerResourceManager = result.ResourceManagers.Single();
        Assert.That (innerResourceManager, Is.InstanceOf<ResourceManagerWrapper>());
        Assert.That (innerResourceManager.Name, Is.EqualTo ("OnMixin1"));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_WithResourceAttribute_SameMixinConfiguration_ResourceManagerIsReturnedFromCache ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result1 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
        var result2 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result1, Is.SameAs (result2));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_WithResourceAttribute_DifferentMixinMasterConfiguration_CacheIsCleared ()
    {
      using (MixinConfiguration.BuildFromActive ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .EnterScope ())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result1 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
        MixinConfiguration.ResetMasterConfiguration();
        var result2 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result1, Is.Not.SameAs (result2));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixin_NewMixinMasterConfiguration_ResourceManagerSetNotReturnedFromCache ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));
      ResourceManagerSet result1;

      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope())
      {
        result1 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
      }
      MixinConfiguration.ResetMasterConfiguration();

      ResourceManagerSet result2;
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .EnterScope ())
      {
        result2 = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);
      }

      Assert.That (result1, Is.Not.SameAs (result2));
    }

    [Test]
    public void GetResourceManager_TypeWithMixinOfMixin_WithResourceAttribute_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .ForClass<MixinWithoutResourceAttribute>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        var innerResourceManager = result.ResourceManagers.Single();
        Assert.That (innerResourceManager, Is.InstanceOf<ResourceManagerWrapper>());
        Assert.That (innerResourceManager.Name, Is.EqualTo ("MixinOfMixinWithResources"));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMultipleMixins_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .ForClass<MixinWithoutResourceAttribute>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.Count(), Is.EqualTo (2));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMixinWithMultipleAttributes_ReturnsResourceManagerSet ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.Count(), Is.EqualTo (2));
      }
    }

    [Test]
    public void GetResourceManager_TypeWithMultipleMixins_ReturnsMostSpecificResourceManagerFirst ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinWithoutResourceAttribute>()
          .ForClass<MixinWithoutResourceAttribute>()
          .AddMixin<MixinOfMixinWithResources>()
          .EnterScope())
      {
        var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

        var result = (ResourceManagerSet) _globalizationService.GetResourceManager (typeInformation);

        Assert.That (result.ResourceManagers.First().Name, Is.EqualTo ("MixinOfMixinWithResources"));
      }
    }
  }
}