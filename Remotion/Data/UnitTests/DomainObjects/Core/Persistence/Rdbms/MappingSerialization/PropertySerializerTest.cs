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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingSerialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.MappingSerialization
{
  [TestFixture]
  public class PropertySerializerTest : StandardMappingTest
  {
    private PropertySerializer _propertySerializer;
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private IColumnSerializer _columnSerializerStub;
    private EnumTypeCollection _enumTypeCollection;

    public override void SetUp ()
    {
      base.SetUp();
      _columnSerializerStub = MockRepository.GenerateStub<IColumnSerializer>();
      _propertySerializer = new PropertySerializer(_columnSerializerStub);
      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _enumTypeCollection = new EnumTypeCollection();
    }

    [Test]
    public void Serialize_SerializesName ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (Computer))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (actual.Attribute ("name").Value, Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber"));
    }
    
    [Test]
    public void Serialize_SerializesDisplayName ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (Computer))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("displayName"));
      Assert.That (actual.Attribute ("displayName").Value, Is.EqualTo ("SerialNumber"));
    }
     
    [Test]
    public void Serialize_SerializesType_SimpleType ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (Computer))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.SerialNumber");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("type"));
      Assert.That (actual.Attribute ("type").Value, Is.EqualTo ("System.String"));
    }

    [Test]
    public void Serialize_SerializesType_DomainObjectProperty ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (Computer))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("type"));
      Assert.That (actual.Attribute ("type").Value, Is.EqualTo ("Remotion.Data.UnitTests::DomainObjects.TestDomain.Employee"));
    }

    [Test]
    public void Serialize_SerializesType_EnumProperty ()
    {
      
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("type"));
      Assert.That (actual.Attribute ("type").Value, Is.EqualTo ("Remotion.Data.UnitTests::DomainObjects.TestDomain.EnumType"));
    }

    [Test]
    public void Serialize_SerializesType_ExtensibleEnumProperty ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("type"));
      Assert.That (actual.Attribute ("type").Value, Is.EqualTo ("Remotion.Data.UnitTests::DomainObjects.TestDomain.Color"));
    }

    [Test]
    public void Serialize_AddsIsNullableAttribute ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaByteProperty");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("isNullable"));
      Assert.That (actual.Attribute ("isNullable").Value, Is.EqualTo ("true"));
    }

    [Test]
    public void Serialize_AddsIsNullableAttributeToNotNullableType ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ByteProperty");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("isNullable"));
      Assert.That (actual.Attribute ("isNullable").Value, Is.EqualTo ("false"));
    }

    [Test]
    public void Serialize_SerializesType_NullableProperty ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NaDateProperty");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("type"));
      Assert.That (actual.Attribute ("type").Value, Is.EqualTo ("System.DateTime"));
    }
  
    [Test]
    public void Serialize_AddsMaxLengthAttribute ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("maxLength"));
      Assert.That (actual.Attribute ("maxLength").Value, Is.EqualTo ("100"));
    }

    [Test]
    public void Serialize_StringPropertyWithoutMaxLengthConstraint ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength");

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);
      Assert.That (actual.Attributes().Select (a => a.Name.LocalName).Contains("maxLength"), Is.False);
    }

    [Test]
    public void Serialize_AddsColumnElements ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty");

      _columnSerializerStub.Stub (s => s.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub))
          .Return (new[] { new XElement ("column1"), new XElement ("column2") });

      var actual = _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (actual.Elements().Count(), Is.EqualTo (2));
      Assert.That (actual.Elements().ElementAt (0).Name.LocalName, Is.EqualTo ("column1"));
      Assert.That (actual.Elements().ElementAt (1).Name.LocalName, Is.EqualTo ("column2"));
    }

    [Test]
    public void Serialize_CollectsEnumTypes ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.EnumProperty");

      _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);
      Assert.That (_enumTypeCollection, Contains.Item (typeof (ClassWithAllDataTypes.EnumType)));
    }

    [Test]
    public void Serialize_CollectsExtensibleEnumProperty ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty");
      
      _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);
      Assert.That (_enumTypeCollection, Contains.Item (typeof (Color)));
    }

    [Test]
    public void Serialize_DoesNotCollectDuplicateEnumTypes ()
    {
      var sampleProperty = GetProperty ((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty);

      _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);
      _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);

      Assert.That (_enumTypeCollection.Count (t => t == typeof (Color)), Is.EqualTo (1));
    }

    [Test]
    public void Serialize_DoesNotCollectNonEnumTypes ()
    {
      var sampleProperty =
          MappingConfiguration.Current.GetTypeDefinition (typeof (ClassWithAllDataTypes))
              .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.StringProperty");
      
      _propertySerializer.Serialize (sampleProperty, _rdbmsPersistenceModelProviderStub, _enumTypeCollection);
      Assert.That (_enumTypeCollection, Is.Empty);
    }

    private PropertyDefinition GetProperty<TSourceObject, TPropertyType> (Expression<Func<TSourceObject, TPropertyType>> expression)
      where TSourceObject : DomainObject
    {
      var propertyInfo = MemberInfoFromExpressionUtility.GetProperty (expression);
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (TSourceObject));
      return classDefinition.ResolveProperty (PropertyInfoAdapter.Create (propertyInfo));
    }
  }
}