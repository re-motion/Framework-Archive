using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  public class ClassWithoutClassIDColumn : TestDomainBase
  {
    // types

    // static members and constants

    public static new ClassWithoutClassIDColumn GetObject (ObjectID id)
    {
      return (ClassWithoutClassIDColumn) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithoutClassIDColumn ()
    {
    }

    public ClassWithoutClassIDColumn (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected ClassWithoutClassIDColumn (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
