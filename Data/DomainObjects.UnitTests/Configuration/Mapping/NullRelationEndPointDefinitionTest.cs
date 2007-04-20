using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class NullRelationEndPointDefinitionTest : ReflectionBasedMappingTest
  {
    private ClassDefinition _clientDefinition;
    private NullRelationEndPointDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientDefinition = MappingConfiguration.Current.ClassDefinitions["Client"];
      _definition = new NullRelationEndPointDefinition (_clientDefinition);
    }

    [Test]
    public void Initialize ()
    {

      Assert.IsNotNull (_definition as IRelationEndPointDefinition);
      Assert.IsNotNull (_definition as INullObject);
      Assert.AreSame (_clientDefinition, _definition.ClassDefinition);
      Assert.AreEqual (CardinalityType.Many, _definition.Cardinality);
      Assert.AreEqual (false, _definition.IsMandatory);
      Assert.AreEqual (true, _definition.IsVirtual);
      Assert.IsNull (_definition.PropertyName);
      Assert.IsNull (_definition.PropertyType);
      Assert.AreEqual (_clientDefinition.IsClassTypeResolved, _definition.IsPropertyTypeResolved);
      Assert.IsNull (_definition.PropertyTypeName);
      Assert.IsTrue (_definition.IsNull);
    }

    [Test]
    public void CorrespondsToTrue ()
    {
      Assert.IsTrue (_definition.CorrespondsTo (_clientDefinition.ID, null));
    }

    [Test]
    public void CorrespondsToFalse ()
    {
      Assert.IsFalse (_definition.CorrespondsTo (_clientDefinition.ID, "PropertyName"));
    }

    [Test]
    public void RelationDefinitionNull ()
    {
      NullRelationEndPointDefinition definition = new NullRelationEndPointDefinition (MappingConfiguration.Current.ClassDefinitions[typeof (Client)]);

      Assert.IsNull (definition.RelationDefinition);
    }

    [Test]
    public void RelationDefinitionNotNull ()
    {
      RelationEndPointDefinition oppositeEndPoint = new RelationEndPointDefinition (
          MappingConfiguration.Current.ClassDefinitions[typeof (Location)], "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", true);

      RelationDefinition relationDefinition = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", _definition, oppositeEndPoint);

      Assert.IsNotNull (_definition.RelationDefinition);
    }
  }
}
