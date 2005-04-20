using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class RelationDefinitionWithNullEndPointTest
{
  // types

  // static members and constants

  // member fields

  private RelationDefinition _relation;
  private NullRelationEndPointDefinition _clientEndPoint;
  private RelationEndPointDefinition _locationEndPoint;

  // construction and disposing

  public RelationDefinitionWithNullEndPointTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _relation = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("ClientToLocation");
    _clientEndPoint = (NullRelationEndPointDefinition) _relation.EndPointDefinitions[0];
    _locationEndPoint = (RelationEndPointDefinition) _relation.EndPointDefinitions[1];
  }

  [Test]
  public void GetOppositeEndPointDefinition ()
  {
    Assert.AreSame (_clientEndPoint, _relation.GetOppositeEndPointDefinition ("Location", "Client"));
    Assert.AreSame (_locationEndPoint, _relation.GetOppositeEndPointDefinition ("Client", null));
  }

  [Test]
  public void GetOppositeClassDefinition ()
  {
    Assert.AreSame (TestMappingConfiguration.Current.ClassDefinitions["Client"], _relation.GetOppositeClassDefinition ("Location", "Client"));
    Assert.AreSame (TestMappingConfiguration.Current.ClassDefinitions["Location"], _relation.GetOppositeClassDefinition ("Client", null));
  }

  [Test]
  public void IsEndPoint ()
  {
    Assert.IsTrue (_relation.IsEndPoint ("Location", "Client"));
    Assert.IsTrue (_relation.IsEndPoint ("Client", null));

    Assert.IsFalse (_relation.IsEndPoint ("Location", null));
    Assert.IsFalse (_relation.IsEndPoint ("Client", "Client"));
  }
}
}
