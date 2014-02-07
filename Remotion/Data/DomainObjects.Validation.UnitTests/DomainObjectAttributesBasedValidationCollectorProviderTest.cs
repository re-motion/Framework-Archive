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
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class DomainObjectAttributesBasedValidationCollectorProviderTest
  {
    [Test]
    public void CreatePropertyRuleReflector ()
    {
      var provider = new TestableDomainObjectAttributesBasedValidationCollectorProvider ();
      var result = provider.CreatePropertyRuleReflector (typeof (Customer).GetProperty ("Name"));

      Assert.That (result, Is.TypeOf (typeof (DomainObjectAttributesBasedValidationPropertyRuleReflector)));
    }

    [Test]
    public void GetValidationCollectorsForMixinProperties ()
    {
      var provider = new DomainObjectAttributesBasedValidationCollectorProvider ();
      var result = provider.GetValidationCollectors (new[] { typeof (MixinTypeWithDomainObjectAttributes) }).SelectMany (c => c).SingleOrDefault ();

      Assert.That (result, Is.Not.Null);
      Assert.That (result.Collector.ValidatedType, Is.EqualTo (typeof (MixinTypeWithDomainObjectAttributes)));
      Assert.That (result.Collector.AddedPropertyRules.Count, Is.EqualTo (6));

      var addingComponentPropertyRule = GetPropertyRule (result, "PropertyWithoutAttribute");
      Assert.That (addingComponentPropertyRule, Is.EqualTo (4));

    }

    private static IAddingComponentPropertyRule GetPropertyRule (ValidationCollectorInfo result, string propertyName)
    {
      return result.Collector.AddedPropertyRules.Single (r=>r.Property.Name == propertyName);
    }
  }
}