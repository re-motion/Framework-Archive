using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.UnitTests.WxeFunctions
{
[Serializable]
public class CreateNoneTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CreateNoneTestTransactedFunction (ClientTransactionScope previousClientTransactionScope) 
      : base (WxeTransactionMode.None, previousClientTransactionScope)
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
    Assert.AreSame (PreviousClientTransactionScope, ClientTransactionScope.ActiveScope);
  }
}
}
