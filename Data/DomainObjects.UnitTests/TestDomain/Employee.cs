using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Employee : TestDomainBase
  {
    public static Employee GetObject (ObjectID id)
    {
      return (Employee) DomainObject.GetObject (id);
    }

    public static Employee Create ()
    {
      return DomainObject.Create<Employee> ();
    }

    protected Employee (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Supervisor")]
    public virtual ObjectList<Employee> Subordinates { get { return (ObjectList<Employee>) GetRelatedObjects(); } }

    [DBBidirectionalRelation ("Subordinates")]
    public abstract Employee Supervisor { get; set; }

    [DBBidirectionalRelation ("Employee")]
    public abstract Computer Computer { get; set; }

    public void DeleteWithSubordinates ()
    {
      foreach (Employee employee in Subordinates.Clone ())
        employee.Delete ();

      this.Delete ();
    }
  }
}
