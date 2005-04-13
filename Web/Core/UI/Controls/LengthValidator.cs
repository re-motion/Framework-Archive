using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using Rubicon.NullableValueTypes;

namespace Rubicon.Web.UI.Controls
{
public class LengthValidator : BaseValidator
{
  private NaInt32 _minimumLength = NaInt32.Null;
  private NaInt32 _maximumLength = NaInt32.Null;

  // static members and constants

  // member fields

  // construction and disposing

  public LengthValidator ()
  {
    EnableClientScript = false;
  }

  // methods and properties

  protected override bool EvaluateIsValid ()
  {
    string text = base.GetControlValidationValue (base.ControlToValidate);

    if (! _minimumLength.IsNull && text.Length < _minimumLength.Value)
      return false;

    if (! _maximumLength.IsNull && text.Length > _maximumLength.Value)
      return false;

    return true;
  }
  
  /// <summary> The minimum number of characters allowed. </summary>
  /// <value> 
  ///  The minimum length of the validated string 
  ///  or less than zero, or <see cref="NaInt32.Null"/> to disable the validation of the minimum length.
  /// </value>
  [Category ("Behavior")]
  [Description ("The maximum number of characters allowed in the validated property. Set MinimumLength to -1 or emtpy  to not validate the minimum length.")]
  [DefaultValue (typeof(NaInt32), "null")]
  public NaInt32 MinimumLength
  {
    get { return _minimumLength; }
    set
    {
      if (value.IsNull || value.Value < 0)
        _minimumLength = NaInt32.Null;
      else
        _minimumLength = value; 
      _minimumLength = value; 
    }
  }

  /// <summary> The maximum number of characters allowed. </summary>
  /// <value> 
  ///  The maximum length of the validated string
  ///  or less than zero, or <see cref="NaInt32.Null"/> to disable the validation of the maximum length.
  /// </value>
  [Category ("Behavior")]
  [Description ("The maximum number of characters allowed in the validated property. Set MaximumLength to -1 or emtpy to not validate the maximum length.")]
  [DefaultValue (typeof(NaInt32), "null")]
  public NaInt32 MaximumLength
  {
    get { return _maximumLength; }
    set
    {
      if (value.IsNull || value.Value < 0)
        _maximumLength = NaInt32.Null;
      else
        _maximumLength = value; 
      _maximumLength = value; 
    }
  }

}
}
