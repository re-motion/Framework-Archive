using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.Test.ExecutionEngine
{

public class TestFunction: WxeFunction
{
  public static readonly string Parameter1Name = "Parameter1";
  public static readonly string ReturnUrlValue = "DefaultReturn.html";

  private string _lastExecutedStepID;

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
    _lastExecutedStepID = "1";
  }
  
  private void Step2()
  {
    _lastExecutedStepID = "2";
  }

  private TestStep Step3 = new TestStep();

  private void Step4()
  {
    _lastExecutedStepID = "4";
  }

  public string LastExecutedStepID
  {
    get { return _lastExecutedStepID; }
  }

  public TestStep TestStep
  {
    get { return Step3; }
  }
}

}
