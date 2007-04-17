using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
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

    protected Computer (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string SerialNumber
    {
      get { return (string) DataContainer.GetValue ("SerialNumber"); }
      set { DataContainer.SetValue ("SerialNumber", value); }
    }

    public Employee Employee
    {
      get { return (Employee) GetRelatedObject ("Employee"); }
      set { SetRelatedObject ("Employee", value); }
    }
  }
}
