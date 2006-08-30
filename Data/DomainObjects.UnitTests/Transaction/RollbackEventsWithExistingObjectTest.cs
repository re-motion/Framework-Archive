using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.MockConstraints;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class RollbackEventsWithExistingObjectTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private MockRepository _mockRepository;

    private ClientTransactionMockEventReceiver _clientTransactionMockEventReceiver;
    private IClientTransactionExtension _clientTransactionExtensionMock;

    private Order _order1;
    private DomainObjectMockEventReceiver _order1MockEventReceiver;

    private Customer _customer1;
    private DomainObjectMockEventReceiver _customer1MockEventReceiver;
    private string _orginalCustomerName;

    // construction and disposing

    public RollbackEventsWithExistingObjectTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _customer1 = _order1.Customer;
      _orginalCustomerName = _customer1.Name;

      _mockRepository = new MockRepository ();

      _clientTransactionMockEventReceiver = _mockRepository.CreateMock<ClientTransactionMockEventReceiver> (ClientTransactionMock);
      _clientTransactionExtensionMock = _mockRepository.CreateMock<IClientTransactionExtension> ();
      ClientTransactionMock.Extensions.Add ("MockExtension", _clientTransactionExtensionMock);

      _order1MockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_order1);
      _customer1MockEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_customer1);
    }

    [Test]
    public void RollbackWithoutChanges ()
    {
      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 0));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock);
        _clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock);

        _clientTransactionExtensionMock.RolledBack (null);
        LastCall.Constraints (Property.Value ("Count", 0));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RollbackWithDomainObject ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_order1));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _order1);

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock, _order1);

        _clientTransactionExtensionMock.RolledBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_order1));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());
        LastCall.Do (new EventHandler (ChangeCustomerNameCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (null, null, null, null);
        LastCall.IgnoreArguments ();

        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1 ));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _order1, _customer1);

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _customer1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock, _order1, _customer1);

        _clientTransactionExtensionMock.RolledBack (null);
        LastCall.Constraints (Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_order1));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _order1);
        LastCall.Do (new ClientTransactionEventHandler (ChangeCustomerNameCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (null, null, null, null);
        LastCall.IgnoreArguments ();

        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_customer1));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _customer1);

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _customer1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock, _order1, _customer1);

        _clientTransactionExtensionMock.RolledBack (null);
        LastCall.Constraints (Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_customer1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());
        LastCall.Do (new EventHandler (ChangeCustomerNameBackToOriginalCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (null, null, null, null);
        LastCall.IgnoreArguments ();

        // Note: Because .NET's event order is not deterministic, Customer1 should raise a RollingBack event.
        //       For further details see comment in ClientTransaction.BeginCommit.
        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_order1));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _order1);

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        // Note: Customer1 must not raise a RolledBack event, because its Name has been set back to the OriginalValue.

        _clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock, _order1);

        _clientTransactionExtensionMock.RolledBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_order1));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_customer1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null);
        LastCall.Constraints (Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1));

        _clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _order1, _customer1);
        LastCall.Do (new ClientTransactionEventHandler (ChangeCustomerNameBackToOriginalCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (null, null, null, null);
        LastCall.IgnoreArguments ();

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        // Note: Customer1 must not raise a RolledBack event, because its Name has been set back to the OriginalValue.

        _clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock, _order1);

        _clientTransactionExtensionMock.RolledBack (null);
        LastCall.Constraints (Property.Value ("Count", 1) & List.IsIn (_order1));
      }

      _mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      _mockRepository.VerifyAll ();
    }

    private void ChangeCustomerNameCallback (object sender, EventArgs e)
    {
      ChangeCustomerName ();
    }

    private void ChangeCustomerNameCallback (object sender, ClientTransactionEventArgs args)
    {
      ChangeCustomerName ();
    }

    private void ChangeCustomerName ()
    {
      _customer1.Name = "New customer name";
    }

    private void ChangeCustomerNameBackToOriginalCallback (object sender, EventArgs e)
    {
      ChangeCustomerNameBackToOriginal ();
    }

    private void ChangeCustomerNameBackToOriginalCallback (object sender, ClientTransactionEventArgs args)
    {
      ChangeCustomerNameBackToOriginal ();
    }

    private void ChangeCustomerNameBackToOriginal ()
    {
      _customer1.Name = _orginalCustomerName;
    }
  }
}
