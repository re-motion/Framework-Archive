using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ConstructionOfVirtualRelationEndPointDefinitionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ConstructionOfVirtualRelationEndPointDefinitionTest ()
  {
  }

  // methods and properties

  [Test]
  [ExpectedException (typeof (MappingException),
      "Relation definition error: Virtual property 'Dummy' of class 'Company' is of type"
          + "'Rubicon.Data.DomainObjects.DomainObject',"
          + " but must be derived from 'Rubicon.Data.DomainObjects.DomainObject' or "
          + " 'Rubicon.Data.DomainObjects.DomainObjectCollection' or must be"
          + " 'Rubicon.Data.DomainObjects.DomainObjectCollection'.")]
  public void VirtualEndPointOfDomainObjectType ()
  {
    ClassDefinition companyDefinition = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
        companyDefinition, "Dummy", false, CardinalityType.One, typeof (DomainObject));
  }

  [Test]
  public void VirtualEndPointOfDomainObjectCollectionType ()
  {
    ClassDefinition companyDefinition = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
        companyDefinition, "Dummy", false, CardinalityType.Many, typeof (DomainObjectCollection));
  }

  [Test]
  public void VirtualEndPointOfOrderCollectionType ()
  {
    ClassDefinition companyDefinition = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
        companyDefinition, "Dummy", false, CardinalityType.Many, typeof (OrderCollection));
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "The property type of a virtual end point of a one-to-one relation"
      + " must be derived from 'Rubicon.Data.DomainObjects.DomainObject'.")]
  public void VirtualEndPointWithCardinalityOneAndWrongPropertyType ()
  {
    ClassDefinition companyDefinition = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
        companyDefinition, "Dummy", false, CardinalityType.One, typeof (OrderCollection));
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "The property type of a virtual end point of a one-to-many relation"
      + " must be or be derived from 'Rubicon.Data.DomainObjects.DomainObjectCollection'.")]
  public void VirtualEndPointWithCardinalityManyAndWrongPropertyType ()
  {
    ClassDefinition companyDefinition = new ClassDefinition ("Company", "Company", typeof (Company), "TestDomain");

    VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
        companyDefinition, "Dummy", false, CardinalityType.Many, typeof (Company));
  }
}
}
