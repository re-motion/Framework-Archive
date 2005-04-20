using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ClassDefinitionWithNullEndPointTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _clientClass;
  private NullRelationEndPointDefinition _clientEndPoint;
  private ClassDefinition _locationClass;
  private RelationEndPointDefinition _locationEndPoint;

  // construction and disposing

  public ClassDefinitionWithNullEndPointTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _clientClass = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Client");
    _locationClass = TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Location");

    RelationDefinition relation = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("ClientToLocation");
    _clientEndPoint = (NullRelationEndPointDefinition) relation.EndPointDefinitions[0];
    _locationEndPoint = (RelationEndPointDefinition) relation.EndPointDefinitions[1];
  }

  [Test]
  public void GetRelationDefinitions ()
  {
    Assert.IsTrue (_locationClass.GetRelationDefinitions().Contains ("ClientToLocation"));
    Assert.IsFalse (_clientClass.GetRelationDefinitions().Contains ("ClientToLocation"));
  }

  [Test]
  public void GetRelationEndPointDefinitions ()
  {
    Assert.IsTrue (Contains (_locationClass.GetRelationEndPointDefinitions (), _locationEndPoint));
    Assert.IsFalse (Contains (_clientClass.GetRelationEndPointDefinitions (), _clientEndPoint));
  }

  [Test]
  public void GetMyRelationEndPointDefinitions ()
  {
    Assert.IsTrue (Contains (_locationClass.GetMyRelationEndPointDefinitions (), _locationEndPoint));
    Assert.IsFalse (Contains (_clientClass.GetMyRelationEndPointDefinitions (), _clientEndPoint));
  }

  private bool Contains (IRelationEndPointDefinition[] endPoints, IRelationEndPointDefinition value)
  {
    foreach (IRelationEndPointDefinition endPoint in endPoints)
    {
      if (object.ReferenceEquals (endPoint, value))
        return true;
    }

    return false;
  }
}
}
