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
/// Performs a sequence of operations in a web application, including execution of code and presentation of web pages.
/// </summary>
public abstract class WxeFunction: WxeStep
{
  /// <summary> Hashtable&lt;Type, WxeParameterDeclaration[]&gt; </summary>
  private static Hashtable s_parameterDeclarations = new Hashtable();

  private NameObjectCollection _variables;

  /// <summary> ArrayList&lt;WxeStep&gt; </summary>
  private ArrayList _steps;

  private int _nextStep;
  private int _lastExecutedStep;
  private string _returnUrl;
  private object[] _actualParameters;

  public WxeFunction (params object[] actualParameters)
  {
    _variables = new NameObjectCollection();
    _steps = new ArrayList();
    _nextStep = 0;
    _lastExecutedStep = -1;
    _returnUrl = null;
    _actualParameters = actualParameters;
    InitialzeSteps();
  }

  public override void Execute (WxeContext context)
  {
    if (ParentStep != null && _lastExecutedStep < 0)
      WxeParameterDeclaration.CopyToCallee (ParameterDeclarations, _actualParameters, ParentStep.Variables, this.Variables);

    for (int i = _nextStep; i < _steps.Count; ++i)
    {
      context.IsPostBack = (i == _lastExecutedStep);
      _lastExecutedStep = i;
      this[i].Execute (context);
      _nextStep = i + 1;
    }
    if (_returnUrl != null)
    {
      Variables.Clear();
      context.HttpContext.Response.Redirect (_returnUrl);
    }

    if (ParentStep != null)
      WxeParameterDeclaration.CopyToCaller (ParameterDeclarations, _actualParameters, this.Variables, ParentStep.Variables);
    
    Variables.Clear();
  }

  public override void ExecuteNextStep (WxeContext context)
  {
    ++ _nextStep;
    RootFunction.Execute (context);
  }

  public string ReturnUrl
  {
    get { return _returnUrl; }
    set { _returnUrl = value; }
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

  public override NameObjectCollection Variables
  {
    get { return _variables; }
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

  public WxeFunction RootFunction
  {
    get
    {
      WxeStep s = this;
      while (s.ParentStep != null)
        s = s.ParentStep;
      return s as WxeFunction;
    }
  }

  public virtual WxeParameterDeclaration[] ParameterDeclarations
  {
    get 
    {
      Type type = this.GetType();
      WxeParameterDeclaration[] declarations = (WxeParameterDeclaration[]) s_parameterDeclarations[type];
      if (declarations == null)
      {
        lock (type)
        {
          declarations = (WxeParameterDeclaration[]) s_parameterDeclarations[type];
          if (declarations == null)
          {
            PropertyInfo[] properties = type.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            ArrayList parameters = new ArrayList(); // ArrayList<WxeParameterDeclaration>
            ArrayList numbers = new ArrayList(); // ArrayList<int>
            foreach (PropertyInfo property in properties)
            {
              WxeParameterAttribute parameterAttribute = WxeParameterAttribute.GetAttribute (property);
              if (parameterAttribute != null)
              {
                parameters.Add (new WxeParameterDeclaration (
                    property.Name, parameterAttribute.Required, parameterAttribute.Direction, property.PropertyType));
                numbers.Add (parameterAttribute.Number);
              }
            }

            declarations = (WxeParameterDeclaration[]) parameters.ToArray (typeof (WxeParameterDeclaration));
            int[] numberArray = (int[]) numbers.ToArray (typeof (int));
            Array.Sort (numberArray, declarations);

            s_parameterDeclarations.Add (type, declarations);
          }
        }
      }
      return declarations;
    }
  }

  private void InitialzeSteps ()
  {
    Type type = this.GetType();
    MemberInfo[] members = type.FindMembers (
        MemberTypes.Field | MemberTypes.Method, 
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly, 
        new MemberFilter (PrefixMemberFilter), 
        null);

    int[] numbers = new int[members.Length];
    for (int i = 0; i < members.Length; ++i)
      numbers[i] = GetStepNumber (members[i].Name);
    Array.Sort (numbers, members);

    foreach (MemberInfo member in members)
    {
      if (member is FieldInfo)
      {
        FieldInfo fieldInfo = (FieldInfo) member;
        Add ((WxeStep) fieldInfo.GetValue (this));
      }
      else
      {
        WxeMethod method = (WxeMethod) Delegate.CreateDelegate (typeof (WxeMethod), this, member.Name, false);
        Add (method);
      }
    }
  }

  private bool PrefixMemberFilter (MemberInfo member, object param)
  {
    return GetStepNumber (member.Name) != -1;
  }

  private int GetStepNumber (string memberName)
  {
    const string prefix = "Step";
    if (! memberName.StartsWith (prefix))
      return -1;
    string numStr = memberName.Substring (prefix.Length);
    if (numStr.Length == 0)
      return -1;
    double num;
    if (! double.TryParse (numStr, System.Globalization.NumberStyles.Integer, null, out num))
      return -1;
    return (int) num;
  }

  public override string ToString()
  {
    StringBuilder sb = new StringBuilder();
    sb.Append (this.GetType().Name);
    sb.Append (" (");
    for (int i = 0; i < _actualParameters.Length; ++i)
    {
      if (i > 0)
        sb.Append (", ");
      object value = _actualParameters[i];
      string parameterName = WxeParameterDeclaration.GetParameterName (value);
      if (parameterName != null)
        sb.Append (parameterName);
      else if (value is string)
        sb.AppendFormat ("\"{0}\"", value);
      else
        sb.Append (value);
    }
    sb.Append (")");
    return sb.ToString();
  }
}

[AttributeUsage (AttributeTargets.Property, AllowMultiple = false)]
public class WxeParameterAttribute: Attribute
{
  private int _number;
  private bool _required;
  private WxeParameterDirection _direction;

  public WxeParameterAttribute (int number, WxeParameterDirection direction)
    : this (number, false, direction)
  {
  }

  public WxeParameterAttribute (int number, bool required)
    : this (number, required, WxeParameterDirection.In)
  {
  }

  public WxeParameterAttribute (int number)
    : this (number, false, WxeParameterDirection.In)
  {
  }

  public WxeParameterAttribute (int number, bool required, WxeParameterDirection direction)
  {
    _number = number;
    _required = required;
    _direction = direction;
  }

  public int Number
  {
    get { return _number; }
  }

  public bool Required
  {
    get { return _required; }
  }

  public WxeParameterDirection Direction
  {
    get { return _direction; }
  }

  public static WxeParameterAttribute GetAttribute (PropertyInfo property)
  {
    WxeParameterAttribute[] attributes = (WxeParameterAttribute[]) property.GetCustomAttributes (typeof (WxeParameterAttribute), false);
    if (attributes == null || attributes.Length == 0)
      return null;
    else
      return attributes[0];
  }
}

}
