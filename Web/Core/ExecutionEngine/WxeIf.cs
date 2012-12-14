using System;
using System.Reflection;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// If-Then-Else block.
/// </summary>
public abstract class WxeIf: WxeStep
{
  WxeStepList _stepList = null; // represents Then or Else step list, depending on result of If()

  public override void Execute (WxeContext context)
  {
    Type type = this.GetType();
    if (_stepList == null)
    {
      MethodInfo ifMethod = type.GetMethod ("If", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
      if (ifMethod == null || ifMethod.ReturnType != typeof (bool))
        throw new ApplicationException ("If-block " + type.FullName + " does not define method \"bool If()\".");

      bool result = (bool) ifMethod.Invoke (this, new object[0]);
      if (result)
      {
        _stepList = GetResultList ("Then");
        if (_stepList == null)
          throw new ApplicationException ("If-block " + type.FullName + " does not define nested class \"Then\".");
      }
      else
      {
        _stepList = GetResultList ("Else");
      }
    }

    if (_stepList != null)
    {
      _stepList.Execute (context);
    }
  }

  private WxeStepList GetResultList (string name)
  {
    Type type = this.GetType();
    Type stepListType = type.GetNestedType (name, BindingFlags.Public | BindingFlags.NonPublic);
    if (stepListType == null)
      return null;
    if (! typeof (WxeStepList).IsAssignableFrom (stepListType))
      throw new ApplicationException ("Type " + stepListType.FullName + " must be derived from WxeStepList.");

    WxeStepList resultList = (WxeStepList) System.Activator.CreateInstance (stepListType);
    resultList.ParentStep = this;
    return resultList;
  }

  public override WxeStep ExecutingStep
  {
    get
    {
      if (_stepList == null)
        return null;
      else 
        return _stepList.ExecutingStep;
    }
  }

  protected override void Dispose (bool disposing)
  {
    if (disposing)
    {
      if (_stepList != null)
        _stepList.Dispose();
    }
  }

}

}
