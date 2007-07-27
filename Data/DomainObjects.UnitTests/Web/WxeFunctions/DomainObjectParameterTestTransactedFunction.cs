using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
[Serializable]
public class DomainObjectParameterTestTransactedFunction : WxeTransactedFunction
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DomainObjectParameterTestTransactedFunction (WxeTransactionMode transactionMode, ClassWithAllDataTypes inParameter) 
      : base (transactionMode, inParameter)
  {
  }

  // methods and properties

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public ClassWithAllDataTypes InParameter
  {
    get { return (ClassWithAllDataTypes) Variables["InParameter"]; }
    set { Variables["InParameter"] = value; }
  }

  [WxeParameter (2, false, WxeParameterDirection.Out)]
  public ClassWithAllDataTypes OutParameter
  {
    get { return (ClassWithAllDataTypes) Variables["OutParameter"]; }
    set { Variables["OutParameter"] = value; }
  }

  private void Step1 ()
  {
    Assert.IsTrue (ExecutionTransaction == null || ExecutionTransaction == ClientTransactionScope.CurrentTransaction);
    Assert.IsTrue (InParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

    OutParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes1);
    OutParameter.Int32Property = InParameter.Int32Property + 5;
  }
}
}
