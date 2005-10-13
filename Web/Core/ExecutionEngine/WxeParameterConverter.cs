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

public class WxeParameterConverter
{
  private WxeParameterDeclaration _parameter;

  public WxeParameterConverter (WxeParameterDeclaration parameter)
  {
    ArgumentUtility.CheckNotNull ("parameter", parameter);
    _parameter = parameter;
  }
  
  protected WxeParameterDeclaration Parameter
  {
    get { return _parameter; }
  }

  /// <summary> Converts a parameter's value to it's string representation. </summary>
  /// <param name="value"> The value to be converted. Must be of assignable to the <see cref="Type"/>. </param>
  /// <param name="callerVariables"> 
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> A string.</returns>
  /// <exception cref="WxeException"> Thrown if the <paramref="value"/> could not be converted. </exception>
  public string ConvertToString (object value, NameObjectCollection callerVariables)
  {
    CheckForRequiredOutParameter();

    WxeVariableReference varRef = value as WxeVariableReference;
    if (varRef != null)
      return ConvertVarRefToString (varRef, callerVariables);

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a <see cref="WxeVariableReference"/>'s value to it's string representation. </summary>
  /// <param name="value"> 
  ///   The <see cref="WxeVariableReference"/> to be converted. The referenced value must be of assignable to the 
  ///   <see cref="ParameterDeclaration"/>'s <see cref="Type"/>. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="callerVariables">
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> A string.</returns>
  /// <exception cref="WxeException"> Thrown if the <paramref="value"/> could not be converted. </exception>
  protected string ConvertVarRefToString (WxeVariableReference varRef, NameObjectCollection callerVariables)
  {
    ArgumentUtility.CheckNotNull ("varRef", varRef);

    if (callerVariables == null)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Requried IN parameter '{0}' is a Variable Reference but no caller variables have been provided.", 
            _parameter.Name));
      }
      return string.Empty;
    }

    object value = callerVariables[_parameter.Name];
    
    if (value is WxeVariableReference)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Requried IN parameter '{0}' is a Variable Reference but no caller variables have been provided.", 
            _parameter.Name));
      }
      return string.Empty;
    }

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a parameter's value to it's string representation. </summary>
  /// <param name="value"> The value to be converted. Must be of assignable to the <see cref="Type"/>. </param>
  /// <returns> A string.</returns>
  /// <exception cref="WxeException"> Thrown if the <paramref="value"/> could not be converted. </exception>
  protected virtual string ConvertObjectToString (object value)
  {
    if (value != null && ! _parameter.Type.IsAssignableFrom (value.GetType()))
      throw new ArgumentTypeException ("value", _parameter.Type, value.GetType());
 
    if (value == null)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Requried IN parameters cannot be converted to strings while they are null. Parameter: '{0}'", _parameter.Name));
      }
      return string.Empty;
    }

    if (_parameter.Type == typeof (string))
      return (string) value;

    //TypeConverter typeConverter = GetStringTypeConverter (_parameter.Type);
    //if (typeConverter != null)
    //  return typeConverter.ConvertToString (value);
    
    if (   StringUtility.GetParseMethodWithFormatProvider (_parameter.Type) != null
        || StringUtility.GetParseMethod (_parameter.Type) != null)
    {
      return value.ToString();
    }

    if (_parameter.Required)
    {
      throw new WxeException (string.Format (
          "Only parameters that can be restored from their string representation may be converted to a string. Parameter: '{0}'.",
          _parameter.Name));
    }

    return string.Empty;
  }

  protected void CheckForRequiredOutParameter ()
  {
    if (_parameter.Required && _parameter.Direction == WxeParameterDirection.Out)
    {
      throw new WxeException (string.Format (
          "Requried OUT parameters cannot be converted to a string. Parameter: '{0}'", _parameter.Name));
    }
  }
}

}
