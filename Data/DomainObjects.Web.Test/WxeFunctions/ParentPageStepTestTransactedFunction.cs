using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class ParentPageStepTestTransactedFunction : WxeTransactedFunction
  {
    private ClientTransaction _transactionInStep1;

    private void Step1 ()
    {
      _transactionInStep1 = ClientTransactionScope.CurrentTransaction;
    }

    private NestedPageStepTestTransactedFunction Step2 = new NestedPageStepTestTransactedFunction ();

    private void Step3 ()
    {
      if (_transactionInStep1 != ClientTransactionScope.CurrentTransaction)
        throw new TestFailureException ("Transaction in parent function was not restored.");
    }
  }
}
