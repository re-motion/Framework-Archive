using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
public sealed class ClassDefinitionFactory
{
  // types

  // static members and constants

  public static ClassDefinition CreateOrderDefinition ()
  {
    return new ClassDefinition ("Order", "OrderTable", "StorageProviderID", typeof (Order));
  }

  public static ClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty ()
  {
    ClassDefinition classDefinition = CreateOrderDefinition ();
    classDefinition.MyPropertyDefinitions.Add (PropertyDefinitionFactory.CreateCustomerDefinition ());

    return classDefinition;
  }

  public static ClassDefinition CreateWithUnresolvedClassType ()
  {
    return new ClassDefinition ("ClassID", "Entity", "StorageProviderID", "UnresolvedType", false);
  }

  public static ClassDefinition CreateWithUnresolvedRelationProperty ()
  {
    ClassDefinition classDefinition = CreateWithUnresolvedClassType ();
    classDefinition.MyPropertyDefinitions.Add (PropertyDefinitionFactory.CreateWithUnresolvedRelationPropertyType ());

    return classDefinition;
  }

  // member fields

  // construction and disposing

  private ClassDefinitionFactory ()
  {
  }

  // methods and properties

}
}
