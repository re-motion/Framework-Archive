using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Globalization;
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
  ///   Actual parameters can either be constant values or variable names referring to callerVariables
  ///   using <see cref="WxeVariableReference"/>. Only in- and inout-parameters are copied.
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
        WxeVariableReference varRef = actualParameters[i] as WxeVariableReference;
        if (callerVariables != null && varRef != null)
          parameterDeclarations[i].CopyToCallee (varRef.Name, callerVariables, calleeVariables);
        else
          parameterDeclarations[i].CopyToCallee (actualParameters[i], calleeVariables);
      }
      else if (parameterDeclarations[i].Required)
      {
        throw new ApplicationException ("Parameter '" + parameterDeclarations[i].Name + "' is missing.");
      }
    }
  }

  public static void CopyToCallee (
      WxeParameterDeclaration[] parameterDeclarations, NameValueCollection parameters, NameObjectCollection calleeVariables, IFormatProvider format)
  {
    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      WxeParameterDeclaration paramDeclaration = parameterDeclarations[i];
      string strval = parameters[paramDeclaration.Name];
      if (strval != null)
        calleeVariables[paramDeclaration.Name] = Parse (paramDeclaration.Type, strval, paramDeclaration.Name, format);
      else if (paramDeclaration.Required)
        throw new ApplicationException ("Parameter '" + paramDeclaration.Name + "' is missing.");
    }
  }

  private static object Parse (Type type, string value, string parameterName, IFormatProvider format)
  {
    try
    {
      MethodInfo parseMethod = null;
      MethodInfo parseFormatMethod  = type.GetMethod (
          "Parse", 
          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, 
          null, 
          new Type[] {typeof (string), typeof (IFormatProvider)}, 
          null);

      if (parseFormatMethod != null && type.IsAssignableFrom (parseFormatMethod.ReturnType))
      {
        return parseFormatMethod.Invoke (null, new object[] { value, format } );
      }
      else
      {
        parseMethod  = type.GetMethod (
            "Parse", 
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, 
            null, 
            new Type[] {typeof (string)}, 
            null);

        if (parseMethod == null || ! type.IsAssignableFrom (parseMethod.ReturnType))
          throw new ApplicationException ("Cannot convert parameter '" + parameterName + "' to type " + type.Name + ". Type does not have method 'public static " + type.Name + " Parse (string s)'.");

        return parseMethod.Invoke (null, new object[] { value } );
      }
    }
    catch (TargetInvocationException e)
    {
      throw new ApplicationException ("Cannot convert parameter '" + parameterName + "' to type " + type.Name + ". Method 'Parse' failed.", e);
    }
  }

  /// <summary>
  ///   Take the parameter values of calleeVariables and pass them back to callerVariables.
  /// </summary>
  /// <remarks>
  ///   Actual parameters can either be constant values or variable names using 
  ///   <see cref="WxeVariableReference"/>.Only out- and inout-parameters referring to variable names are copied.
  /// </remarks>
  public static void CopyToCaller (
      WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters, 
      NameObjectCollection calleeVariables, NameObjectCollection callerVariables)
  {
    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      if (i < actualParameters.Length)
      {
        WxeVariableReference varRef = actualParameters[i] as WxeVariableReference;
        if (varRef != null)
          parameterDeclarations[i].CopyToCaller (varRef.Name, calleeVariables, callerVariables);
      }
    }
  }


  public static object[] ParseActualParameters (Type wxeFunctionType, string actualParameters, IFormatProvider format)
  {
    return ParseActualParameters (WxeFunction.GetParamaterDeclarations (wxeFunctionType), actualParameters, format);
  }

  public static object[] ParseActualParameters (Type wxeFunctionType, string actualParameters)
  {
    return ParseActualParameters (wxeFunctionType, actualParameters, CultureInfo.InvariantCulture);
  }

  /// <summary>
  ///   Parses a string of comma separated actual parameters.
  /// </summary>
  /// <remarks>
  ///   <list type="table">
  ///     <listheader>
  ///       <term> Type </term>
  ///       <description> Syntax </description>
  ///     </listheader>
  ///     <item>
  ///       <term> <see cref="String"/> </term>
  ///       <description> A quoted string. Escape quotes and line breaks using the backslash character.</description>
  ///     </item>
  ///     <item>
  ///       <term> Any type that has a <see langword="static"/> <c>Parse</c> method. </term>
  ///       <description> A quoted string that can be passed to the type's <c>Parse</c> method. </description>
  ///     </item>
  ///     <item>
  ///       <term> Variable Reference </term>
  ///       <description> An unquoted variable name. </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// <example>
  ///   "the first \"string\" argument, containing quotes and a comma", "true", "12/30/04 12:00", variableName
  /// </example>
  public static object[] ParseActualParameters (WxeParameterDeclaration[] parameterDeclarations, string actualParameters, IFormatProvider format)
  {
    StringBuilder current = new StringBuilder();
    ArrayList argsArray = new ArrayList();

    bool isQuoted = false;
    ArrayList isQuotedArray = new ArrayList();

    int len = actualParameters.Length;
    int state = 0; // 0 ... between arguments, 1 ... within argument, 2 ... within quotes
    for (int i = 0; i < len; ++i)
    {
      char c = actualParameters[i];
      if (state == 0)
      {
        switch (c)
        {
          case '\"':
            state = 2;
            isQuoted = true;
            break;
          case ' ':
            break;
          case ',':
            break;
          default:
            state = 1;
            current.Append (c);
            break;
        }
      }
      else if (state == 1)
      {
        switch (c)
        {
          case '\"':
            throw new ApplicationException ("Error at " + i + " while parsing " + actualParameters);
          case ',':
            state = 0;
            if (current.Length > 0)
            {
              argsArray.Add (current.ToString());
              current.Length = 0;
              isQuotedArray.Add (isQuoted);
              isQuoted = false;
            }
            break;
          default:
            current.Append (c);
            break;
        }
      }
      else if (state == 2)
      {
        switch (c)
        {
          case '\"':
            if ((i + 1) < len && actualParameters[i+1] != ',')
              throw new ApplicationException ("Error at " + i + " while parsing " + actualParameters);
            state = 1;
            break;
          case '\\':
            if ((i + 1) < len)
            {
              switch (actualParameters[i+1])
              {
                case '\\':
                  current.Append ('\\');
                  ++i;
                  break;
                case '\"':
                  current.Append ('\"');
                  ++i;
                  break;
                case '\'':
                  current.Append ('\'');
                  ++i;
                  break;
                case '\r':
                  current.Append ('\r');
                  ++i;
                  break;
                case '\n':
                  current.Append ('\n');
                  ++i;
                  break;
                default:
                  current.Append ('\\');
                  break;
              }
            }
            else
            {
              state = 1;
            }
            break;

          default:
            current.Append (c);
            break;
        }
      }
    }
    if (current.Length > 0)
    {
      argsArray.Add (current.ToString());
      isQuotedArray.Add (isQuoted);
    }

    if (argsArray.Count > parameterDeclarations.Length)
      throw new ApplicationException ("Number of actual parameters exceeds number of formal paramteres.");

    ArrayList arguments = new ArrayList();
    for (int i = 0; i < argsArray.Count; ++i)
    {
      string arg = (string) argsArray[i];
      WxeParameterDeclaration paramDecl = parameterDeclarations[i];

      if (! (bool) isQuotedArray[i])
        arguments.Add (new WxeVariableReference (arg));
      else if (paramDecl.Type == typeof (string))
        arguments.Add (arg);
      else 
        arguments.Add (Parse (paramDecl.Type, arg, paramDecl.Name, format));
    }

    return arguments.ToArray ();
  }

  public static object[] ParseActualParameters (WxeParameterDeclaration[] parameterDeclarations, string actualParameters)
  {
    return ParseActualParameters (parameterDeclarations, actualParameters, CultureInfo.InvariantCulture);
  }
}

public class WxeVariableReference
{
  private string _name;

  public WxeVariableReference (string variableName)
  {
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
