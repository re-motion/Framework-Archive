using System;
using System.Collections;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
[TestFixture]
public class IntegrationTest : SerializationBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public IntegrationTest ()
  {
  }

  // methods and properties

  [Test]
  public void ObjectsFromPartnerClassDefinition ()
  {
    ClassDefinition companyClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Company");
    ClassDefinition supplierClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Supplier");
    ClassDefinition partnerClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");
    PropertyDefinition partnerContactPersonPropertyDefinition = partnerClassDefinition.GetMandatoryPropertyDefinition ("ContactPerson");
    RelationDefinition partnerToPersonRelationDefinition = partnerClassDefinition.GetMandatoryRelationDefinition ("ContactPerson");

    object[] partnerObjects = new object[] {
        partnerClassDefinition,
        partnerClassDefinition.BaseClass,
        partnerClassDefinition.DerivedClasses,
        partnerClassDefinition.MyPropertyDefinitions,
        partnerClassDefinition.MyRelationDefinitions,
        supplierClassDefinition,
        partnerContactPersonPropertyDefinition,
        partnerToPersonRelationDefinition,
        partnerToPersonRelationDefinition.EndPointDefinitions
    };

    object[] deserializedPartnerObjects = (object[]) SerializeAndDeserialize (partnerObjects);

    Assert.AreEqual (partnerObjects.Length, deserializedPartnerObjects.Length);
    Assert.AreSame (partnerClassDefinition, deserializedPartnerObjects[0]);
    Assert.AreSame (companyClassDefinition, deserializedPartnerObjects[1]);

    ClassDefinitionCollection deserializedDerivedClasses = (ClassDefinitionCollection) deserializedPartnerObjects[2];
    Assert.IsFalse (object.ReferenceEquals (partnerClassDefinition.DerivedClasses, deserializedDerivedClasses));
    Assert.AreEqual (partnerClassDefinition.DerivedClasses.Count, deserializedDerivedClasses.Count);
    for (int i = 0; i < partnerClassDefinition.DerivedClasses.Count; i++)
      Assert.AreSame (partnerClassDefinition.DerivedClasses[i], deserializedDerivedClasses[i]);

    PropertyDefinitionCollection deserializedPropertyDefinitions = (PropertyDefinitionCollection) deserializedPartnerObjects[3];
    Assert.IsFalse (object.ReferenceEquals (partnerClassDefinition.MyPropertyDefinitions, deserializedPropertyDefinitions));
    Assert.AreEqual (partnerClassDefinition.MyPropertyDefinitions.Count, deserializedPropertyDefinitions.Count);
    for (int i = 0; i < partnerClassDefinition.MyPropertyDefinitions.Count; i++)
      Assert.AreSame (partnerClassDefinition.MyPropertyDefinitions[i], deserializedPropertyDefinitions[i]);

    RelationDefinitionCollection deserializedRelationDefinitions = (RelationDefinitionCollection) deserializedPartnerObjects[4];
    Assert.IsFalse (object.ReferenceEquals (partnerClassDefinition.MyRelationDefinitions, deserializedRelationDefinitions));
    Assert.AreEqual (partnerClassDefinition.MyRelationDefinitions.Count, deserializedRelationDefinitions.Count);
    for (int i = 0; i < partnerClassDefinition.MyRelationDefinitions.Count; i++)
      Assert.AreSame (partnerClassDefinition.MyRelationDefinitions[i], deserializedRelationDefinitions[i]);

    Assert.AreSame (supplierClassDefinition, deserializedPartnerObjects[5]);

    Assert.AreSame (partnerContactPersonPropertyDefinition, deserializedPartnerObjects[6]);

    Assert.AreSame (partnerToPersonRelationDefinition, deserializedPartnerObjects[7]);

    IRelationEndPointDefinition[] deserializedRelationEndPoints = (IRelationEndPointDefinition[]) deserializedPartnerObjects[8];
    Assert.IsFalse (object.ReferenceEquals (partnerToPersonRelationDefinition.EndPointDefinitions, deserializedRelationEndPoints));
    Assert.AreEqual (partnerToPersonRelationDefinition.EndPointDefinitions.Length, deserializedRelationEndPoints.Length);
    for (int i = 0; i < partnerToPersonRelationDefinition.EndPointDefinitions.Length; i++)
      Assert.AreSame (partnerToPersonRelationDefinition.EndPointDefinitions[i], deserializedRelationEndPoints[i]);
  }

  [Test]
  public void RelationDefinitionsFromClientClassDefinition ()
  {
    ClassDefinition clientClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Client");
    RelationDefinition parentClientToChildClientRelationDefinition = clientClassDefinition.GetMandatoryRelationDefinition ("ParentClient");
    ClassDefinition locationClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Location");
    RelationDefinition clientToLocationRelationDefinition = locationClassDefinition.GetMandatoryRelationDefinition ("Client");

    object[] clientObjects = new object[] {
        clientClassDefinition,
        clientClassDefinition.DerivedClasses,
        clientClassDefinition.MyRelationDefinitions,
        parentClientToChildClientRelationDefinition,
        parentClientToChildClientRelationDefinition.EndPointDefinitions[0],
        parentClientToChildClientRelationDefinition.EndPointDefinitions[1],
        clientToLocationRelationDefinition.EndPointDefinitions[0],
        clientToLocationRelationDefinition.EndPointDefinitions[1],
    };

    object[] deserializedClientObjects = (object[]) SerializeAndDeserialize (clientObjects);

    Assert.AreEqual (clientObjects.Length, deserializedClientObjects.Length);
    Assert.AreSame (clientClassDefinition, deserializedClientObjects[0]);

    ClassDefinitionCollection deserializedDerivedClasses = (ClassDefinitionCollection) deserializedClientObjects[1];
    Assert.IsFalse (object.ReferenceEquals (clientClassDefinition.DerivedClasses, deserializedDerivedClasses));
    Assert.AreEqual (0, deserializedDerivedClasses.Count);

    RelationDefinitionCollection deserializedRelationDefinitions = (RelationDefinitionCollection) deserializedClientObjects[2];
    Assert.IsFalse (object.ReferenceEquals (clientClassDefinition.MyRelationDefinitions, deserializedRelationDefinitions));
    Assert.AreEqual (clientClassDefinition.MyRelationDefinitions.Count, deserializedRelationDefinitions.Count);
    for (int i = 0; i < clientClassDefinition.MyRelationDefinitions.Count; i++)
      Assert.AreSame (clientClassDefinition.MyRelationDefinitions[i], deserializedRelationDefinitions[i]);

    RelationDefinition deserializedParentClientToChildClientRelationDefinition = (RelationDefinition) deserializedClientObjects[3];
    Assert.AreSame (parentClientToChildClientRelationDefinition, deserializedParentClientToChildClientRelationDefinition);

    IRelationEndPointDefinition deserializedParentClientToChildClientFirstEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[4];
    Assert.AreSame (parentClientToChildClientRelationDefinition.EndPointDefinitions[0], deserializedParentClientToChildClientFirstEndPoint);

    IRelationEndPointDefinition deserializedParentClientToChildClientSecondEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[5];
    Assert.AreSame (parentClientToChildClientRelationDefinition.EndPointDefinitions[1], deserializedParentClientToChildClientSecondEndPoint);

    IRelationEndPointDefinition deserializedClientToLocationFirstEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[6];
    Assert.AreSame (clientToLocationRelationDefinition.EndPointDefinitions[0], deserializedClientToLocationFirstEndPoint);

    IRelationEndPointDefinition deserializedClientToLocationSecondEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[7];
    Assert.AreSame (clientToLocationRelationDefinition.EndPointDefinitions[1], deserializedClientToLocationSecondEndPoint);
  }
}
}
