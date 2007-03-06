using System;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.MockConstraints;
using Rubicon.Utilities;
using Mocks_Is = Rhino.Mocks.Is;
using Mocks_List = Rhino.Mocks.List;
using Mocks_Property = Rhino.Mocks.Property;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class ClientTransactionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ClientTransactionMockEventReceiver (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      clientTransaction.Loaded += new ClientTransactionEventHandler (Loaded);
      clientTransaction.Committing += new ClientTransactionEventHandler (Committing);
      clientTransaction.Committed += new ClientTransactionEventHandler (Committed);
      clientTransaction.RollingBack += new ClientTransactionEventHandler (RollingBack);
      clientTransaction.RolledBack += new ClientTransactionEventHandler (RolledBack);
    }

    // abstract methods and properties

    public abstract void Loaded (object sender, ClientTransactionEventArgs args);
    public abstract void Committing (object sender, ClientTransactionEventArgs args);
    public abstract void Committed (object sender, ClientTransactionEventArgs args);
    public abstract void RollingBack (object sender, ClientTransactionEventArgs args);
    public abstract void RolledBack (object sender, ClientTransactionEventArgs args);

    // methods and properties

    public void RollingBack (object sender, params DomainObject[] domainObjects)
    {
      RollingBack (null, (ClientTransactionEventArgs) null);

      LastCall.Constraints (
          Mocks_Is.Same (sender),
          Mocks_Property.ValueConstraint ("DomainObjects",
              Mocks_Property.Value ("Count", domainObjects.Length)
              & new ContainsConstraint (domainObjects)));
    }

    public void RolledBack (object sender, params DomainObject[] domainObjects)
    {
      RolledBack (null, (ClientTransactionEventArgs) null);

      LastCall.Constraints (
          Mocks_Is.Same (sender),
          Mocks_Property.ValueConstraint ("DomainObjects",
              Mocks_Property.Value ("Count", domainObjects.Length)
              & new ContainsConstraint (domainObjects)));
    }
  }
}
