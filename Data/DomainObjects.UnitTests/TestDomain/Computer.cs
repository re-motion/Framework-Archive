using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
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

  protected Computer (DataContainer dataContainer) : base (dataContainer)
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
