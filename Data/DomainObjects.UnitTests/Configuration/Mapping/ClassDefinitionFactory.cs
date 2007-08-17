using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public static class ClassDefinitionFactory
  {
    public static ReflectionBasedClassDefinition CreateOrderDefinition()
    {
      return new ReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProviderID", typeof (Order), false);
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty()
    {
      ReflectionBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", "CustomerID", typeof (ObjectID), false));

      return classDefinition;
    }
  }
}