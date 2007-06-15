using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [Instantiable]
  public abstract class ClassWithLegacyLoadConstructor: DomainObject
  {
    protected ClassWithLegacyLoadConstructor ()
    {
    }

    protected ClassWithLegacyLoadConstructor (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}