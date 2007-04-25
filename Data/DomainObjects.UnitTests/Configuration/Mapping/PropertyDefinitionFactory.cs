using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
public sealed class PropertyDefinitionFactory
{
  // types

  // static members and constants

  public static PropertyDefinition CreateCustomerDefinition ()
  {
    return new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName, true, false, null);
  }

  public static PropertyDefinition CreateWithUnresolvedRelationPropertyType ()
  {
    return new PropertyDefinition ("PropertyName", "StorageSpecificName", TypeInfo.ObjectIDMappingTypeName, false, false, null);
  }

  // member fields

  // construction and disposing

  private PropertyDefinitionFactory ()
  {
  }

  // methods and properties

}
}
