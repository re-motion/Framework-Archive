using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class OneToOneRelationChangeWithNullTest : ClientTransactionBaseTest
  {
    [Test]
    public void OldRelatedObjectOfNewRelatedObjectIsNull ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee3);
      Computer newComputerWithoutEmployee = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);

      employee.Computer = newComputerWithoutEmployee;

      // expectation: no exception
    }

    [Test]
    public void NewRelatedObjectIsNull ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee3);
      employee.Computer = null;

      // expectation: no exception
    }

    [Test]
    public void OldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      Computer computerWithoutEmployee = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      employeeWithoutComputer.Computer = computerWithoutEmployee;

      // expectation: no exception
    }

    [Test]
    public void SetRelatedObjectAndOldRelatedObjectIsNull ()
    {
      Computer computerWithoutEmployee = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      computerWithoutEmployee.Employee = employee;

      Assert.AreEqual (employee.ID, computerWithoutEmployee.DataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.AreSame (employee, computerWithoutEmployee.Employee);
      Assert.AreSame (computerWithoutEmployee, employee.Computer);
    }

    [Test]
    public void SetRelatedObjectOverVirtualEndPointAndOldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      Computer computer = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      employeeWithoutComputer.Computer = computer;

      Assert.AreEqual (employeeWithoutComputer.ID, computer.DataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.AreSame (computer, employeeWithoutComputer.Computer);
      Assert.AreSame (employeeWithoutComputer, computer.Employee);
    }

    [Test]
    public void SetNewRelatedObjectNull ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      computer.Employee = null;

      Assert.IsNull (computer.DataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.IsNull (computer.Employee);
      Assert.IsNull (employee.Computer);
    }

    [Test]
    public void SetNewRelatedObjectNullOverVirtualEndPoint ()
    {
      Employee employee = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee3);
      Computer computer = employee.Computer;
      employee.Computer = null;

      Assert.IsNull (computer.DataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));

      Assert.IsNull (employee.Computer);
      Assert.IsNull (computer.Employee);
    }
  }
}