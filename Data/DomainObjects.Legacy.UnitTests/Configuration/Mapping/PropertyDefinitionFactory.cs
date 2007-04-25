using System;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
public sealed class PropertyDefinitionFactory
{
  // types

  // static members and constants

  public static PropertyDefinition CreateCustomerDefinition ()
  {
    return new XmlBasedPropertyDefinition ("Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName, true, false, null);
  }

  public static PropertyDefinition CreateWithUnresolvedRelationPropertyType ()
  {
    return new XmlBasedPropertyDefinition ("PropertyName", "StorageSpecificName", TypeInfo.ObjectIDMappingTypeName, false, false, null);
  }

  // member fields

  // construction and disposing

  private PropertyDefinitionFactory ()
  {
  }

  // methods and properties

}
}
