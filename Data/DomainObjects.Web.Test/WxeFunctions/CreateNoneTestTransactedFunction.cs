using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
public class CreateNoneTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateNoneTestTransactedFunction (ClientTransaction previousClientTransaction) 
      : base (TransactionMode.None, previousClientTransaction)
  {
  }

  // methods and properties

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public ClientTransaction PreviousClientTransaction
  {
    get { return (ClientTransaction) Variables["PreviousClientTransaction"]; }
    set { Variables["PreviousClientTransaction"] = value; }
  }

  private void Step1 ()
  {
    if (ClientTransaction.Current != PreviousClientTransaction)
      throw new TestFailureException ("The WxeTransactedFunction of the parent function was not properly used.");
  }

}
}
