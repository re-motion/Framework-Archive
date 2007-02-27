using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutTimestampColumn : TestDomainBase
  {
    // types

    // static members and constants

    public static new ClassWithoutTimestampColumn GetObject (ObjectID id)
    {
      return (ClassWithoutTimestampColumn) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithoutTimestampColumn ()
    {
    }

    public ClassWithoutTimestampColumn (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected ClassWithoutTimestampColumn (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
