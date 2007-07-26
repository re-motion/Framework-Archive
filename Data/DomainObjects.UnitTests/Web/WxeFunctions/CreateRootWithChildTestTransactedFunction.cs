using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.Web.UnitTests.WxeFunctions
{
[Serializable]
public class CreateRootWithChildTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateRootWithChildTestTransactedFunction (ClientTransaction previousClientTransaction) 
      : base (WxeTransactionMode.CreateRoot, previousClientTransaction)
  {
    Add (new CreateChildIfParentTestTransactedFunction ());
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
    Assert.AreNotSame (PreviousClientTransaction, ClientTransactionScope.CurrentTransaction);
  }

}
}
