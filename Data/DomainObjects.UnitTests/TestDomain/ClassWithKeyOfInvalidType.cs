using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithKeyOfInvalidType")]
  public class ClassWithKeyOfInvalidType : TestDomainBase
  {
    // types

    // static members and constants

    public static new ClassWithKeyOfInvalidType GetObject (ObjectID id)
    {
      return (ClassWithKeyOfInvalidType) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithKeyOfInvalidType ()
    {
    }

    public ClassWithKeyOfInvalidType (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected ClassWithKeyOfInvalidType (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
