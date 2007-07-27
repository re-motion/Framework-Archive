using System;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class EncapsulatedExecutionStartedFinishedTestFunction : WxeFunction
  {
    public SurroundingExecutionStartedFinishedTestFunction OuterFunction;

    public bool ExecutionStartedCalled;
    public bool ExecutionFinishedCalled;

    public EncapsulatedExecutionStartedFinishedTestFunction ()
    {
    }

    private void Step1 ()
    {
      Assert.IsTrue (OuterFunction.ExecutionStartedCalled);
      Assert.IsFalse (OuterFunction.ExecutionFinishedCalled);
      OuterFunction.Steps.Add ("Encapsulated.Step1");
    }

    protected override void OnExecutionStarted ()
    {
      Assert.IsTrue(OuterFunction.ExecutionStartedCalled);
      ExecutionStartedCalled = true;
      OuterFunction.Steps.Add ("Encapsulated.Started");
      base.OnExecutionStarted();
    }

    protected override void OnExecutionFinished ()
    {
      Assert.IsFalse (OuterFunction.ExecutionFinishedCalled);
      ExecutionFinishedCalled = true;
      OuterFunction.Steps.Add ("Encapsulated.Finished");
      base.OnExecutionFinished ();
    }
  }
}