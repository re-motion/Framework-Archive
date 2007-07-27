using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
[Serializable]
public class RemoveCurrentTransactionScopeFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RemoveCurrentTransactionScopeFunction () 
      : base (WxeTransactionMode.CreateRoot)
  {
  }

  // methods and properties

  private void Step1 ()
  {
    ClientTransactionScope.ActiveScope.Leave ();
  }

}
}
