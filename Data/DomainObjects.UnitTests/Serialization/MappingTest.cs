using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class MappingTest : SerializationBaseTest
  {
    [Test]
    public void PropertyDefinitionWithoutClassDefinition ()
    {
      PropertyDefinition propertyDefinition = new PropertyDefinition ("PropertyName", "ColumnName", "string", true, true, 100);

      PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);

      Assert.IsFalse (object.ReferenceEquals (propertyDefinition, deserializedPropertyDefinition));
      AreEqual (propertyDefinition, deserializedPropertyDefinition);
    }

    [Test]
    public void PropertyDefinitionWithClassDefinition ()
    {
      PropertyDefinition propertyDefinition = new PropertyDefinition ("OrderNumber", "OrderNo", "int32", false);
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order), false);
      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);

      Assert.IsFalse (object.ReferenceEquals (propertyDefinition, deserializedPropertyDefinition));
      Assert.IsFalse (object.ReferenceEquals (propertyDefinition.ClassDefinition, deserializedPropertyDefinition.ClassDefinition));
      AreEqual (propertyDefinition, deserializedPropertyDefinition);

    }

    [Test]
    public void PropertyDefinitionInMapping ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      PropertyDefinition orderNumberDefinition = orderDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"];

      PropertyDefinition deserializedOrderNumberDefinition = (PropertyDefinition) SerializeAndDeserialize (orderNumberDefinition);
      Assert.AreSame (orderNumberDefinition, deserializedOrderNumberDefinition);
    }

    [Test]
    public void PropertyDefinitionWithUnresolvedNativePropertyType ()
    {
      PropertyDefinition propertyDefinition = new PropertyDefinition ("PropertyName", "ColumnName", "int32", false, true, null);

      PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);
      AreEqual (propertyDefinition, deserializedPropertyDefinition);
    }

    [Test]
    public void PropertyDefinitionWithUnresolvedUnknownPropertyType ()
    {
      PropertyDefinition propertyDefinition = new PropertyDefinition ("PropertyName", "ColumnName", "UnknownPropertyType", false, true, null);

      PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);
      AreEqual (propertyDefinition, deserializedPropertyDefinition);
    }

    [Test]
    public void SimplePropertyDefinitionCollection ()
    {
      PropertyDefinitionCollection definitions = new PropertyDefinitionCollection ();
      definitions.Add (MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions[1]);

      PropertyDefinitionCollection deserializedDefinitions = (PropertyDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (object.ReferenceEquals (definitions, deserializedDefinitions));
      Assert.AreEqual (definitions.Count, deserializedDefinitions.Count);
      Assert.AreSame (definitions[0], deserializedDefinitions[0]);
      Assert.AreSame (definitions[1], deserializedDefinitions[1]);
    }

    [Test]
    public void RelationEndPointDefinitionWithoutRelationDefinition ()
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);
      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));
      RelationEndPointDefinition endPointdefinition = new RelationEndPointDefinition (classDefinition, "Order", true);

      RelationEndPointDefinition deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointdefinition);

      Assert.IsFalse (object.ReferenceEquals (endPointdefinition, deserializedEndPointDefinition));
      AreEqual (endPointdefinition, deserializedEndPointDefinition);
    }

    [Test]
    public void RelationEndPointDefinitionWithRelationDefinition ()
    {
      ReflectionBasedClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      ReflectionBasedClassDefinition orderTicketDefinition = new ReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);

      orderTicketDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = new VirtualRelationEndPointDefinition (
          orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

      RelationDefinition relationDefinition = new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      RelationEndPointDefinition deserializedOrderTicketEndPointdefinition = (RelationEndPointDefinition) SerializeAndDeserialize (orderTicketEndPointdefinition);

      Assert.IsFalse (object.ReferenceEquals (orderTicketEndPointdefinition, deserializedOrderTicketEndPointdefinition));
      AreEqual (orderTicketEndPointdefinition, deserializedOrderTicketEndPointdefinition);
    }

    [Test]
    public void RelationEndPointDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"];
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("OrderTicket", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      RelationEndPointDefinition deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithoutRelationDefinition ()
    {
      ClassDefinition classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);

      VirtualRelationEndPointDefinition endPointdefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "OrderTicket", true, CardinalityType.One, typeof (Order));

      VirtualRelationEndPointDefinition deserializedEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointdefinition);

      Assert.IsFalse (object.ReferenceEquals (endPointdefinition, deserializedEndPointDefinition));
      AreEqual (endPointdefinition, deserializedEndPointDefinition);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionWithRelationDefinition ()
    {
      ClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order), false);
      ClassDefinition orderTicketDefinition = new ReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket), false);

      orderTicketDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = new VirtualRelationEndPointDefinition (
          orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

      RelationDefinition relationDefinition = new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      VirtualRelationEndPointDefinition deserializedOrderEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (orderEndPointDefinition);

      Assert.IsFalse (object.ReferenceEquals (orderEndPointDefinition, deserializedOrderEndPointDefinition));
      AreEqual (orderEndPointDefinition, deserializedOrderEndPointDefinition);
    }

    [Test]
    public void VirtualRelationEndPointDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"];
      VirtualRelationEndPointDefinition endPointDefinition = (VirtualRelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("Order", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      VirtualRelationEndPointDefinition deserializedEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    public void AnonymousRelationEndPointDefinitionWithoutRelationDefinition ()
    {
      ClassDefinition classDefinition = new ReflectionBasedClassDefinition ("Client", "Client", "TestDomain", typeof (Client), false);

      AnonymousRelationEndPointDefinition endPointdefinition = new AnonymousRelationEndPointDefinition (classDefinition);

      AnonymousRelationEndPointDefinition deserializedEndPointDefinition = (AnonymousRelationEndPointDefinition) SerializeAndDeserialize (endPointdefinition);

      Assert.IsFalse (object.ReferenceEquals (endPointdefinition, deserializedEndPointDefinition));
      AreEqual (endPointdefinition, deserializedEndPointDefinition);
    }

    [Test]
    public void AnonymousRelationEndPointDefinitionWithRelationDefinition ()
    {
      ClassDefinition clientDefinition = new ReflectionBasedClassDefinition ("Client", "Client", "TestDomain", typeof (Client), false);
      ClassDefinition locationDefinition = new ReflectionBasedClassDefinition ("Location", "Location", "TestDomain", typeof (Location), false);

      locationDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Client", "ClientID", TypeInfo.ObjectIDMappingTypeName, false));

      AnonymousRelationEndPointDefinition clientEndPointDefinition = new AnonymousRelationEndPointDefinition (clientDefinition);
      RelationEndPointDefinition locationEndPointDefinition = new RelationEndPointDefinition (locationDefinition, "Client", true);

      RelationDefinition relationDefinition = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", clientEndPointDefinition, locationEndPointDefinition);

      AnonymousRelationEndPointDefinition deserializedClientEndPointDefinition = (AnonymousRelationEndPointDefinition) SerializeAndDeserialize (clientEndPointDefinition);

      Assert.IsFalse (object.ReferenceEquals (clientEndPointDefinition, deserializedClientEndPointDefinition));
      AreEqual (clientEndPointDefinition, deserializedClientEndPointDefinition);
    }

    [Test]
    public void AnonymousRelationEndPointDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client"];
      AnonymousRelationEndPointDefinition endPointDefinition = (AnonymousRelationEndPointDefinition) relationDefinition.GetOppositeEndPointDefinition ("Location", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");

      AnonymousRelationEndPointDefinition deserializedEndPointDefinition = (AnonymousRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    public void RelationDefinitionNotInMapping ()
    {
      RelationDefinition relationDefinition = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
      RelationDefinition deserializedRelationDefinition = (RelationDefinition) SerializeAndDeserialize (relationDefinition);

      Assert.IsFalse (object.ReferenceEquals (relationDefinition, deserializedRelationDefinition));
      AreEqual (relationDefinition, deserializedRelationDefinition);
    }

    [Test]
    public void RelationDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
      RelationDefinition deserializedRelationDefinition = (RelationDefinition) SerializeAndDeserialize (relationDefinition);

      Assert.AreSame (relationDefinition, deserializedRelationDefinition);
      AreEqual (relationDefinition, deserializedRelationDefinition);
    }

    [Test]
    public void SimpleRelationDefinitionCollection ()
    {
      RelationDefinitionCollection definitions = new RelationDefinitionCollection ();
      definitions.Add (MappingConfiguration.Current.RelationDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.RelationDefinitions[1]);

      RelationDefinitionCollection deserializedDefinitions = (RelationDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (object.ReferenceEquals (definitions, deserializedDefinitions));
      Assert.AreSame (definitions[0], deserializedDefinitions[0]);
      Assert.AreSame (definitions[1], deserializedDefinitions[1]);
    }

    [Test]
    public void SimpleClassDefinitionCollection ()
    {
      ClassDefinitionCollection definitions = new ClassDefinitionCollection ();
      definitions.Add (TestMappingConfiguration.Current.ClassDefinitions[0]);
      definitions.Add (TestMappingConfiguration.Current.ClassDefinitions[1]);

      ClassDefinitionCollection deserializedDefinitions = (ClassDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (object.ReferenceEquals (definitions, deserializedDefinitions));
      AreEqual (definitions[0], deserializedDefinitions[0]);
      AreEqual (definitions[1], deserializedDefinitions[1]);
      Assert.IsTrue (deserializedDefinitions.Contains (definitions[0].ID));
    }

    [Test]
    public void ClassDefinitionNotInMapping ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      ClassDefinition deserializedClassDefinition = (ClassDefinition) SerializeAndDeserialize (classDefinition);

      Assert.IsFalse (object.ReferenceEquals (classDefinition, deserializedClassDefinition));
      AreEqual (classDefinition, deserializedClassDefinition);
    }

    [Test]
    public void ClassDefinitionInMapping ()
    {
      // Note: Partner has a base class and several derived classes.
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      ClassDefinition deserializedClassDefinition = (ClassDefinition) SerializeAndDeserialize (classDefinition);

      Assert.AreSame (classDefinition, deserializedClassDefinition);
    }

    [Test]
    public void PropertyDefinitionCollectionInMapping ()
    {
      PropertyDefinitionCollection definitions = MappingConfiguration.Current.ClassDefinitions["Order"].MyPropertyDefinitions;
      PropertyDefinitionCollection deserializedDefinitions = (PropertyDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (object.ReferenceEquals (definitions, deserializedDefinitions));
      Assert.AreSame (definitions.ClassDefinition, deserializedDefinitions.ClassDefinition);
    }

    [Test]
    public void PropertyDefinitionWithEnumType ()
    {
      PropertyDefinition enumPropertyDefinition =
          TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer").MyPropertyDefinitions["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"];

      PropertyDefinition deserializedEnumPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (enumPropertyDefinition);

      AreEqual (enumPropertyDefinition, deserializedEnumPropertyDefinition);
    }

    private void AreEqual (ClassDefinition expected, ClassDefinition actual)
    {
      Assert.AreEqual (expected.ID, actual.ID);

      if (expected.BaseClass != null)
        Assert.AreEqual (expected.BaseClass.ID, actual.BaseClass.ID);
      else
        Assert.IsNull (actual.BaseClass);

      Assert.AreEqual (expected.ClassType, actual.ClassType);
      Assert.AreEqual (expected.MyEntityName, actual.MyEntityName);
      Assert.AreEqual (expected.StorageProviderID, actual.StorageProviderID);

      Assert.AreEqual (expected.DerivedClasses.Count, actual.DerivedClasses.Count);
      for (int i = 0; i < expected.DerivedClasses.Count; i++)
        AreEqual (expected.DerivedClasses[i], actual.DerivedClasses[i]);

      Assert.AreEqual (expected.MyPropertyDefinitions.Count, actual.MyPropertyDefinitions.Count);
      for (int i = 0; i < expected.MyPropertyDefinitions.Count; i++)
        AreEqual (expected.MyPropertyDefinitions[i], actual.MyPropertyDefinitions[i]);

      Assert.AreEqual (expected.MyRelationDefinitions.Count, actual.MyRelationDefinitions.Count);
      for (int i = 0; i < expected.MyRelationDefinitions.Count; i++)
        AreEqual (expected.MyRelationDefinitions[i], actual.MyRelationDefinitions[i]);
    }

    private void AreEqual (RelationDefinition expected, RelationDefinition actual)
    {
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreEqual (expected.EndPointDefinitions.Length, actual.EndPointDefinitions.Length);

      for (int i = 0; i < expected.EndPointDefinitions.Length; i++)
        AreEqual (expected.EndPointDefinitions[i], actual.EndPointDefinitions[i]);
    }

    private void AreEqual (IRelationEndPointDefinition expected, IRelationEndPointDefinition actual)
    {
      Assert.AreEqual (expected.GetType (), actual.GetType ());

      Assert.AreEqual (expected.Cardinality, actual.Cardinality);
      Assert.AreEqual (expected.ClassDefinition.ID, actual.ClassDefinition.ID);
      Assert.AreEqual (expected.IsMandatory, actual.IsMandatory);
      Assert.AreEqual (expected.IsNull, actual.IsNull);
      Assert.AreEqual (expected.IsVirtual, actual.IsVirtual);
      Assert.AreEqual (expected.PropertyName, actual.PropertyName);
      Assert.AreEqual (expected.PropertyType, actual.PropertyType);

      if (expected.RelationDefinition != null)
        Assert.AreEqual (expected.RelationDefinition.ID, actual.RelationDefinition.ID);
      else
        Assert.IsNull (actual.RelationDefinition);

      if (expected.GetType () == typeof (VirtualRelationEndPointDefinition))
        Assert.AreEqual (((VirtualRelationEndPointDefinition) expected).SortExpression, ((VirtualRelationEndPointDefinition) actual).SortExpression);
    }

    private void AreEqual (PropertyDefinition expected, PropertyDefinition actual)
    {
      if (expected.ClassDefinition != null)
        Assert.AreEqual (expected.ClassDefinition.ID, actual.ClassDefinition.ID);
      else
        Assert.IsNull (actual.ClassDefinition);

      Assert.AreEqual (expected.StorageSpecificName, actual.StorageSpecificName);
      Assert.AreEqual (expected.DefaultValue, actual.DefaultValue);
      Assert.AreEqual (expected.IsNullable, actual.IsNullable);
      Assert.AreEqual (expected.MappingTypeName, actual.MappingTypeName);
      Assert.AreEqual (expected.MaxLength, actual.MaxLength);
      Assert.AreEqual (expected.PropertyName, actual.PropertyName);
      Assert.AreEqual (expected.PropertyType, actual.PropertyType);
      Assert.AreEqual (expected.IsPersistent, actual.IsPersistent);
    }
  }
}
