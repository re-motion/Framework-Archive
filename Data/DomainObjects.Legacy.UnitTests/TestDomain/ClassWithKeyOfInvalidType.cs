using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
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
