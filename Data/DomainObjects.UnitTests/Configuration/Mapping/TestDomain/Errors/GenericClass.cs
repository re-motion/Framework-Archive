using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class GenericClass<T>: DomainObject
  {
    protected GenericClass ()
    {
    }
  }
}