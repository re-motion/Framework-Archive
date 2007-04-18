using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectWithHierarchyTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectWithHierarchyTest ()
    {
    }

    // methods and properties

    [Test]
    public void GetObjectInHierarchy ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);

      Assert.IsNotNull (employee);
      Assert.AreEqual (DomainObjectIDs.Employee1, employee.ID);
    }

    [Test]
    public void GetChildren ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.IsNotNull (subordinates);
      Assert.AreEqual (2, subordinates.Count);
      Assert.IsNotNull (subordinates[DomainObjectIDs.Employee4]);
      Assert.IsNotNull (subordinates[DomainObjectIDs.Employee5]);
    }

    [Test]
    public void GetChildrenTwice ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.IsTrue (object.ReferenceEquals (subordinates, employee.Subordinates));
    }

    [Test]
    public void GetParent ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee4);
      Employee supervisor = employee.Supervisor;

      Assert.IsNotNull (supervisor);
      Assert.AreEqual (DomainObjectIDs.Employee1, supervisor.ID);
    }

    [Test]
    public void GetParentTwice ()
    {
      Employee employee1 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee4);
      Employee employee2 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee5);

      Assert.IsTrue (object.ReferenceEquals (employee1.Supervisor, employee2.Supervisor));
    }
  }
}
