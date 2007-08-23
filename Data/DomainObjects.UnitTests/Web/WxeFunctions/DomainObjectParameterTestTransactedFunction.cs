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

  public DomainObjectParameterTestTransactedFunction (WxeTransactionMode transactionMode, ClassWithAllDataTypes inParameter,
      ClassWithAllDataTypes[] inParameterArray) 
      : base (transactionMode, inParameter, inParameterArray)
  {
  }

  // methods and properties

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public ClassWithAllDataTypes InParameter
  {
    get { return (ClassWithAllDataTypes) Variables["InParameter"]; }
    set { Variables["InParameter"] = value; }
  }

  [WxeParameter (2, false, WxeParameterDirection.In)]
  public ClassWithAllDataTypes[] InParameterArray
  {
    get { return (ClassWithAllDataTypes[]) Variables["InParameterArray"]; }
    set { Variables["InParameterArray"] = value; }
  }

  [WxeParameter (3, false, WxeParameterDirection.Out)]
  public ClassWithAllDataTypes OutParameter
  {
    get { return (ClassWithAllDataTypes) Variables["OutParameter"]; }
    set { Variables["OutParameter"] = value; }
  }

  [WxeParameter (4, false, WxeParameterDirection.Out)]
  public ClassWithAllDataTypes[] OutParameterArray
  {
    get { return (ClassWithAllDataTypes[]) Variables["OutParameterArray"]; }
    set { Variables["OutParameterArray"] = value; }
  }

  private void Step1 ()
  {
    Assert.IsTrue (Transaction == null || Transaction == ClientTransactionScope.CurrentTransaction);
    Assert.IsTrue (InParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
    Assert.IsTrue (InParameterArray[0].CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

    OutParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes1);
    OutParameter.Int32Property = InParameter.Int32Property + 5;

    OutParameterArray = new ClassWithAllDataTypes[] { ClassWithAllDataTypes.GetObject (new DomainObjectIDs ().ClassWithAllDataTypes2) };
    OutParameterArray[0].Int32Property = InParameterArray[0].Int32Property + 5;
  }
}
}
