using System;
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
  }
}
