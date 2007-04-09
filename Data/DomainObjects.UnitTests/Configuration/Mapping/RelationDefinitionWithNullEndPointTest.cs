using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationDefinitionWithNullEndPointTest : ReflectionBasedMappingTest
  {
    private RelationDefinition _relation;
    private NullRelationEndPointDefinition _clientEndPoint;
    private RelationEndPointDefinition _locationEndPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _relation = TestMappingConfiguration.Current.RelationDefinitions.GetMandatory ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _clientEndPoint = (NullRelationEndPointDefinition) _relation.EndPointDefinitions[0];
      _locationEndPoint = (RelationEndPointDefinition) _relation.EndPointDefinitions[1];
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      Assert.AreSame (_clientEndPoint, _relation.GetOppositeEndPointDefinition ("Location", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.AreSame (_locationEndPoint, _relation.GetOppositeEndPointDefinition ("Client", null));
    }

    [Test]
    public void GetOppositeClassDefinition ()
    {
      Assert.AreSame (TestMappingConfiguration.Current.ClassDefinitions[typeof (Client)], _relation.GetOppositeClassDefinition ("Location", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.AreSame (TestMappingConfiguration.Current.ClassDefinitions[typeof (Location)], _relation.GetOppositeClassDefinition ("Client", null));
    }

    [Test]
    public void IsEndPoint ()
    {
      Assert.IsTrue (_relation.IsEndPoint ("Location", "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
      Assert.IsTrue (_relation.IsEndPoint ("Client", null));

      Assert.IsFalse (_relation.IsEndPoint ("Location", null));
      Assert.IsFalse (_relation.IsEndPoint ("Client", "Client"));
    }
  }
}
