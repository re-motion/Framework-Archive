using System;
using Rubicon.Collections;

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

  /// <summary>
  ///   Take the values of actualParameters and pass them to calleeVariables.
  /// </summary>
  /// <remarks>
  ///   Actual parameters can either be constant values or variable names referring to callerVariables. Use strings with the '@' prefix character to
  ///   reference variables in callerVariables. Only in- and inout-parameters are copied.
  /// </remarks>
  public static void CopyToCallee (
      WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters, 
      NameObjectCollection callerVariables, NameObjectCollection calleeVariables)
  {
    if (actualParameters.Length > parameterDeclarations.Length)
      throw new ApplicationException (string.Format ("{0} parameters provided but only {1} were expected.", actualParameters.Length, parameterDeclarations.Length));

    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      if (i < actualParameters.Length && actualParameters[i] != null)
      {
        string parameterName = GetParameterName (actualParameters[i]);
        if (parameterName != null)
          parameterDeclarations[i].CopyToCallee (parameterName, callerVariables, calleeVariables);
        else
          parameterDeclarations[i].CopyToCallee (actualParameters[i], calleeVariables);
      }
      else if (parameterDeclarations[i].Required)
      {
        throw new ApplicationException ("Parameter '" + parameterDeclarations[i].Name + "' is missing.");
      }
    }
  }

  /// <summary>
  ///   Take the parameter values of calleeVariables and pass them back to callerVariables.
  /// </summary>
  /// <remarks>
  ///   Actual parameters can either be constant values or variable names referring to callerVariables. Use strings with the '@' prefix character to
  ///   reference variables in callerVariables. Only out- and inout-parameters referring to variable names are copied.
  /// </remarks>
  public static void CopyToCaller (
      WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters, 
      NameObjectCollection calleeVariables, NameObjectCollection callerVariables)
  {
    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      if (i < actualParameters.Length)
      {
        string parameterName = GetParameterName (actualParameters[i]);
        if (parameterName != null)
          parameterDeclarations[i].CopyToCaller (parameterName, calleeVariables, callerVariables);
      }
    }
  }

  /// <summary>
  ///   If actualParameterValue is a string like "@param", "param" is returned. Otherwise, null is returned.
  /// </summary>
  internal static string GetParameterName (object actualParameterValue)
  {
    string valueString = actualParameterValue as string;
    if (valueString != null && valueString.Length > 0 && valueString[0] == '@')
      return valueString.Substring (1);
    else 
      return null;
  }
}

}
