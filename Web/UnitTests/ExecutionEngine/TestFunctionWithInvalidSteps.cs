using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
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
