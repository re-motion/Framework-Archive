using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public class Computer : TestDomainBase
  {
    // types

    // static members and constants

    public static new Computer GetObject (ObjectID id)
    {
      return (Computer) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Computer ()
    {
    }

    public Computer (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Computer (ClientTransaction clientTransaction, ObjectID objectID)
      : base(clientTransaction, objectID)
    {
    }

    protected Computer (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string SerialNumber
    {
      get { return DataContainer.GetString ("SerialNumber"); }
      set { DataContainer.SetValue ("SerialNumber", value); }
    }

    public Employee Employee
    {
      get { return (Employee) GetRelatedObject ("Employee"); }
      set { SetRelatedObject ("Employee", value); }
    }
  }
}
