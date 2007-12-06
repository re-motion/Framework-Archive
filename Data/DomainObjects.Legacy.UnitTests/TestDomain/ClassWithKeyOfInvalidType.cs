using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithKeyOfInvalidType : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithKeyOfInvalidType GetObject (ObjectID id)
    {
      return (ClassWithKeyOfInvalidType) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithKeyOfInvalidType ()
    {
    }

    protected ClassWithKeyOfInvalidType (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
