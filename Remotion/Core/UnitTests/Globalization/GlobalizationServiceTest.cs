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
using Remotion.Reflection;
using Remotion.UnitTests.Globalization.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class GlobalizationServiceTest
  {
    private GlobalizationService _globalizationService;
    
    [SetUp]
    public void SetUp ()
    {
      _globalizationService = new GlobalizationService();
    }

    [Test]
    public void GetResourceManager_WithTypeNotSupportingConversionFromITypeInformationToType ()
    {
      var typeInformation = MockRepository.GenerateStub<ITypeInformation>();
      
      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf(typeof(NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_TypeWithResourceForType ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithMultiLingualResourcesAttributes));

      var result = _globalizationService.GetResourceManager (typeInformation) as ResourceManagerSet;
      Assert.That (result, Is.Not.Null);
      Assert.That (result.ResourceManagers.Count(), Is.EqualTo (3));
    }

    [Test]
    public void GetResourceManager_TypeWithoutResourceForType ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes));

      var result = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (result, Is.TypeOf(typeof(NullResourceManager)));
    }

    [Test]
    public void GetResourceManager_ClassWithRealResource ()
    {
      var typeInformation = TypeAdapter.Create (typeof (ClassWithResources));

      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (resourceManager.GetString ("property:Value1"), Is.EqualTo ("Value 1"));
    }

   
 
  }
}