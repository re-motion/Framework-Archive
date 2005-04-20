using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class NullRelationEndPointDefinitionTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _clientDefinition;
  private NullRelationEndPointDefinition _definition;
  
  // construction and disposing

  public NullRelationEndPointDefinitionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _clientDefinition = MappingConfiguration.Current.ClassDefinitions["Client"]; 
    _definition = new NullRelationEndPointDefinition (_clientDefinition);
  }

  [Test]
  public void Initialize ()
  {

    Assert.IsNotNull (_definition as IRelationEndPointDefinition);
    Assert.IsNotNull (_definition as INullableObject);
    Assert.AreSame (_clientDefinition, _definition.ClassDefinition);
    Assert.AreEqual (CardinalityType.Many, _definition.Cardinality);
    Assert.AreEqual (false, _definition.IsMandatory);
    Assert.AreEqual (true, _definition.IsVirtual);
    Assert.IsNull (_definition.PropertyName);
    Assert.IsNull (_definition.PropertyType);
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
}
}
