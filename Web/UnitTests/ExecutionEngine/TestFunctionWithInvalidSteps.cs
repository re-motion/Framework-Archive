using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.Test.ExecutionEngine
{

public class TestFunctionWithInvalidSteps: WxeFunction
{
  public TestFunctionWithInvalidSteps()
	{
	}

	public TestFunctionWithInvalidSteps (params object[] args)
    : base (args)
	{
	}

  static void InvalidStep1 ()
  {
  }

  void InvalidStep2 (object obj)
  {
  }

  void InvalidStep3 (WxeContext context, object obj)
  {
  }
}

}
