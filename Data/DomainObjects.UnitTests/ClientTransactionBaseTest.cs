using System;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests
{
public class ClientTransactionBaseTest : DatabaseTest
{
  // types

  // static members and constants

  // member fields

  private ClientTransactionMock _clientTransactionMock;

  // construction and disposing

  protected ClientTransactionBaseTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _clientTransactionMock = new ClientTransactionMock ();
    ClientTransaction.SetCurrent (_clientTransactionMock);
  }

  [TearDown]
  public virtual void TearDown ()
  {
    ClientTransaction.SetCurrent (null);
  }

  protected ClientTransactionMock ClientTransactionMock
  {
    get { return _clientTransactionMock; }
  }
}
}
