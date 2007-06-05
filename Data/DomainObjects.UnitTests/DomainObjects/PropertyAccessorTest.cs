using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorTest : ClientTransactionBaseTest
  {
    private static PropertyAccessor CreateAccessor (DomainObject domainObject, string propertyIdentifier)
    {
      ConstructorInfo ctor =
          typeof (PropertyAccessor).GetConstructor (
              BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof (DomainObject), typeof (string)}, null);
      try
      {
        return
            (PropertyAccessor)
            ctor.Invoke (BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {domainObject, propertyIdentifier}, null);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void ManualPropertyAccessor ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();

      Company company = Company.NewObject ();
      company.IndustrialSector = sector;
      Assert.AreSame (sector, company.IndustrialSector, "related object");

      Assert.IsTrue (sector.Companies.ContainsObject (company), "related objects");

      sector.Name = "Foo";
      Assert.AreEqual ("Foo", sector.Name, "property value");
    }

    [Test]
    public void GetPropertyObjects()
    {
      CheckPropertyObjects(typeof (IndustrialSector), "Name",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.A);
            Assert.AreSame (t.C, t.A.ClassDefinition);
            Assert.AreEqual (t.D, t.A.PropertyName);
            Assert.IsNull (t.B);
          });

      CheckPropertyObjects (typeof (IndustrialSector), "Companies",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Company), "IndustrialSector",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Employee), "Computer",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Computer), "Employee",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });

      CheckPropertyObjects (typeof (Client), "ParentClient",
          delegate (Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> t)
          {
            Assert.IsNotNull (t.B);
            Assert.AreSame (t.C, t.B.ClassDefinition);
            Assert.AreEqual (t.D, t.B.PropertyName);
          });
    }

    private static void CheckPropertyObjects (Type type, string shortPropertyName,
        Action<Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string>> checker)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
      string identifier = type.FullName + "." + shortPropertyName;

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects =
          PropertyAccessor.GetPropertyDefinitionObjects (classDefinition, identifier);

      checker (new Tuple<PropertyDefinition, IRelationEndPointDefinition, ClassDefinition, string> (propertyObjects.A, propertyObjects.B,
          classDefinition, identifier));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void GetPropertyObjectsThrowsOnInvalidPropertyID1()
    {
      PropertyAccessor.GetPropertyDefinitionObjects (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Bla");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void GetPropertyObjectsThrowsOnInvalidPropertyID2 ()
    {
      PropertyAccessor.GetPropertyDefinitionObjects (
          MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");
    }

    [Test]
    public void PropertyAccessorKindStatic ()
    {
      Assert.AreEqual (PropertyKind.PropertyValue,
          PropertyAccessor.GetPropertyKind(MappingConfiguration.Current.ClassDefinitions[typeof(IndustrialSector)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          "Property value type");

      Assert.AreEqual (PropertyKind.RelatedObjectCollection,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind(MappingConfiguration.Current.ClassDefinitions[typeof(Company)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Employee)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (PropertyKind.RelatedObject,
          PropertyAccessor.GetPropertyKind (MappingConfiguration.Current.ClassDefinitions[typeof (Client)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyAccessorKindInstance ()
    {
      IndustrialSector sector = IndustrialSector.NewObject();
      Assert.AreEqual (PropertyKind.PropertyValue,
          CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").Kind, "Property value type");

      Assert.AreEqual (PropertyKind.RelatedObjectCollection,
          CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies").Kind,
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Company company = Company.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (company, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector").Kind,
          "Related object type - bidirectional relation 1:n, n side");

      Employee employee = Employee.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (employee, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer").Kind,
          "Related object type - bidirectional relation 1:1, referenced side");

      Computer computer = Computer.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (computer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee").Kind,
          "Related object type - bidirectional relation 1:1, foreign key side");

      Client client = Client.NewObject ();
      Assert.AreEqual (PropertyKind.RelatedObject,
          CreateAccessor (client, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient").Kind,
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyTypeStatic()
    {
      Assert.AreEqual (typeof (string),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(IndustrialSector)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          "Property value type");

      Assert.AreEqual (typeof (ObjectList<Company>),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(IndustrialSector)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (typeof (IndustrialSector),
          PropertyAccessor.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Company)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (typeof (Computer),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(Employee)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (typeof (Employee),
          PropertyAccessor.GetPropertyType (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (typeof (Client),
          PropertyAccessor.GetPropertyType(MappingConfiguration.Current.ClassDefinitions[typeof(Client)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyTypeInstance ()
    {
      Assert.AreEqual (typeof (string),
          CreateAccessor (IndustrialSector.NewObject(), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").PropertyType,
          "Property value type");

      Assert.AreEqual (typeof (ObjectList<Company>),
          CreateAccessor (IndustrialSector.NewObject (), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies").PropertyType,
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.AreEqual (typeof (IndustrialSector),
          CreateAccessor (Company.NewObject (), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector").PropertyType,
          "Related object type - bidirectional relation 1:n, n side");

      Assert.AreEqual (typeof (Computer),
          CreateAccessor (Employee.NewObject(), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer").PropertyType,
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.AreEqual (typeof (Employee),
          CreateAccessor (Computer.NewObject (),  "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee").PropertyType,
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.AreEqual (typeof (Client),
          CreateAccessor (Client.NewObject (),  "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient").PropertyType,
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'.")]
    public void PropertyAccessorGetThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").GetValue<int>();
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = "Actual type 'System.String' of property "
        + "'Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'.")]
    public void PropertyAccessorSetThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name").SetValue (5);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "does not have a mapping property named", MatchType = MessageMatch.Contains)]
    public void PropertyAccessorThrowsIfWrongIdentifier ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.FooBarFredBaz");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Related object collections cannot be set.",
          MatchType = MessageMatch.Contains)]
    public void PropertyAccessorThrowsIfSettingObjectList ()
    {
      IndustrialSector sector = IndustrialSector.NewObject ();
      CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies").SetValue (new ObjectList<Company> ());
    }

    [Test]
    public void IsValidProperty ()
    {
      Assert.IsFalse (PropertyAccessor.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], "Bla"));
      Assert.IsFalse (PropertyAccessor.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], "Companies"));
      Assert.IsTrue (PropertyAccessor.IsValidProperty (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)],
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"));
    }

    [Test]
    public void PropertyMetadata()
    {
      PropertyAccessor accessor = CreateAccessor (IndustrialSector.NewObject(),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");
      
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)], accessor.ClassDefinition);
      Assert.AreSame ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies", accessor.PropertyIdentifier);
      Assert.IsNull (accessor.PropertyDefinition);
      Assert.IsNotNull (accessor.RelationEndPointDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
          .GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          accessor.RelationEndPointDefinition);

      accessor = CreateAccessor (IndustrialSector.NewObject(),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name");

      Assert.IsNotNull (accessor.PropertyDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
          .GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          accessor.PropertyDefinition);

      Assert.IsNull (accessor.RelationEndPointDefinition);

      accessor = CreateAccessor (Computer.NewObject (),
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee");

      Assert.IsNotNull (accessor.PropertyDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)]
          .GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          accessor.PropertyDefinition);

      Assert.IsNotNull (accessor.RelationEndPointDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions[typeof (Computer)]
          .GetRelationEndPointDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          accessor.RelationEndPointDefinition);
    }

    [Test]
    public void HasChangedAndOriginalValueSimple()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name");
      Assert.IsFalse (property.HasChanged);
      string originalValue = property.GetValue<string>();
      Assert.IsNotNull (originalValue);
      Assert.AreEqual (originalValue, property.GetOriginalValue<string> ());

      property.SetValue ("Foo");
      Assert.IsTrue (property.HasChanged);
      Assert.AreEqual ("Foo", property.GetValue<string>());
      Assert.AreNotEqual (property.GetValue<string>(), property.GetOriginalValue<string> ());
      Assert.AreEqual (originalValue, property.GetOriginalValue<string> ());
    }

    [Test]
    public void HasChangedAndOriginalValueRelated()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      PropertyAccessor property = CreateAccessor (computer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee");
      Assert.IsFalse (property.HasChanged);
      Employee originalValue = property.GetOriginalValue<Employee>();

      property.GetValue<Employee> ().Name = "FooBarBazFred";
      Assert.IsFalse (property.HasChanged);

      Employee newValue = Employee.NewObject ();
      property.SetValue (newValue);
      Assert.IsTrue (property.HasChanged);
      Assert.AreEqual (newValue, property.GetValue<Employee> ());
      Assert.AreNotEqual (property.GetValue<Employee> (), property.GetOriginalValue<Employee> ());
      Assert.AreEqual (originalValue, property.GetOriginalValue<Employee> ());

    }

    [Test]
    public void HasChangedAndOriginalValueRelatedCollection()
    {
      IndustrialSector sector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      PropertyAccessor property = CreateAccessor (sector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies");

      Assert.IsFalse (property.HasChanged);
      ObjectList<Company> originalValue = property.GetValue<ObjectList<Company>> ();
      int originalCount = originalValue.Count;
      Assert.IsNotNull (originalValue);
      Assert.AreEqual (originalValue, property.GetOriginalValue<ObjectList<Company>> ());

      property.GetValue<ObjectList<Company>> ().Add (Company.NewObject ());
      Assert.AreNotEqual (originalCount, property.GetValue<ObjectList<Company>> ().Count);
      Assert.IsTrue (property.HasChanged);

      Assert.AreSame (originalValue, property.GetValue<ObjectList<Company>> ()); // !!
      Assert.AreNotSame (property.GetValue<ObjectList<Company>> (), property.GetOriginalValue<ObjectList<Company>> ());
      Assert.AreEqual (originalCount, property.GetOriginalValue<ObjectList<Company>>().Count);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage =  "Actual type .* of property .* does not match expected type 'System.Int32'",
        MatchType = MessageMatch.Regex)]
    public void GetOriginalValueThrowsWithWrongType()
    {
      CreateAccessor (IndustrialSector.NewObject(), "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies")
          .GetOriginalValue<int>();
    }
  }
}
