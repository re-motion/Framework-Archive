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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorDataTest : StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void ConstructorThrows_OnWrongIdentifier ()
    {
      CreateAccessorData (typeof (IndustrialSector), "FooBarFredBaz");
    }

    [Test]
    public void GetPropertyObjects_StringProperty ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)];
      var propertyIdentifier = GetPropertyIdentifier (classDefinition, "Name");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      Assert.That (propertyObjects.A, Is.Not.Null);
      Assert.That (propertyObjects.A.ClassDefinition, Is.SameAs (classDefinition));
      Assert.That (propertyObjects.A.PropertyName, Is.EqualTo (propertyIdentifier));
      Assert.That (propertyObjects.B, Is.Null);
    }

    [Test]
    public void GetPropertyObjects_CollectionProperty ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)];
      var propertyIdentifier = GetPropertyIdentifier (classDefinition, "Companies");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      Assert.That (propertyObjects.B, Is.Not.Null);
      Assert.That (propertyObjects.B.ClassDefinition, Is.SameAs (classDefinition));
      Assert.That (propertyObjects.B.PropertyName, Is.EqualTo (propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_CollectionProperty_BackReference ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Company)];
      var propertyIdentifier = GetPropertyIdentifier (classDefinition, "IndustrialSector");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      Assert.That (propertyObjects.B, Is.Not.Null);
      Assert.That (propertyObjects.B.ClassDefinition, Is.SameAs (classDefinition));
      Assert.That (propertyObjects.B.PropertyName, Is.EqualTo (propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_OneOne_VirtualSide ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Employee)];
      var propertyIdentifier = GetPropertyIdentifier (classDefinition, "Computer");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      Assert.That (propertyObjects.B, Is.Not.Null);
      Assert.That (propertyObjects.B.ClassDefinition, Is.SameAs (classDefinition));
      Assert.That (propertyObjects.B.PropertyName, Is.EqualTo (propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_OneOne_RealSide ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Computer)];
      var propertyIdentifier = GetPropertyIdentifier (classDefinition, "Employee");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      Assert.That (propertyObjects.B, Is.Not.Null);
      Assert.That (propertyObjects.B.ClassDefinition, Is.SameAs (classDefinition));
      Assert.That (propertyObjects.B.PropertyName, Is.EqualTo (propertyIdentifier));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void GetPropertyObjects_ThrowsOnInvalidPropertyID1 ()
    {
      PropertyAccessorData.GetPropertyDefinitionObjects (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Bla");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void GetPropertyObjects_ThrowsOnInvalidPropertyID2 ()
    {
      PropertyAccessorData.GetPropertyDefinitionObjects (
          MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies");
    }

    [Test]
    public void InstancePropertyObjects_CollectionProperty ()
    {
      PropertyAccessorData accessor = CreateAccessorData (typeof (IndustrialSector), "Companies");

      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], accessor.ClassDefinition);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies", accessor.PropertyIdentifier);
      Assert.IsNull (accessor.PropertyDefinition);
      Assert.IsNotNull (accessor.RelationEndPointDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
          .GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"),
          accessor.RelationEndPointDefinition);
    }

    [Test]
    public void InstancePropertyObjects_StringProperty ()
    {
      PropertyAccessorData accessor = CreateAccessorData (typeof (IndustrialSector), "Name");

      Assert.IsNotNull (accessor.PropertyDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
                          .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"),
                      accessor.PropertyDefinition);
      Assert.IsNull (accessor.RelationEndPointDefinition);
    }

    [Test]
    public void InstancePropertyObjects_OneOneProperty_RealSide ()
    {
      PropertyAccessorData accessor = CreateAccessorData (typeof (Computer), "Employee");

      Assert.IsNotNull (accessor.PropertyDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)]
          .GetPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"),
          accessor.PropertyDefinition);

      Assert.IsNotNull (accessor.RelationEndPointDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)]
          .GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"),
          accessor.RelationEndPointDefinition);
    }

    [Test]
    public void GetPropertyKind ()
    {
      Assert.AreEqual (PropertyKind.PropertyValue,
          PropertyAccessorData.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"),
          "Property value type");

      Assert.AreEqual (PropertyKind.RelatedObjectCollection,
          PropertyAccessorData.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessorData.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector"),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessorData.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Employee)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer"),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessorData.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessorData.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Client)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient"),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void Kind ()
    {
      Assert.AreEqual (PropertyKind.PropertyValue,
          CreateAccessorData (typeof (IndustrialSector), "Name").Kind, "Property value type");

      Assert.AreEqual (PropertyKind.RelatedObjectCollection,
          CreateAccessorData (typeof (IndustrialSector), "Companies").Kind, "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessorData (typeof (Company), "IndustrialSector").Kind, "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessorData (typeof (Employee), "Computer").Kind, "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessorData (typeof (Computer), "Employee").Kind, "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessorData (typeof (Client), "ParentClient").Kind, "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void GetPropertyType ()
    {
      Assert.AreEqual (typeof (string),
          PropertyAccessorData.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Name"),
          "Property value type");

      Assert.AreEqual (typeof (ObjectList<Company>),
          PropertyAccessorData.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (typeof (IndustrialSector),
          PropertyAccessorData.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.IndustrialSector"),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (typeof (Computer),
          PropertyAccessorData.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Employee)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Computer"),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (typeof (Employee),
          PropertyAccessorData.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (typeof (Client),
          PropertyAccessorData.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Client)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient"),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyType ()
    {
      Assert.AreEqual (typeof (string), CreateAccessorData (typeof (IndustrialSector), "Name").PropertyType, 
          "Property value type");

      Assert.AreEqual (typeof (ObjectList<Company>), CreateAccessorData (typeof (IndustrialSector), "Companies").PropertyType,
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (typeof (IndustrialSector), CreateAccessorData (typeof (Company), "IndustrialSector").PropertyType,
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (typeof (Computer), CreateAccessorData (typeof (Employee), "Computer").PropertyType,
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (typeof (Employee), CreateAccessorData(typeof (Computer), "Employee").PropertyType,
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (typeof (Client), CreateAccessorData (typeof (Client), "ParentClient").PropertyType,
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void IsValidProperty ()
    {
      Assert.IsFalse (PropertyAccessorData.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], "Bla"));
      Assert.IsFalse (PropertyAccessorData.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], "Companies"));
      Assert.IsTrue (PropertyAccessorData.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.IndustrialSector.Companies"));
    }

    [Test]
    public void Equals_True()
    {
      var data1 = CreateAccessorData (typeof (IndustrialSector), "Name");
      var data2 = CreateAccessorData (typeof (IndustrialSector), "Name");

      Assert.That (data1, Is.EqualTo (data2));
    }

    [Test]
    public void Equals_False_ClassDefinition ()
    {
      var data1 = CreateAccessorData (typeof (IndustrialSector), "Name");
      var data2 = CreateAccessorData (typeof (Company), "Name");

      Assert.That (data1, Is.Not.EqualTo (data2));
    }

    [Test]
    public void Equals_False_PropertyIfentifier ()
    {
      var data1 = CreateAccessorData (typeof (IndustrialSector), "Name");
      var data2 = CreateAccessorData (typeof (IndustrialSector), "Companies");

      Assert.That (data1, Is.Not.EqualTo (data2));
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var data1 = CreateAccessorData (typeof (IndustrialSector), "Name");
      var data2 = CreateAccessorData (typeof (IndustrialSector), "Name");

      Assert.That (data1.GetHashCode(), Is.EqualTo (data2.GetHashCode()));
    }

    private static string GetPropertyIdentifier(ClassDefinition classDefinition, string shortPropertyName)
    {
      return classDefinition.ClassType.FullName + "." + shortPropertyName;
    }

    private PropertyAccessorData CreateAccessorData (Type type, string shortIdentifier)
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
      return new PropertyAccessorData (classDefinition, GetPropertyIdentifier (classDefinition, shortIdentifier));
    }
  }
}
