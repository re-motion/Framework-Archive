using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Reflection;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
///   Performs a sequence of steps in a web application.
/// </summary>
public class WxeStepList: WxeStep
{
  /// <summary> ArrayList&lt;WxeStep&gt; </summary>
  private ArrayList _steps;

  private int _nextStep;
  private int _lastExecutedStep;

  public WxeStepList ()
  {
    _steps = new ArrayList();
    _nextStep = 0;
    _lastExecutedStep = -1;
    InitialzeSteps();
  }

  public override void Execute (WxeContext context)
  {
    for (int i = _nextStep; i < _steps.Count; ++i)
    {
      context.IsPostBack = (i == _lastExecutedStep);
      _lastExecutedStep = i;
      this[i].Execute (context);
      _nextStep = i + 1;
    }
  }

  public override void ExecuteNextStep (WxeContext context)
  {
    ++ _nextStep;
    RootFunction.Execute (context);
  }

  public WxeStep this[int index]
  {
    get { return (WxeStep) _steps[index]; }
  }

  public int Count
  {
    get { return _steps.Count; }
  }

  public void Add (WxeStep step)
  {
    _steps.Add (step);
    step.ParentStep = this;
  }

  public void Add (WxeMethod method)
  {
    Add (new WxeMethodStep (method));
  }

  public void Add (WxeMethodWithContext method)
  {
    Add (new WxeMethodStep (method));
  }

  public override WxeStep ExecutingStep
  {
    get
    {
      if (_lastExecutedStep < _steps.Count)
        return ((WxeStep) _steps[_lastExecutedStep]).ExecutingStep;
      else
        return this;
    }
  }

  public bool ExecutionStarted
  {
    get { return _lastExecutedStep >= 0; }
  }

  private void InitialzeSteps ()
  {
    Type type = this.GetType();
    MemberInfo[] members = NumberedMemberFinder.FindMembers (
        type, 
        "Step", 
        MemberTypes.Field | MemberTypes.Method | MemberTypes.NestedType, 
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

    foreach (MemberInfo member in members)
    {
      if (member is FieldInfo)
      {
        FieldInfo fieldInfo = (FieldInfo) member;
        Add ((WxeStep) fieldInfo.GetValue (this));
      }
      else if (member is MethodInfo)
      {
        MethodInfo methodInfo = (MethodInfo) member;
        if (methodInfo.GetParameters().Length == 0)
        {
          WxeMethod method = (WxeMethod) Delegate.CreateDelegate (typeof (WxeMethod), this, member.Name, false);
          Add (method);
        }
        else
        {
          WxeMethodWithContext method = (WxeMethodWithContext) Delegate.CreateDelegate (typeof (WxeMethodWithContext), this, member.Name, false);
          Add (method);
        }
      }
      else if (member is Type)
      {
        Type subtype = member as Type;
        if (typeof (WxeStep).IsAssignableFrom (subtype))
          Add ((WxeStep) Activator.CreateInstance (subtype));
      }
    }
  }
}

}