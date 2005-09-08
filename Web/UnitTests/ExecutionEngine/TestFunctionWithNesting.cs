using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.Test.ExecutionEngine
{

public class TestFunctionWithNesting: WxeFunction
{
  public static readonly string Parameter1Name = "Parameter1";
  public static readonly string ReturnUrlValue = "DefaultReturn.html";

  private WxeContext _wxeContext;
  private string _lastExecutedStepID;

  public TestFunctionWithNesting()
	{
    ReturnUrl = TestFunction.ReturnUrlValue;
	}

	public TestFunctionWithNesting (params object[] args)
    : base (args)
	{
    ReturnUrl = TestFunction.ReturnUrlValue;
	}

  [WxeParameter (1, false, WxeParameterDirection.In)]
  public string Parameter1
  {
    get { return (string) Variables["Parameter1"]; }
    set { Variables["Parameter1"] = value; }
  }

  private void Step1()
  {
    _lastExecutedStepID = "1";
  }
  
  private void Step2()
  {
    _lastExecutedStepID = "2";
  }

  private class Step3: WxeFunction
  {
    public Step3()
	  {
	  }

    private void Step1()
    {
      TestFunctionWithNesting._lastExecutedStepID = "3.1";
    }
    
    private void Step2()
    {
      TestFunctionWithNesting._lastExecutedStepID = "3.2";
    }
    
    private void Step3_()
    {
      TestFunctionWithNesting._lastExecutedStepID = "3.3";
    }

    private TestFunctionWithNesting TestFunctionWithNesting
    {
      get { return (TestFunctionWithNesting) ParentStep; }
    }

  }
  
  private void Step4()
  {
    _lastExecutedStepID = "4";
  }

  public WxeContext WxeContext
  {
    get { return _wxeContext; }
  }

  public string LastExecutedStepID
  {
    get { return _lastExecutedStepID; }
  }

  public override void Execute (WxeContext context)
  {
    _wxeContext = context;
    base.Execute (context);
  }

}

}
