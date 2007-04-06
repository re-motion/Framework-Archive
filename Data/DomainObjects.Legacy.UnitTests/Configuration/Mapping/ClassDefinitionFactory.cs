using System;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  public static class ClassDefinitionFactory
  {
    public static XmlBasedClassDefinition CreateOrderDefinition()
    {
      return new XmlBasedClassDefinition ("Order", "OrderTable", "StorageProviderID", typeof (Order));
    }

    public static XmlBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty()
    {
      XmlBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (PropertyDefinitionFactory.CreateCustomerDefinition());

      return classDefinition;
    }

    public static XmlBasedClassDefinition CreateWithUnresolvedClassType()
    {
      return new XmlBasedClassDefinition ("ClassID", "Entity", "StorageProviderID", "UnresolvedType", false);
    }

    public static XmlBasedClassDefinition CreateWithUnresolvedRelationProperty()
    {
      XmlBasedClassDefinition classDefinition = CreateWithUnresolvedClassType();
      classDefinition.MyPropertyDefinitions.Add (PropertyDefinitionFactory.CreateWithUnresolvedRelationPropertyType());

      return classDefinition;
    }
  }
}