using System;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
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
      classDefinition.MyPropertyDefinitions.Add (
          new XmlBasedPropertyDefinition (classDefinition, "Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName, true, false, null));

      return classDefinition;
    }

    public static XmlBasedClassDefinition CreateWithUnresolvedClassType()
    {
      return new XmlBasedClassDefinition ("ClassID", "Entity", "StorageProviderID", "UnresolvedType", false);
    }

    public static XmlBasedClassDefinition CreateWithUnresolvedRelationProperty()
    {
      XmlBasedClassDefinition classDefinition = CreateWithUnresolvedClassType();
      classDefinition.MyPropertyDefinitions.Add (
          new XmlBasedPropertyDefinition (classDefinition, "PropertyName", "StorageSpecificName", TypeInfo.ObjectIDMappingTypeName, false, false, null));

      return classDefinition;
    }
  }
}