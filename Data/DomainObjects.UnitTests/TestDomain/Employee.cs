using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public class Employee : TestDomainBase
  {
    // types

    // static members and constants

    public static new Employee GetObject (ObjectID id)
    {
      return (Employee) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Employee ()
    {
    }

    public Employee (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Employee (ClientTransaction clientTransaction, ObjectID objectID)
      : base(clientTransaction, objectID)
    {
    }

    protected Employee (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public DomainObjectCollection Subordinates
    {
      get { return GetRelatedObjects ("Subordinates"); }
    }

    public Employee Supervisor
    {
      get { return (Employee) GetRelatedObject ("Supervisor"); }
      set { SetRelatedObject ("Supervisor", value); }
    }

    public Computer Computer
    {
      get { return (Computer) GetRelatedObject ("Computer"); }
      set { SetRelatedObject ("Computer", value); }
    }

    public void DeleteWithSubordinates ()
    {
      DomainObjectCollection subordinates = (DomainObjectCollection) Subordinates.Clone ();
      foreach (Employee employee in subordinates)
        employee.Delete ();

      this.Delete ();
    }
  }
}
