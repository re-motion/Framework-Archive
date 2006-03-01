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
    return new PropertyDefinition ("Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName, false, NaInt32.Null, true);
  }

  public static PropertyDefinition CreateWithUnresolvedRelationPropertyType ()
  {
    return new PropertyDefinition ("PropertyName", "ColumnName", TypeInfo.ObjectIDMappingTypeName, false, NaInt32.Null, false);
  }

  // member fields

  // construction and disposing

  private PropertyDefinitionFactory ()
  {
  }

  // methods and properties

}
}
