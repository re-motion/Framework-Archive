using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Factories;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Remotion.NullableValueTypes;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Serialization
{
  [TestFixture]
  public class MappingTest : SerializationBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The XmlBasedPropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithoutClassDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order));
      XmlBasedPropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition (classDefinition, "PropertyName", "ColumnName", "string", true, true, 100);

      XmlBasedPropertyDefinition deserializedPropertyDefinition = (XmlBasedPropertyDefinition) SerializeAndDeserialize (propertyDefinition);

      Assert.IsFalse (object.ReferenceEquals (propertyDefinition, deserializedPropertyDefinition));
      AreEqual (propertyDefinition, deserializedPropertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The XmlBasedPropertyDefinition 'OrderNumber' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithClassDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order));
      XmlBasedPropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition (classDefinition, "OrderNumber", "OrderNo", "int32", false);
      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      SerializeAndDeserialize (propertyDefinition);
    }

    [Test]
    public void PropertyDefinition_InMapping ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      XmlBasedPropertyDefinition orderNumberDefinition = (XmlBasedPropertyDefinition) orderDefinition["OrderNumber"];

      XmlBasedPropertyDefinition deserializedOrderNumberDefinition = (XmlBasedPropertyDefinition) SerializeAndDeserialize (orderNumberDefinition);
      Assert.AreSame (orderNumberDefinition, deserializedOrderNumberDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The XmlBasedPropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithUnresolvedNativePropertyType_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order));
      XmlBasedPropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition (classDefinition, "PropertyName", "ColumnName", "int32", false, true, null);

      XmlBasedPropertyDefinition deserializedPropertyDefinition = (XmlBasedPropertyDefinition) SerializeAndDeserialize (propertyDefinition);
      AreEqual (propertyDefinition, deserializedPropertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The XmlBasedPropertyDefinition 'PropertyName' cannot be serialized because is is not part of the current mapping.")]
    public void PropertyDefinitionWithUnresolvedUnknownPropertyType_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
      XmlBasedPropertyDefinition propertyDefinition = new XmlBasedPropertyDefinition (classDefinition, "PropertyName", "ColumnName", "UnknownPropertyType", false, true, null);

      XmlBasedPropertyDefinition deserializedPropertyDefinition = (XmlBasedPropertyDefinition) SerializeAndDeserialize (propertyDefinition);
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
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The RelationEndPointDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket));
      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classDefinition, "Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));
      RelationEndPointDefinition endPointdefinition = new RelationEndPointDefinition (classDefinition, "Order", true);

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The RelationEndPointDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void RelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition orderDefinition = new XmlBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
      XmlBasedClassDefinition orderTicketDefinition = new XmlBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket));

      orderTicketDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderTicketDefinition, "Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = new VirtualRelationEndPointDefinition (
          orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

      RelationDefinition relationDefinition = new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      SerializeAndDeserialize (orderTicketEndPointdefinition);
    }

    [Test]
    public void RelationEndPointDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["OrderToOrderTicket"];
      RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("OrderTicket", "Order");

      RelationEndPointDefinition deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The VirtualRelationEndPointDefinition 'OrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void VirtualRelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));

      VirtualRelationEndPointDefinition endPointdefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "OrderTicket", true, CardinalityType.One, typeof (Order));

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The VirtualRelationEndPointDefinition 'OrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void VirtualRelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition orderDefinition = new XmlBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
      XmlBasedClassDefinition orderTicketDefinition = new XmlBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket));

      orderTicketDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderTicketDefinition, "Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));

      VirtualRelationEndPointDefinition orderEndPointDefinition = new VirtualRelationEndPointDefinition (
          orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

      RelationDefinition relationDefinition = new RelationDefinition ("OrderToOrderTicket", orderEndPointDefinition, orderTicketEndPointdefinition);

      SerializeAndDeserialize (orderEndPointDefinition);
    }

    [Test]
    public void VirtualRelationEndPointDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["OrderToOrderTicket"];
      VirtualRelationEndPointDefinition endPointDefinition = (VirtualRelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("Order", "OrderTicket");

      VirtualRelationEndPointDefinition deserializedEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The AnonymousRelationEndPointDefinition '<anonymous>' cannot be serialized because is is not part of the current mapping.")]
    public void AnonymousRelationEndPointDefinitionWithoutRelationDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("Client", "Client", "TestDomain", typeof (Client));

      AnonymousRelationEndPointDefinition endPointdefinition = new AnonymousRelationEndPointDefinition (classDefinition);

      SerializeAndDeserialize (endPointdefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The AnonymousRelationEndPointDefinition '<anonymous>' cannot be serialized because is is not part of the current mapping.")]
    public void AnonymousRelationEndPointDefinitionWithRelationDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition clientDefinition = new XmlBasedClassDefinition ("Client", "Client", "TestDomain", typeof (Client));
      XmlBasedClassDefinition locationDefinition = new XmlBasedClassDefinition ("Location", "Location", "TestDomain", typeof (Location));

      locationDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (locationDefinition, "Client", "ClientID", TypeInfo.ObjectIDMappingTypeName, false));

      AnonymousRelationEndPointDefinition clientEndPointDefinition = new AnonymousRelationEndPointDefinition (clientDefinition);
      RelationEndPointDefinition locationEndPointDefinition = new RelationEndPointDefinition (locationDefinition, "Client", true);

      RelationDefinition relationDefinition = new RelationDefinition ("ClientToLocation", clientEndPointDefinition, locationEndPointDefinition);

      SerializeAndDeserialize (clientEndPointDefinition);
    }

    [Test]
    public void AnonymousRelationEndPointDefinitionInMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["ClientToLocation"];
      AnonymousRelationEndPointDefinition endPointDefinition = (AnonymousRelationEndPointDefinition) relationDefinition.GetOppositeEndPointDefinition ("Location", "Client");

      AnonymousRelationEndPointDefinition deserializedEndPointDefinition = (AnonymousRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
      Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The RelationDefinition 'OrderToOrderTicket' cannot be serialized because is is not part of the current mapping.")]
    public void RelationDefinition_NotInMapping ()
    {
      RelationDefinition relationDefinition = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("OrderToOrderTicket");
      SerializeAndDeserialize (relationDefinition);
    }

    [Test]
    public void RelationDefinition_InMapping ()
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory ("OrderToOrderTicket");
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
      definitions.Add (MappingConfiguration.Current.ClassDefinitions[0]);
      definitions.Add (MappingConfiguration.Current.ClassDefinitions[1]);

      ClassDefinitionCollection deserializedDefinitions = (ClassDefinitionCollection) SerializeAndDeserialize (definitions);

      Assert.IsFalse (object.ReferenceEquals (definitions, deserializedDefinitions));
      AreEqual (definitions[0], deserializedDefinitions[0]);
      AreEqual (definitions[1], deserializedDefinitions[1]);
      Assert.IsTrue (deserializedDefinitions.Contains (definitions[0].ID));
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The XmlBasedClassDefinition 'Partner' cannot be serialized because is is not part of the current mapping.")]
    public void ClassDefinition_NotInMapping ()
    {
      XmlBasedClassDefinition classDefinition = (XmlBasedClassDefinition) TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      SerializeAndDeserialize (classDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "The XmlBasedClassDefinition 'Order' cannot be serialized because is is not part of the current mapping.")]
    public void ClassDefinitionNotInMappingWithUnresolvedClassType ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProver", "UnexistingType", false);

      SerializeAndDeserialize (classDefinition);
    }

    [Test]
    public void ClassDefinitionInMapping ()
    {
      // Note: Partner has a base class and several derived classes.
      XmlBasedClassDefinition classDefinition = (XmlBasedClassDefinition) MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");

      XmlBasedClassDefinition deserializedClassDefinition = (XmlBasedClassDefinition) SerializeAndDeserialize (classDefinition);

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
      XmlBasedPropertyDefinition enumPropertyDefinition =
          (XmlBasedPropertyDefinition) MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer").MyPropertyDefinitions["Type"];

      XmlBasedPropertyDefinition deserializedEnumPropertyDefinition = (XmlBasedPropertyDefinition) SerializeAndDeserialize (enumPropertyDefinition);

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
      Assert.AreEqual (((XmlBasedClassDefinition)expected).ClassTypeName, ((XmlBasedClassDefinition)actual).ClassTypeName);
      Assert.AreEqual (expected.MyEntityName, actual.MyEntityName);
      Assert.AreEqual (expected.StorageProviderID, actual.StorageProviderID);

      Assert.AreEqual (expected.DerivedClasses.Count, actual.DerivedClasses.Count);
      for (int i = 0; i < expected.DerivedClasses.Count; i++)
        AreEqual (expected.DerivedClasses[i], actual.DerivedClasses[i]);

      Assert.AreEqual (expected.MyPropertyDefinitions.Count, actual.MyPropertyDefinitions.Count);
      for (int i = 0; i < expected.MyPropertyDefinitions.Count; i++)
        AreEqual ((XmlBasedPropertyDefinition) expected.MyPropertyDefinitions[i], (XmlBasedPropertyDefinition) actual.MyPropertyDefinitions[i]);

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

    private void AreEqual (XmlBasedPropertyDefinition expected, XmlBasedPropertyDefinition actual)
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
