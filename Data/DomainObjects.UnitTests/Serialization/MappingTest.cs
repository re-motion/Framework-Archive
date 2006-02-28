using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
[TestFixture]
public class MappingTest : SerializationBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public MappingTest ()
  {
  }

  // methods and properties

  [Test]
  public void PropertyDefinitionWithoutClassDefinition ()
  {
    PropertyDefinition propertyDefinition = new PropertyDefinition ("PropertyName", "ColumnName", "string", true, 100);

    PropertyDefinition deserializedPropertyDefinition = (PropertyDefinition) SerializeAndDeserialize (propertyDefinition);
 
    Assert.IsFalse (object.ReferenceEquals (propertyDefinition, deserializedPropertyDefinition));
    AreEqual (propertyDefinition, deserializedPropertyDefinition);
  }

  [Test]
  public void PropertyDefinitionWithClassDefinition ()
  {
    PropertyDefinition propertyDefinition = new PropertyDefinition ("OrderNumber", "OrderNo", "int32", false);
    ClassDefinition classDefinition = new ClassDefinition ("ClassID", "EntityName", "TestDomain", typeof (Order));
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
    PropertyDefinition orderNumberDefinition = orderDefinition["OrderNumber"];

    PropertyDefinition deserializedOrderNumberDefinition = (PropertyDefinition) SerializeAndDeserialize (orderNumberDefinition);
    Assert.AreSame (orderNumberDefinition, deserializedOrderNumberDefinition);
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
    ClassDefinition classDefinition = new ClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket)); 
    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID", false));
    RelationEndPointDefinition endPointdefinition = new RelationEndPointDefinition (classDefinition, "Order", true);

    RelationEndPointDefinition deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointdefinition);
 
    Assert.IsFalse (object.ReferenceEquals (endPointdefinition, deserializedEndPointDefinition));
    AreEqual (endPointdefinition, deserializedEndPointDefinition);
  }

  [Test]
  public void RelationEndPointDefinitionWithRelationDefinition ()
  {
    ClassDefinition orderDefinition = new ClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
    ClassDefinition orderTicketDefinition = new ClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket)); 
    
    orderTicketDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID", false));

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
    RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["OrderToOrderTicket"];
    RelationEndPointDefinition endPointDefinition = (RelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("OrderTicket", "Order");

    RelationEndPointDefinition deserializedEndPointDefinition = (RelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
    Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
  }

  [Test]
  public void VirtualRelationEndPointDefinitionWithoutRelationDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("Order", "Order", "TestDomain", typeof (Order)); 

    VirtualRelationEndPointDefinition endPointdefinition = new VirtualRelationEndPointDefinition (
        classDefinition, "OrderTicket", true, CardinalityType.One, typeof (Order));

    VirtualRelationEndPointDefinition deserializedEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointdefinition);
 
    Assert.IsFalse (object.ReferenceEquals (endPointdefinition, deserializedEndPointDefinition));
    AreEqual (endPointdefinition, deserializedEndPointDefinition);
  }

  [Test]
  public void VirtualRelationEndPointDefinitionWithRelationDefinition ()
  {
    ClassDefinition orderDefinition = new ClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
    ClassDefinition orderTicketDefinition = new ClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket)); 
    
    orderTicketDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID", false));

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
    RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["OrderToOrderTicket"];
    VirtualRelationEndPointDefinition endPointDefinition = (VirtualRelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("Order", "OrderTicket");

    VirtualRelationEndPointDefinition deserializedEndPointDefinition = (VirtualRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
    Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
  }

  [Test]
  public void NullRelationEndPointDefinitionWithoutRelationDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("Client", "Client", "TestDomain", typeof (Client)); 

    NullRelationEndPointDefinition endPointdefinition = new NullRelationEndPointDefinition (classDefinition);

    NullRelationEndPointDefinition deserializedEndPointDefinition = (NullRelationEndPointDefinition) SerializeAndDeserialize (endPointdefinition);
 
    Assert.IsFalse (object.ReferenceEquals (endPointdefinition, deserializedEndPointDefinition));
    AreEqual (endPointdefinition, deserializedEndPointDefinition);
  }

  [Test]
  public void NullRelationEndPointDefinitionWithRelationDefinition ()
  {
    ClassDefinition clientDefinition = new ClassDefinition ("Client", "Client", "TestDomain", typeof (Client));
    ClassDefinition locationDefinition = new ClassDefinition ("Location", "Location", "TestDomain", typeof (Location)); 
    
    locationDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Client", "ClientID", "objectID", false));

    NullRelationEndPointDefinition clientEndPointDefinition = new NullRelationEndPointDefinition (clientDefinition);
    RelationEndPointDefinition locationEndPointDefinition = new RelationEndPointDefinition (locationDefinition, "Client", true);

    RelationDefinition relationDefinition = new RelationDefinition ("ClientToLocation", clientEndPointDefinition, locationEndPointDefinition);

    NullRelationEndPointDefinition deserializedClientEndPointDefinition = (NullRelationEndPointDefinition) SerializeAndDeserialize (clientEndPointDefinition);
 
    Assert.IsFalse (object.ReferenceEquals (clientEndPointDefinition, deserializedClientEndPointDefinition));
    AreEqual (clientEndPointDefinition, deserializedClientEndPointDefinition);
  }

  [Test]
  public void NullRelationEndPointDefinitionInMapping ()
  {
    RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions["ClientToLocation"];
    NullRelationEndPointDefinition endPointDefinition = (NullRelationEndPointDefinition) relationDefinition.GetOppositeEndPointDefinition ("Location", "Client");

    NullRelationEndPointDefinition deserializedEndPointDefinition = (NullRelationEndPointDefinition) SerializeAndDeserialize (endPointDefinition);
    Assert.AreSame (endPointDefinition, deserializedEndPointDefinition);
  }

  [Test]
  public void RelationDefinitionNotInMapping ()
  {
    RelationDefinition relationDefinition = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("OrderToOrderTicket");
    RelationDefinition deserializedRelationDefinition = (RelationDefinition) SerializeAndDeserialize (relationDefinition);

    Assert.IsFalse (object.ReferenceEquals (relationDefinition, deserializedRelationDefinition));
    AreEqual (relationDefinition, deserializedRelationDefinition);
  }

  [Test]
  public void RelationDefinitionInMapping ()
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
  [Ignore ("Reactivate test after ClassDefinitionCollection can handle unresolved type names.")]  
  public void ClassDefinitionNotInMappingWithUnresolvedClassType ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("Order", "OrderTable", "StorageProver", "UnexistingTypeName", false);
    
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
        TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer").MyPropertyDefinitions["CustomerType"];

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
    Assert.AreEqual (expected.ClassTypeName, actual.ClassTypeName);
    Assert.AreEqual (expected.EntityName, actual.EntityName);
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

    Assert.AreEqual (expected.ColumnName, actual.ColumnName);
    Assert.AreEqual (expected.DefaultValue, actual.DefaultValue);
    Assert.AreEqual (expected.IsNullable, actual.IsNullable);
    Assert.AreEqual (expected.MappingType, actual.MappingType);
    Assert.AreEqual (expected.MaxLength, actual.MaxLength);
    Assert.AreEqual (expected.PropertyName, actual.PropertyName);
    Assert.AreEqual (expected.PropertyType, actual.PropertyType);
  }
}
}
