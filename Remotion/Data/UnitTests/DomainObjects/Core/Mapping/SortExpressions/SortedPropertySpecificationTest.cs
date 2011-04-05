// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions
{
  [TestFixture]
  public class SortedPropertySpecificationTest : StandardMappingTest
  {
    private ClassDefinition _orderItemClassDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;

    private ClassDefinition _customerClassDefinition;
    private PropertyDefinition _customerSincePropertyDefinition;
    private PropertyDefinition _customerTypePropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();
      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _productPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Position");

      _customerClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Customer));
      _customerSincePropertyDefinition = _customerClassDefinition.GetMandatoryPropertyDefinition (typeof (Customer).FullName + ".CustomerSince");
      _customerTypePropertyDefinition = _customerClassDefinition.GetMandatoryPropertyDefinition (typeof (Customer).FullName + ".Type");
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Cannot sort by property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty' - its property type "
        + "('Byte[]') does not implement IComparable.")]
    public void Initialization_NoIComparableType ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      var propertyDefinition = classDefinition.GetPropertyDefinition (typeof (ClassWithAllDataTypes).FullName + ".BinaryProperty");
      
      new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
    }

    [Test]
    public void Initialization_IComparableType_Nullable ()
    {
      Assert.That (Nullable.GetUnderlyingType (_customerSincePropertyDefinition.PropertyType), Is.Not.Null);
      new SortedPropertySpecification (_customerSincePropertyDefinition, SortOrder.Ascending);
    }

    [Test]
    public void Initialization_NotResolvedType ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      var fakePropertyDefinition = MockRepository.GenerateStub<PropertyDefinition> (classDefinition, "BinaryProperty", null, StorageClass.Persistent);
      Assert.That (fakePropertyDefinition.IsPropertyTypeResolved, Is.False);

      var result = new SortedPropertySpecification (fakePropertyDefinition, SortOrder.Ascending);

      Assert.That (result.PropertyDefinition, Is.SameAs (fakePropertyDefinition));
    }

    [Test]
    public new void ToString ()
    {
      var specificationAsc = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specificationDesc = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Descending);

      Assert.That (specificationAsc.ToString (), Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product ASC"));
      Assert.That (specificationDesc.ToString (), Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product DESC"));
    }

    [Test]
    public void Equals ()
    {
      var specification1 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specification3 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Descending);
      var specification4 = new SortedPropertySpecification (_positionPropertyDefinition, SortOrder.Ascending);

      Assert.That (specification1, Is.Not.EqualTo (null));
      Assert.That (specification1, Is.EqualTo (specification2));
      Assert.That (specification1, Is.Not.EqualTo (specification3));
      Assert.That (specification1, Is.Not.EqualTo (specification4));
    }

    [Test]
    public new void GetHashCode ()
    {
      var specification1 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (_productPropertyDefinition, SortOrder.Ascending);

      Assert.That (specification1.GetHashCode(), Is.EqualTo (specification2.GetHashCode()));
    }

    [Test]
    public void Serialization ()
    {
      var specification = new SortedPropertySpecification (_customerTypePropertyDefinition, SortOrder.Ascending);

      var result = Serializer.SerializeAndDeserialize (specification);

      Assert.That (result.PropertyDefinition, Is.EqualTo (_customerTypePropertyDefinition));
      Assert.That (result.Order, Is.EqualTo (SortOrder.Ascending));
    }
  }
}