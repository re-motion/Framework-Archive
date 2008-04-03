using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutIDColumn : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithoutIDColumn GetObject (ObjectID id)
    {
      return (ClassWithoutIDColumn) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithoutIDColumn ()
    {
    }

    protected ClassWithoutIDColumn (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
