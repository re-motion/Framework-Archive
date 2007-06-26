using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutIDColumn : TestDomainBase
  {
    // types

    // static members and constants

    public static new ClassWithoutIDColumn GetObject (ObjectID id)
    {
      return (ClassWithoutIDColumn) DomainObject.GetObject (id);
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
