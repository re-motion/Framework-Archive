using System;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class ExecutionStartedFinishedTestTransactedFunction : WxeTransactedFunctionBase<TestTransaction>
  {
    public bool ExecutionStartedCalled;
    public bool ExecutionFinishedCalled;

    public ExecutionStartedFinishedTestTransactedFunction ()
    {
    }

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new WxeTransactionMock (null, false, true);
    }

    protected override void OnExecutionStarted ()
    {
      ExecutionStartedCalled = true;
      Assert.IsNotNull (OwnTransaction);
      base.OnExecutionStarted ();
    }

    protected override void OnExecutionFinished ()
    {
      ExecutionFinishedCalled = true;
      Assert.IsNotNull (OwnTransaction);
      base.OnExecutionFinished ();
    }
  }
}