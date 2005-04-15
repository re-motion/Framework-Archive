using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Reflection;
using System.Text;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.Collections;
using log4net;

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

  private static ILog s_log = LogManager.GetLogger (typeof (WxeFunction));

  private NameObjectCollection _variables;

  private string _returnUrl;
  private object[] _actualParameters;
  private bool _parametersInitialized = false;

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
      s_log.Debug ("Initializing execution of " + this.GetType().FullName + ".");
      NameObjectCollection parentVariables = (ParentStep != null) ? ParentStep.Variables : null;
      EnsureParametersInitialized (null);
    }

    base.Execute (context);

    if (ParentStep != null)
      ReturnParametersToCaller();
    s_log.Debug ("Ending Execution of " + this.GetType().FullName + ".");
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

  /// <summary> Initalizes parameters by position. </summary>
  /// <remarks> Remove obsolete and make public if this method should be required. </remarks>
  [Obsolete]
  private void InitializeParameters (object[] parameters)
  {
    CheckParametersNotInitialized();
    WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
    if (parameters.Length > parameterDeclarations.Length)
      throw new ApplicationException (string.Format ("{0} parameters provided but only {1} were expected.", parameters.Length, parameterDeclarations.Length));

    for (int i = 0; i < parameters.Length; i++)
    {
      WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
      object parameter = parameters[i];
      _variables[parameterDeclaration.Name] = parameter;
    }

    for (int i = parameters.Length; i < parameterDeclarations.Length; i++)
    {
      WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
      if (parameterDeclaration.Required)
        throw new ApplicationException ("Parameter '" + parameterDeclaration.Name + "' is missing.");
    }

    _parametersInitialized = true; // since parameterString may not contain variable references, initialization is done right away
  }

  public void InitializeParameters (NameValueCollection parameters)
  {
    CheckParametersNotInitialized();

    WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;

    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      WxeParameterDeclaration paramDeclaration = parameterDeclarations[i];
      string strval = parameters[paramDeclaration.Name];
      if (strval != null)
      {
        try 
        {
          _variables[paramDeclaration.Name] = StringUtility.Parse (paramDeclaration.Type, strval, CultureInfo.InvariantCulture);
        }
        catch (ParseException e)
        {
          throw new ApplicationException ("Parameter " + paramDeclaration.Name + ": " + e.Message, e);
        }
      }
      else if (paramDeclaration.Required)
      {
        throw new ApplicationException ("Parameter '" + paramDeclaration.Name + "' is missing.");
      }
    }

    _parametersInitialized = true; // since parameterString may not contain variable references, initialization is done right away
  }

  public void InitializeParameters (string parameterString, bool delayInitialization)
  {
    InitializeParameters (parameterString, null, delayInitialization);
  }

  public void InitializeParameters (string parameterString, NameObjectCollection additionalParameters)
  {
    InitializeParameters (parameterString, additionalParameters, false);
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
  ///       <description> A quoted string that can be passed to the type's <c>Parse</c> method. For boolean constants 
  ///         (&quot;true&quot;, &quot;false&quot;) and numeric constants, quotes are optional.  </description>
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
  private void InitializeParameters (string parameterString, NameObjectCollection additionalParameters, bool delayInitialization)
  {
    CheckParametersNotInitialized();

    _actualParameters = ParseActualParameters (ParameterDeclarations, parameterString, CultureInfo.InvariantCulture);
  
    if (! delayInitialization)
      EnsureParametersInitialized (additionalParameters);
  }

  private static object[] ParseActualParameters (WxeParameterDeclaration[] parameterDeclarations, string actualParameters, IFormatProvider format)
  {
    StringUtility.ParsedItem[] parsedItems = StringUtility.ParseSeparatedList (actualParameters, ',');

    if (parsedItems.Length > parameterDeclarations.Length)
      throw new ApplicationException ("Number of actual parameters exceeds number of formal paramteres.");

    ArrayList arguments = new ArrayList();
    for (int i = 0; i < parsedItems.Length; ++i)
    {
      StringUtility.ParsedItem item = parsedItems[i];
      WxeParameterDeclaration paramDecl = parameterDeclarations[i];

      try
      {
        if (item.IsQuoted)
        {
          if (paramDecl.Type == typeof (string))                              // string constant
            arguments.Add (item.Value);
          else                                                                // parse constant
            arguments.Add (StringUtility.Parse (paramDecl.Type, item.Value, format));
        }
        else
        {
          if (string.CompareOrdinal (item.Value, "true") == 0)                // true
            arguments.Add (true);
          else if (string.CompareOrdinal (item.Value, "false") == 0)          // false
            arguments.Add (false);
          else if (item.Value.Length > 0 && char.IsDigit (item.Value[0]))     // starts with digit -> parse constant
            arguments.Add (StringUtility.Parse (paramDecl.Type, item.Value, format));
          else                                                                // variable name
            arguments.Add (new WxeVariableReference (item.Value));
        }
      }
      catch (ParseException e)
      {
        throw new ApplicationException ("Parameter " + paramDecl.Name + ": " + e.Message, e);
      }
    }

    return arguments.ToArray ();
  }

  /// <summary> Pass actualParameters to Variables. </summary>
  private void EnsureParametersInitialized (NameObjectCollection additionalParameters)
  {
    if (_parametersInitialized)
      return;

    WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
    NameObjectCollection callerVariables = (ParentStep != null) ? ParentStep.Variables : null;
    callerVariables = NameObjectCollection.Merge (callerVariables, additionalParameters);

    if (_actualParameters.Length > parameterDeclarations.Length)
      throw new ApplicationException (string.Format ("{0} parameters provided but only {1} were expected.", _actualParameters.Length, parameterDeclarations.Length));

    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      if (i < _actualParameters.Length && _actualParameters[i] != null)
      {
        WxeVariableReference varRef = _actualParameters[i] as WxeVariableReference;
        if (callerVariables != null && varRef != null)
          parameterDeclarations[i].CopyToCallee (varRef.Name, callerVariables, _variables);
        else
          parameterDeclarations[i].CopyToCallee (_actualParameters[i], _variables);
      }
      else if (parameterDeclarations[i].Required)
      {
        throw new ApplicationException ("Parameter '" + parameterDeclarations[i].Name + "' is missing.");
      }
    }

    _parametersInitialized = true;
  }

  private void ReturnParametersToCaller()
  {
    WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
    NameObjectCollection callerVariables = ParentStep.Variables;

    for (int i = 0; i < parameterDeclarations.Length; ++i)
    {
      if (i < _actualParameters.Length)
      {
        WxeVariableReference varRef = _actualParameters[i] as WxeVariableReference;
        if (varRef != null)
          parameterDeclarations[i].CopyToCaller (varRef.Name, _variables, callerVariables);
      }
    }
  }

  private void CheckParametersNotInitialized()
  {
    if (_parametersInitialized) 
      throw new InvalidOperationException ("Parameters are already initialized.");
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
