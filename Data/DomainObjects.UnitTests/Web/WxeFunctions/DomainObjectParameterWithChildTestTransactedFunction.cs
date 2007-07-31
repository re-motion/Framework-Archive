using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class DomainObjectParameterWithChildTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildTestTransactedFunction ()
        : base (WxeTransactionMode.CreateRoot, new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateChildIfParent, null))
    {
      Insert (0, new WxeMethodStep (FirstStep));
      Add (new WxeMethodStep (LastStep));
    }

    private void FirstStep ()
    {
      Assert.AreSame (MyTransaction, ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2);
      inParameter.Int32Property = 47;

      childFunction.InParameter = inParameter;
    }

    private void LastStep ()
    {
      Assert.AreSame (MyTransaction, ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes outParameter = childFunction.OutParameter;

      Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.AreEqual (52, outParameter.Int32Property); // 47 + 5
    }
  }
}
