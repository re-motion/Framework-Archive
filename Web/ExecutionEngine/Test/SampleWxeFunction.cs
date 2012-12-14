// SampleWxeFunction ()
// {
//   string Var1;
//   string Var2;
//   
//   Var1 = "SampleWxeFunction Step1";
//   Var2 = "Var2 - Step1";
// 
//   WxePageStep ("WebForm1.aspx");
// 
//   SampleWxeSubFunction (ref Var2, "constant for Var2");
// 
//   WxePageStep ("WebForm1.aspx");
// }
// 
// 
// SampleWxeSubFunction (ref string Var1, string Var2)
// {
//   try
//   {
//     WxePageStep ("WebForm1.aspx");
//  
//     Var1 = "SampleWxeSubFunction Step3";
// 
//     WxePageStep ("WebForm1.aspx");
// 
//     Var1 = "exit SampleWxeSubFunction";
//   }
//   catch (ApplicationException e)
//   {
//     if (e.Message != null && e.Message.Length > 0)
//     {
//       Var1 = e.Message;
//       WxePageStep ("WebForm1.aspx");
//     }
//
//     Var1 = "Exception caught.";
//     WxePageStep ("WebForm1.aspx");
//   }
//   finally 
//   {
//     Var2 = "finally";
//
//     WxePageStep ("WebForm1.aspx");
//   }
// }

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

  void Step1 ()
  {
    Var1 = "SampleWxeFunction Step1";
    Var2 = "Var2 - Step1";
  } 
  WxeStep Step2 = new WxePageStep ("WebForm1.aspx");
  WxeStep Step3 = new SampleWxeSubFunction (varref("Var2"), "constant for Var2");
  WxeStep Step4 = new WxePageStep ("WebForm1.aspx");
}

public class SampleWxeSubFunction: WxeFunction, ISampleFunctionVariables
{
  public SampleWxeSubFunction ()
  {
  }
  public SampleWxeSubFunction (params object[] args)
    : base (args)
  {
  }
  public SampleWxeSubFunction (string var1, string var2)
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

  class Step1: WxeTryCatch
  {
    class Try: WxeTryBlock
    {
      SampleWxeSubFunction Function { get { return (SampleWxeSubFunction) ParentFunction; } }

      void Step1 (WxeContext context)
      {
        // Var1 = "SampleWxeSubFunction Step1";
      }

      WxeStep Step2 = new WxePageStep ("WebForm1.aspx");

      void Step3 (WxeContext context)
      {
        Function.Var1 = "SampleWxeSubFunction Step3";
      }

      WxeStep Step4  = new WxePageStep ("WebForm1.aspx");

      void Step5 ()
      {
        Function.Var1 = "exit SampleWxeSubFunction";
      }    
    }

    [WxeException (typeof (ApplicationException))]
    class Catch1: WxeCatchBlock
    {
      SampleWxeSubFunction Function { get { return (SampleWxeSubFunction) ParentFunction; } }

      class Step1: WxeIf 
      {
        SampleWxeSubFunction Function { get { return (SampleWxeSubFunction) ParentFunction; } }

        bool If ()
        {
          return CurrentException.Message != null && CurrentException.Message.Length > 0;
        }
        class Then: WxeStepList
        {
          SampleWxeSubFunction Function { get { return (SampleWxeSubFunction) ParentFunction; } }

          void Step1()
          {
            Function.Var1 = CurrentException.Message;
          }
          WxeStep Step2 = new WxePageStep ("WebForm1.aspx");
        }
      }

      void Step2 (WxeContext context)
      {
        Function.Var1 = "Exception caught.";
      }

      WxeStep Step3 = new WxePageStep ("WebForm1.aspx");
    }

    class Finally: WxeFinallyBlock
    {
      SampleWxeSubFunction Function { get { return (SampleWxeSubFunction) ParentFunction; } }

      void Step1()
      {
        Function.Var2 = "finally";
      }
      WxeStep Step2 = new WxePageStep ("WebForm1.aspx");
    }
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
