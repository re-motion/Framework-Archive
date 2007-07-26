using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.Web.UnitTests.WxeFunctions
{
[Serializable]
public class CreateChildIfParentTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateChildIfParentTestTransactedFunction () 
      : base (WxeTransactionMode.CreateChildIfParent)
  {
  }

  // methods and properties

  private void Step1 ()
  {
    ITransaction parentTransaction = (ITransaction) PrivateInvoke.GetNonPublicProperty (ParentFunction, "Transaction");
    Assert.AreNotSame (parentTransaction, ClientTransactionScope.CurrentTransaction);
    Assert.AreSame (parentTransaction, ClientTransactionScope.CurrentTransaction.ParentTransaction);
  }

}
}
