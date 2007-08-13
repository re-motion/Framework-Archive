using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
[Serializable]
  public class CreateRootWithChildTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateRootWithChildTestTransactedFunction (ClientTransaction previousClientTransaction, WxeFunction childFunction) 
      : base (WxeTransactionMode.CreateRoot, childFunction, previousClientTransaction)
  {
    Add (new WxeMethodStep (delegate() {
      TransactionAfterChild = ClientTransactionScope.CurrentTransaction;
      Assert.AreSame (TransactionBeforeChild, TransactionAfterChild);
    }));
  }

    public ClientTransaction TransactionBeforeChild;
    public ClientTransaction TransactionAfterChild;

  // methods and properties

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public ClientTransaction PreviousClientTransaction
  {
    get { return (ClientTransaction) Variables["PreviousClientTransaction"]; }
    set { Variables["PreviousClientTransaction"] = value; }
  }

  private void Step1 ()
  {
    Assert.AreNotSame (PreviousClientTransaction, ClientTransactionScope.CurrentTransaction);
    TransactionBeforeChild = ClientTransactionScope.CurrentTransaction;
  }

}
}
