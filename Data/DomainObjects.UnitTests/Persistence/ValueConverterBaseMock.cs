using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
public class ValueConverterBaseMock : ValueConverterBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ValueConverterBaseMock ()
  {
  }

  // methods and properties

  public new ClassDefinition GetOppositeClassDefinition (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
  {
    return base.GetOppositeClassDefinition (classDefinition, propertyDefinition);
  }

  public new object GetEnumValue (PropertyDefinition propertyDefinition, object dataValue)
  {
    return base.GetEnumValue (propertyDefinition, dataValue);
  }
}
}
