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
  public class DomainObjectParameterWithChildInvalidInTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildInvalidInTestTransactedFunction ()
        : base (WxeTransactionMode.CreateRoot, new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateChildIfParent, null))
    {
      Insert (0, new WxeMethodStep (FirstStep));
    }

    private void FirstStep ()
    {
      Assert.AreSame (MyTransaction, ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2);
      inParameter.Delete ();
      childFunction.InParameter = inParameter;
    }
  }
}
