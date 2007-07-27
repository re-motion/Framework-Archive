using System;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class ExecutionStartedFinishedTestFunction : WxeFunction
  {
    public bool ExecutionStartedCalled;
    public bool ExecutionFinishedCalled;

    public ExecutionStartedFinishedTestFunction ()
    {
      Assert.IsFalse (ExecutionStartedCalled);
      Assert.IsFalse (ExecutionFinishedCalled);
    }

    private void Step1 ()
    {
      Assert.IsTrue (ExecutionStartedCalled);
      Assert.IsFalse (ExecutionFinishedCalled);
    }

    private void Step2 (WxeContext context)
    {
      Assert.IsTrue (ExecutionStartedCalled);
      Assert.IsFalse (ExecutionFinishedCalled);
    }


    protected override void OnExecutionStarted ()
    {
      Assert.IsFalse (ExecutionStartedCalled);
      Assert.IsFalse (ExecutionFinishedCalled);
      ExecutionStartedCalled = true;
      base.OnExecutionStarted();
    }

    protected override void OnExecutionFinished ()
    {
      Assert.IsTrue (ExecutionStartedCalled);
      Assert.IsFalse (ExecutionFinishedCalled);
      ExecutionFinishedCalled = true;
      base.OnExecutionFinished ();
    }

    public void CheckNotExecutionFinishedCalled()
    {
      Assert.IsFalse (ExecutionFinishedCalled);
    }

    public void AddAdditionalCheckStep ()
    {
      Add (new WxeMethodStep (CheckNotExecutionFinishedCalled));
    }
  }
}