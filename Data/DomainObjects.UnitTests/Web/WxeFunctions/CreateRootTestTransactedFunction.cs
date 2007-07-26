using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.Web.UnitTests.WxeFunctions
{
[Serializable]
public class CreateRootTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateRootTestTransactedFunction (ClientTransactionScope previousClientTransactionScope) 
      : base (WxeTransactionMode.CreateRoot, previousClientTransactionScope)
  {
  }

  // methods and properties

  [WxeParameter (1, true, WxeParameterDirection.In)]
  public ClientTransactionScope PreviousClientTransactionScope
  {
    get { return (ClientTransactionScope) Variables["PreviousClientTransactionScope"]; }
    set { Variables["PreviousClientTransactionScope"] = value; }
  }

  private void Step1 ()
  {
    Assert.AreNotSame (PreviousClientTransactionScope, ClientTransactionScope.ActiveScope);
  }

}
}
