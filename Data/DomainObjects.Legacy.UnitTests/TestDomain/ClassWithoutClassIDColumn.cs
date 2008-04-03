using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutClassIDColumn : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithoutClassIDColumn GetObject (ObjectID id)
    {
      return (ClassWithoutClassIDColumn) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithoutClassIDColumn ()
    {
    }

    protected ClassWithoutClassIDColumn (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
