using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.Test.ExecutionEngine
{

public class TestFunction: WxeFunction
{
  public static readonly string Parameter1Name = "Parameter1";
  public static readonly string ReturnUrlValue = "DefaultReturn.html";

  private WxeContext _wxeContext;
  private int _lastExecutedStepNumber;

  public TestFunction()
	{
    ReturnUrl = TestFunction.ReturnUrlValue;
	}

	public TestFunction (params object[] args)
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
    _lastExecutedStepNumber = 1;
  }
  
  private void Step2()
  {
    _lastExecutedStepNumber = 2;
  }
  
  private void Step3()
  {
    _lastExecutedStepNumber = 3;
  }

  public WxeContext WxeContext
  {
    get { return _wxeContext; }
  }

  public int LastExecutedStepNumber
  {
    get { return _lastExecutedStepNumber; }
  }

  public override void Execute (WxeContext context)
  {
    _wxeContext = context;
    base.Execute (context);
  }

}

}
