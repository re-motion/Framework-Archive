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
///   Performs a sequence of operations in a web application using named arguments.
/// </summary>
public abstract class WxeFunction: WxeStepList
{
  public static WxeParameterDeclaration[] GetParamaterDeclarations (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    if (! typeof (WxeFunction).IsAssignableFrom (type)) throw new ArgumentException ("Type " + type.FullName + " is not derived from WxeFunction.", "type");

    return GetParamaterDeclarationsUnchecked (type);
  }

  private static WxeParameterDeclaration[] GetParamaterDeclarationsUnchecked (Type type)
  {
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

  /// <summary> Hashtable&lt;Type, WxeParameterDeclaration[]&gt; </summary>
  private static Hashtable s_parameterDeclarations = new Hashtable();

  private NameObjectCollection _variables;

  private string _returnUrl;
  private object[] _actualParameters;

  public WxeFunction (params object[] actualParameters)
  {
    _variables = new NameObjectCollection(); // TODO: use a case sensitive collection
    _returnUrl = null;
    _actualParameters = actualParameters;
  }

  /// <summary> Take the actual parameters without any conversion. </summary>

  public override void Execute (WxeContext context)
  {
    if (! ExecutionStarted)
    {
      NameObjectCollection parentVariables = (ParentStep != null) ? ParentStep.Variables : null;
      WxeParameterDeclaration.CopyToCallee (ParameterDeclarations, _actualParameters, parentVariables, this.Variables);
    }

    base.Execute (context);

    if (_returnUrl != null)
    {
      // Variables.Clear();
      if (_returnUrl.StartsWith ("javascript:"))
      {
        context.HttpContext.Response.Clear();
        string script = _returnUrl.Substring ("javascript:".Length);
        context.HttpContext.Response.Write ("<html><script language=\"JavaScript\">" + script + "</script></html>");
        context.HttpContext.Response.End();
      }
      else
      {
        context.HttpContext.Response.Redirect (_returnUrl, true);
      }
    }

    if (ParentStep != null)
      WxeParameterDeclaration.CopyToCaller (ParameterDeclarations, _actualParameters, this.Variables, ParentStep.Variables);
    
    // Variables.Clear();

    if (ParentStep == null)
      context.HttpContext.Response.End();
  }

  public string ReturnUrl
  {
    get { return _returnUrl; }
    set { _returnUrl = value; }
  }

  public override NameObjectCollection Variables
  {
    get { return _variables; }
  }

  public virtual WxeParameterDeclaration[] ParameterDeclarations
  {
    get { return GetParamaterDeclarations (this.GetType()); }
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
      if (value is WxeVariableReference)
        sb.Append ("@" + ((WxeVariableReference)value).Name);
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
