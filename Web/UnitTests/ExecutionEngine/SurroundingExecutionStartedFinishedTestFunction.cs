using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class SurroundingExecutionStartedFinishedTestFunction : WxeFunction
  {
    public EncapsulatedExecutionStartedFinishedTestFunction InnerFunction;

    public bool ExecutionStartedCalled;
    public bool ExecutionFinishedCalled;

    public List<string> Steps = new List<string> ();

    public SurroundingExecutionStartedFinishedTestFunction (EncapsulatedExecutionStartedFinishedTestFunction innerFunction)
    {
      InnerFunction = innerFunction;
      Add (new WxeMethodStep (FirstStep));
      Add (innerFunction);
      Add (new WxeMethodStep (LastStep));
      innerFunction.OuterFunction = this;
    }

    private void FirstStep ()
    {
      Assert.IsFalse (InnerFunction.ExecutionStartedCalled);
      Assert.IsFalse (InnerFunction.ExecutionFinishedCalled);
      Steps.Add ("Surrounding.First");
    }

    private void LastStep ()
    {
      Assert.IsTrue (InnerFunction.ExecutionStartedCalled);
      Assert.IsTrue (InnerFunction.ExecutionFinishedCalled);
      Steps.Add ("Surrounding.Last");
    }

    protected override void OnExecutionStarted ()
    {
      Assert.IsFalse (InnerFunction.ExecutionStartedCalled);
      ExecutionStartedCalled = true;
      Steps.Add ("Surrounding.Started");
      base.OnExecutionStarted();
    }

    protected override void OnExecutionFinished ()
    {
      Assert.IsTrue (InnerFunction.ExecutionFinishedCalled);
      ExecutionFinishedCalled = true;
      Steps.Add ("Surrounding.Finished");
      base.OnExecutionFinished ();
    }
  }
}