using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
  public class StubValueConverterBase : ValueConverterBase
  {
    public StubValueConverterBase (TypeConversionProvider typeConversionProvider)
        : base(typeConversionProvider)
    {
    }

    public new object GetEnumValue (PropertyDefinition propertyDefinition, object dataValue)
    {
      return base.GetEnumValue (propertyDefinition, dataValue);
    }
  }
}
