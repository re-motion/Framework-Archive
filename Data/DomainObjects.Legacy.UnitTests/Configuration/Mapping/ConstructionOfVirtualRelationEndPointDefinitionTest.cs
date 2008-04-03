using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ConstructionOfVirtualRelationEndPointDefinitionTest : StandardMappingTest
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
        ExpectedMessage = "Relation definition error: Virtual property 'Dummy' of class 'Company' is of type"
            + "'Remotion.Data.DomainObjects.DomainObject',"
            + " but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
            + " 'Remotion.Data.DomainObjects.DomainObjectCollection' or must be"
            + " 'Remotion.Data.DomainObjects.DomainObjectCollection'.")]
    public void VirtualEndPointOfDomainObjectType ()
    {
      XmlBasedClassDefinition companyDefinition = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          companyDefinition, "Dummy", false, CardinalityType.One, typeof (DomainObject));
    }

    [Test]
    public void VirtualEndPointOfDomainObjectCollectionType ()
    {
      XmlBasedClassDefinition companyDefinition = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          companyDefinition, "Dummy", false, CardinalityType.Many, typeof (DomainObjectCollection));
    }

    [Test]
    public void VirtualEndPointOfOrderCollectionType ()
    {
      XmlBasedClassDefinition companyDefinition = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          companyDefinition, "Dummy", false, CardinalityType.Many, typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The property type of a virtual end point of a one-to-one relation"
        + " must be derived from 'Remotion.Data.DomainObjects.DomainObject'.")]
    public void VirtualEndPointWithCardinalityOneAndWrongPropertyType ()
    {
      XmlBasedClassDefinition companyDefinition = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          companyDefinition, "Dummy", false, CardinalityType.One, typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The property type of a virtual end point of a one-to-many relation"
        + " must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.")]
    public void VirtualEndPointWithCardinalityManyAndWrongPropertyType ()
    {
      XmlBasedClassDefinition companyDefinition = new XmlBasedClassDefinition ("Company", "Company", "TestDomain", typeof (Company));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          companyDefinition, "Dummy", false, CardinalityType.Many, typeof (Company));
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      XmlBasedClassDefinition customerDefinition = new XmlBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          customerDefinition, "Orders", false, CardinalityType.Many, typeof (OrderCollection), "OrderNumber desc");

      Assert.AreEqual ("OrderNumber desc", endPointDefinition.SortExpression);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Property 'Orders' of class 'Customer' must not specify a SortExpression, because cardinality is equal to 'one'.")]
    public void InitializeWithSortExpressionAndCardinalityOfOne ()
    {
      XmlBasedClassDefinition customerDefinition = new XmlBasedClassDefinition ("Customer", "Customer", "TestDomain", typeof (Customer));

      VirtualRelationEndPointDefinition endPointDefinition = new VirtualRelationEndPointDefinition (
          customerDefinition, "Orders", false, CardinalityType.One, typeof (Order), "OrderNumber desc");
    }
  }
}
