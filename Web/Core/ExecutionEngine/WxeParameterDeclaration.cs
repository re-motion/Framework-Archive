using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// Specifies in which direction a parameter is passed.
/// </summary>
public enum WxeParameterDirection
{
  /// <summary>
  /// The parameter is passed from the caller to the callee.
  /// </summary>
  In,
  /// <summary>
  /// The parameter is returned from the callee to the caller.
  /// </summary>
  Out,
  /// <summary>
  /// The parameter is passed from the caller to the callee and returned back to the caller.
  /// </summary>
  InOut
}

/// <summary>
/// Declares a WXE parameter.
/// </summary>
[Serializable]
public class WxeParameterDeclaration
{
  private string _name;
  private bool _required;
  private WxeParameterDirection _direction;
  private Type _type;

  public WxeParameterDeclaration (string name, bool required, WxeParameterDirection direction, Type type)
  {
    _name = name;
    _required = required;
    _direction = direction;
    _type = type;
  }
  
  public string Name
  {
    get { return _name; }
  }

  public bool Required
  {
    get { return _required; }
  }

  public WxeParameterDirection Direction
  {
    get { return _direction; }
  }

  public Type Type
  {
    get { return _type; }
  }

  /// <summary>
  ///   Copy a single caller variable to a callee parameter.
  /// </summary>
  public void CopyToCallee (string actualParameterName, NameObjectCollection callerVariables, NameObjectCollection calleeVariables)
  {
    if (_direction != WxeParameterDirection.Out)
      CopyParameter (actualParameterName, callerVariables, _name, calleeVariables, _required);
  }

  /// <summary>
  ///   Copy a value to a callee parameter.
  /// </summary>
  public void CopyToCallee (object parameterValue, NameObjectCollection calleeVariables)
  {
    if (_direction == WxeParameterDirection.Out)
      throw new ApplicationException ("Constant provided for output parameter.");

    SetParameter (_name, parameterValue, calleeVariables); 
  }

  /// <summary>
  ///   Copy a single callee parameter back to a caller variable.
  /// </summary>
  public void CopyToCaller (string actualParameterName, NameObjectCollection calleeVariables, NameObjectCollection callerVariables)
  {
    if (_direction != WxeParameterDirection.In)
      CopyParameter (_name, calleeVariables, actualParameterName, callerVariables, false);
  }

  /// <summary>
  ///   Copy fromVariables[fromName] to toVariables[toName].
  /// </summary>
  private void CopyParameter (string fromName, NameObjectCollection fromVariables, string toName, NameObjectCollection toVariables, bool required)
  {
    object value = fromVariables[fromName];
    if (value == null && required)
      throw new ApplicationException ("Parameter '" + fromName + "' is missing.");
    SetParameter (toName, value, toVariables);
  }

  /// <summary>
  ///   Set the parameter variables[parameterName] to the specified value.
  /// </summary>
  private void SetParameter (string parameterName, object value, NameObjectCollection variables)
  {
    if (value != null && _type != null && ! _type.IsAssignableFrom (value.GetType()))
      throw new ApplicationException ("Parameter '" + parameterName + "' has unexpected type " + value.GetType().FullName + " (" + _type.FullName + " was expected).");
    variables[parameterName] = value;
  }

}

[Serializable]
public class WxeVariableReference
{
  private string _name;

  public WxeVariableReference (string variableName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("variableName", variableName);
    if (! System.Text.RegularExpressions.Regex.IsMatch (variableName, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
      throw new ArgumentException ("Invalid variable name", "variableName");
    _name = variableName;
  }

  public string Name
  {
    get { return _name; }
  }

  public override bool Equals (object obj)
  {
    WxeVariableReference other = obj as WxeVariableReference;
    if (other == null)
      return false;
    
    return this._name == other._name;
  }

  public override int GetHashCode()
  {
    return _name.GetHashCode();
  }
}

}
