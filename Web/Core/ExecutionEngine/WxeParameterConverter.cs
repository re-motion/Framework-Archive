using System;
using System.ComponentModel;
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
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  public string ConvertToString (object value, NameObjectCollection callerVariables)
  {
    CheckForRequiredOutParameter();

    WxeVariableReference varRef = value as WxeVariableReference;
    if (varRef != null)
      return ConvertVarRefToString (varRef, callerVariables);

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a <see cref="WxeVariableReference"/>'s value to it's string representation. </summary>
  /// <param name="varRef"> 
  ///   The <see cref="WxeVariableReference"/> to be converted. The referenced value must be of assignable to the 
  ///   <see cref="WxeParameterDeclaration"/>'s <see cref="Type"/>. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="callerVariables">
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> A string.</returns>
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
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
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  protected virtual string ConvertObjectToString (object value)
  {
    if (value != null && ! _parameter.Type.IsAssignableFrom (value.GetType()))
      throw new ArgumentTypeException ("value", _parameter.Type, value.GetType());
 
    value = TryConvertNullToString (value);
    if (value is string)
      return (string) value;

    value = TryConvertStringToString (value);
    if (value is string)
      return (string) value;

    //TypeConverter typeConverter = GetStringTypeConverter (_parameter.Type);
    //if (typeConverter != null)
    //  return typeConverter.ConvertToString (value);
    
    //TODO: #if DEBUG
    value = TryConvertObjectToStringForParseMethod (value);
    if (value is string)
      return (string) value;

    if (_parameter.Required)
    {
      throw new WxeException (string.Format (
          "Only parameters that can be restored from their string representation may be converted to a string. Parameter: '{0}'.",
          _parameter.Name));
    }
    return string.Empty;
  }

  protected object TryConvertStringToString (object value)
  {
    if (_parameter.Type != typeof (string))
      return value;
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (string));

    if (_parameter.Required && ((string) value) == string.Empty)
    {
      throw new WxeException (string.Format (
          "Requried IN parameters of type String may not be empty. Parameter: '{0}'", _parameter.Name));
    }
    return (string) value;
  }

  protected object TryConvertObjectToStringForParseMethod (object value)
  {
    if (value == null)
      return value;

    if (StringUtility.HasParseMethod (_parameter.Type))
      return value.ToString();
    return value;
  }

  protected object TryConvertNullToString (object value)
  {
    if (value == null)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Requried IN parameters cannot be converted to strings while they are null. Parameter: '{0}'", _parameter.Name));
      }
      return string.Empty;
    }
    return value;
  }

  protected void CheckForRequiredOutParameter ()
  {
    if (_parameter.Required && _parameter.Direction == WxeParameterDirection.Out)
    {
      throw new WxeException (string.Format (
          "Requried OUT parameters cannot be converted to a string. Parameter: '{0}'", _parameter.Name));
    }
  }

  protected TypeConverter GetStringTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    TypeConverterAttribute[] typeConverters = 
        (TypeConverterAttribute[]) type.GetCustomAttributes (typeof (TypeConverterAttribute), true);
    if (typeConverters.Length == 1) 
    {
      Type typeConverterType = Type.GetType (typeConverters[0].ConverterTypeName, true, false);
      TypeConverter typeConverter = (TypeConverter) Activator.CreateInstance (typeConverterType);
      if (typeConverter.CanConvertTo (typeof (string)) && typeConverter.CanConvertFrom (typeof (string)))
        return typeConverter;
    }
    return null;
  }
}

}
