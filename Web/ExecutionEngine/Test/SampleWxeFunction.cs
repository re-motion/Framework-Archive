using System;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.PageTransition
{

public class SampleWxeFunction: WxeFunction, ISampleFunctionVariables
{
  public SampleWxeFunction ()
  {
    ReturnUrl = "Start.aspx";
  }

  // parameters and local variables

  public string Var1
  {
    get { return (string) Variables["Var1"]; }
    set { Variables["Var1"] = value; }
  }

  public string Var2
  {
    get { return (string) Variables["Var2"]; }
    set { Variables["Var2"] = value; }
  }

  // steps

  private void Step1 ()
  {
    Var1 = "SampleWxeFunction Step1";
    Var2 = "Var2 - Step1";
  }

  private WxeStep Step2 = new WxePageStep ("WebForm1.aspx");

  private WxeStep Step3 = new SampleWxeSubFunction ("@Var2", "constant for Var2");

  private void Step4 (WxeContext context)
  {
    // Var1 = Var2;
    // Var1 = "SampleWxeFunction Step4";
  }

  private WxeStep Step5 = new WxePageStep ("WebForm1.aspx");
}

public class SampleWxeSubFunction: WxeFunction, ISampleFunctionVariables
{
  public SampleWxeSubFunction (object var1, object var2)
    : base (var1, var2)
  {
  }

  // parameters and local variables

  [WxeParameter (1, true, WxeParameterDirection.InOut)]
  public string Var1
  {
    get { return (string) Variables["Var1"]; }
    set { Variables["Var1"] = value; }
  }

  [WxeParameter (2, true, WxeParameterDirection.In)]
  public string Var2
  {
    get { return (string) Variables["Var2"]; }
    set { Variables["Var2"] = value; }
  }

  // steps

  private WxeStep Step1 = new WxeTryCatch (typeof (Try), typeof (CatchApplicationExecption));

  private class Try: WxeStepList
  {
    private SampleWxeSubFunction Function 
    {
      get { return (SampleWxeSubFunction) ParentFunction; }
    }

    private void Step1 (WxeContext context)
    {
      // Var1 = "SampleWxeSubFunction Step1";
    }

    private WxeStep Step2 = new WxePageStep ("WebForm1.aspx");

    private void Step3 (WxeContext context)
    {
      Function.Var1 = "SampleWxeSubFunction Step3";
    }

    private WxeStep Step4  = new WxePageStep ("WebForm1.aspx");

    private void Step5 (WxeContext context)
    {
      Function.Var1 = "exit SampleWxeSubFunction";
      Function.Var2 = "this should never appear";
    }    
  }

  [WxeException (typeof (ApplicationException))]
  private class CatchApplicationExecption: WxeCatchBlock
  {
    private SampleWxeSubFunction Function 
    {
      get { return (SampleWxeSubFunction) ParentFunction; }
    }

    void Step1 (WxeContext context)
    {
      Function.Var1 = "Exception var1";
    }

    WxeStep Step2 = new WxePageStep ("WebForm1.aspx");
  }
}

/// <summary>
/// This interface exists so that WebForm1.aspx can access both SampleWxeFunction and 
/// SampleWxeSubFunction variables in a type safe way.
/// Outside of demo scenarios, this would usually not make sense.
/// </summary>
public interface ISampleFunctionVariables
{
  string Var1 { get; set; }
  string Var2 { get; set; }
}
 
}
