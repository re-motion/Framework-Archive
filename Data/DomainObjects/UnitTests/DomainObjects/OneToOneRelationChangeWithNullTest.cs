using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class OneToOneRelationChangeWithNullTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public OneToOneRelationChangeWithNullTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();
  }

  [Test]
  public void OldRelatedObjectOfNewRelatedObjectIsNull ()
  {
    Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
    Computer newComputerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);

    employee.Computer = newComputerWithoutEmployee;

    // expectation: no exception
  }

  [Test]
  public void NewRelatedObjectIsNull ()
  {
    Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
    employee.Computer = null;

    // expectation: no exception
  }

  [Test]
  public void OldRelatedObjectIsNull ()
  {
    Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
    Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
    employeeWithoutComputer.Computer = computerWithoutEmployee;

    // expectation: no exception
  }

  [Test]
  public void SetRelatedObjectAndOldRelatedObjectIsNull ()
  {
    Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
    Employee employee = Employee.GetObject (DomainObjectIDs.Employee1);
    computerWithoutEmployee.Employee = employee;

    Assert.AreEqual (employee.ID, computerWithoutEmployee.DataContainer.GetObjectID ("Employee"));

    Assert.AreSame (employee, computerWithoutEmployee.Employee);
    Assert.AreSame (computerWithoutEmployee, employee.Computer);
  }

  [Test]
  public void SetRelatedObjectOverVirtualEndPointAndOldRelatedObjectIsNull ()
  {
    Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
    employeeWithoutComputer.Computer = computer;

    Assert.AreEqual (employeeWithoutComputer.ID, computer.DataContainer.GetObjectID ("Employee"));

    Assert.AreSame (computer, employeeWithoutComputer.Computer);
    Assert.AreSame (employeeWithoutComputer, computer.Employee);
  }

  [Test]
  public void SetNewRelatedObjectNull ()
  {
    Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
    Computer computer = employee.Computer;
    computer.Employee = null;

    Assert.IsNull (computer.DataContainer.GetObjectID ("Employee"));

    Assert.IsNull (computer.Employee);
    Assert.IsNull (employee.Computer);
  }

  [Test]
  public void SetNewRelatedObjectNullOverVirtualEndPoint ()
  {
    Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
    Computer computer = employee.Computer;
    employee.Computer = null;

    Assert.IsNull (computer.DataContainer.GetObjectID ("Employee"));

    Assert.IsNull (employee.Computer);
    Assert.IsNull (computer.Employee);
  }
}
}