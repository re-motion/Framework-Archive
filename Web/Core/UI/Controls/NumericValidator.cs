using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

/// <summary> Summary validates the text for a valid numeric value. </summary>
/// <remarks> Validation is performed using the respective data type's <b>Parse</b> method. </remarks>
public class NumericValidator : BaseValidator
{
  private NumericValidationDataType _dataType = NumericValidationDataType.Integer;
  private bool _allowNegative = true;
  private NumberStyles _numberStyle = NumberStyles.None;

  // static members and constants

  // member fields

  // construction and disposing

  public NumericValidator()
  {
    EnableClientScript = false;
  }

  // methods and properties

  protected override bool EvaluateIsValid()
  {
    string text = GetControlValidationValue (ControlToValidate);

    if (StringUtility.IsNullOrEmpty (text))
      return true;

    switch (_dataType)
    {
      case NumericValidationDataType.Integer:
      {
        return IsInteger (text);
      }
      case NumericValidationDataType.Double:
      {
        return IsDouble (text);
      }
      case NumericValidationDataType.Currency:
      {
        return IsCurrency (text);
      }
    }
    return true;
  }

  protected bool IsInteger (string text)
  {
    int parsedValue = 0;
    
    try
    {
      if (_numberStyle != NumberStyles.None)
        parsedValue = int.Parse (text, _numberStyle);
      else
        parsedValue = int.Parse (text);
    }
    catch (ArgumentException e)
    {
      throw new ApplicationException ("The combination of the flags in the 'NumberStyle' property is invalid.", e);
    }
    catch (FormatException)
    {
      return false;
    }
    catch (OverflowException)
    {
      return false;
    }

    if (!_allowNegative && parsedValue < 0)
      return false;

    return true;
  }

  protected bool IsDouble (string text)
  {
    double parsedValue = 0d;
    
    try
    {
      if (_numberStyle != NumberStyles.None)
        parsedValue = double.Parse (text, _numberStyle);
      else
        parsedValue = double.Parse (text);
    }
    catch (FormatException)
    {
      return false;
    }
    catch (OverflowException)
    {
      return false;
    }

    if (!_allowNegative && parsedValue < 0d)
      return false;

    return true;
  }

  protected bool IsCurrency (string text)
  {
    decimal parsedValue = 0m;
    
    try
    {
      if (_numberStyle != NumberStyles.None)
        parsedValue = decimal.Parse (text, _numberStyle);
      else
        parsedValue = decimal.Parse (text);
    }
    catch (FormatException)
    {
      return false;
    }
    catch (OverflowException)
    {
      return false;
    }

    if (!_allowNegative && parsedValue < 0m)
      return false;

    return true;
  }
  
  /// <summary> Gets or sets the data type to be tested. </summary>
  /// <value> A value of the <see cref="NumericValidationDataType"/> enumeration. Defaults to <b>Integer</b>. </value>
  [Category ("Behavior")]
  [Description ("The data type to be tested.")]
  [DefaultValue (NumericValidationDataType.Integer)]
  public NumericValidationDataType DataType
  {
    get { return _dataType; }
    set { _dataType = value; }
  }

  /// <summary> Gets or sets a value that determines whether negative values are allowed. </summary>
  /// <value>
  /// <see langword="true"/> to allow negative values. Defaults to <see lagnword="true"/>.
  /// </value>
  [Category ("Behavior")]
  [Description ("A value that determines whether negative values are allowed.")]
  [DefaultValue (true)]
  public bool AllowNegative
  {
    get { return _allowNegative; }
    set { _allowNegative = value; }
  }

  /// <summary> Gets or sets the allowed <see cref="NumberStyles"/> of the value to be tested. </summary>
  /// <value> A combination of the values of the <see cref="NumberStyles"/> enumeration. Defaults to <b>None</b>. </value>
  /// <remarks> 
  ///   The number style is used by the <b>Parse</b> method. If it is set to <b>None</b>, the <b>Parse</b> method is 
  ///   called without the <b>NumberStyles</b> argument.
  /// </remarks>
  [Category ("Behavior")]
  [Description ("The allowed NumberStyles of the value to be tested.")]
  [DefaultValue (NumberStyles.None)]
  public NumberStyles NumberStyle
  {
    get { return _numberStyle; }
    set { _numberStyle = value; }
  }
}

public enum NumericValidationDataType
{
  Integer,
  Double,
  Currency
}
}
