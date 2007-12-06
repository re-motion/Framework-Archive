using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutTimestampColumn : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithoutTimestampColumn GetObject (ObjectID id)
    {
      return (ClassWithoutTimestampColumn) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithoutTimestampColumn ()
    {
    }

    protected ClassWithoutTimestampColumn (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
