using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using Rubicon.ObjectBinding;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Utilities;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BocDateTimeValueValidator: BaseValidator
{
  private string _requiredErrorMessage = null;
  private string _incompleteErrorMessage = null;
  private string _invalidDateAndTimeErrorMessage = null;
  private string _invalidDateErrorMessage = null;
  private string _invalidTimeErrorMessage = null;

  protected override bool EvaluateIsValid()
  {
    Control control = this.NamingContainer.FindControl(ControlToValidate);

    BocDateTimeValue dateTimeValueControl = control as BocDateTimeValue;

    if (dateTimeValueControl == null)
      return true;

    if (! EvaluateIsRequiredValid (dateTimeValueControl))
    {
      if (! StringUtility.IsNullOrEmpty (RequiredErrorMessage))
        ErrorMessage = RequiredErrorMessage;

      return false;
    }

    if (! EvaluateIsCompleteValid (dateTimeValueControl))
    {
      if (! StringUtility.IsNullOrEmpty (IncompleteErrorMessage))
        ErrorMessage = IncompleteErrorMessage;

      return false;
    }

    bool isValidDate = EvaluateIsValidDate (dateTimeValueControl);
    bool isValidTime = EvaluateIsValidTime (dateTimeValueControl);

    if (! isValidDate && ! isValidTime)
    {
      if (! StringUtility.IsNullOrEmpty (InvalidDateAndTimeErrorMessage))
        ErrorMessage = InvalidDateAndTimeErrorMessage;
    }
    else if (! isValidDate)
    {
       if (! StringUtility.IsNullOrEmpty (InvalidDateErrorMessage))
        ErrorMessage = InvalidDateErrorMessage;
    }
    else if (! isValidTime)
    {
      if (! StringUtility.IsNullOrEmpty (InvalidTimeErrorMessage))
        ErrorMessage = InvalidTimeErrorMessage;
    }

    return isValidDate && isValidTime;
  }

  private bool EvaluateIsRequiredValid (BocDateTimeValue control)
  {
    if (! control.IsRequired)
      return true;

    bool isDateRequired =     control.ActualValueType == BocDateTimeValueType.DateTime
                          ||  control.ActualValueType == BocDateTimeValueType.Date;
    bool isTimeRequired =     control.ActualValueType == BocDateTimeValueType.DateTime;

    //  Neither field required because the value of the control of an unknown/undefined type
    if (! isDateRequired && ! isTimeRequired)
      return true;

    bool hasDate = ! StringUtility.IsNullOrEmpty (control.DateTextBox.Text); 
    bool hasTime = ! StringUtility.IsNullOrEmpty (control.TimeTextBox.Text); 

    bool isDateMissing = isDateRequired && ! hasDate;
    bool isTimeMissing = isTimeRequired && ! hasTime;

    return ! (isDateMissing || isTimeMissing);
  }

  private bool EvaluateIsCompleteValid (BocDateTimeValue control)
  {
    bool isDateRequired =     control.ActualValueType == BocDateTimeValueType.DateTime
                          ||  control.ActualValueType == BocDateTimeValueType.Date;
    
    bool hasDate = ! StringUtility.IsNullOrEmpty (control.DateTextBox.Text);     
    bool hasTime = ! StringUtility.IsNullOrEmpty (control.TimeTextBox.Text); 

    bool isDateMissing = isDateRequired && ! hasDate;

    return ! (isDateMissing && hasTime);
  }

  /// <summary>
  ///   Validates date values in the current culture.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This class does not provide client-side validation.
  ///   </para><para>
  ///     Cannot detect a 0:0 time component included in the date string.
  ///     Since a time-offset of 0:0 will not falsify the result, this is acceptable.
  ///     Prevented by setting the MaxLength attribute of the input field.
  ///   </para>
  /// </remarks>
  private bool EvaluateIsValidDate (BocDateTimeValue control)
  {
    if (control.ActualValueType == BocDateTimeValueType.Undefined)
    {
      return true;
    }

    string dateValue = control.DateTextBox.Text;

    //  Test for empty

    if (dateValue == null)
      return true;

    dateValue = dateValue.Trim();    
    if (dateValue.Length == 0)
      return true;

    try
    {
      //  Is a valid date/time value? If not, FormatException will be thrown
      DateTime dateTime = DateTime.Parse (dateValue);

      //  Has a time component?
      if (dateTime.TimeOfDay != TimeSpan.Zero)
        return false;
    }
    catch (FormatException)
    {
      return false;
    }

    try
    {
      //  If there is only a time value in the date field, 
      //  it will be detected if dateTimeToday and dateTimeFirstDay differ

      //  Empty date defaults to 01.01.0001
      DateTime dateTimeFirstDay = DateTime.Parse (
        dateValue, 
        Thread.CurrentThread.CurrentCulture);

      //  Empty date defaults to today
      DateTime dateTimeToday = DateTime.Parse (
        dateValue, 
        Thread.CurrentThread.CurrentCulture,
        DateTimeStyles.NoCurrentDateDefault);

      //  That's actually a time instead of a date
      if (dateTimeToday.Date != dateTimeFirstDay.Date)
        return false;
    }
    catch (FormatException)
    {
      //  This exception will most likely never happen
    }

    return true;
  }

  /// <summary>
  ///   Valitimes time values in the current culture.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     This class does not provide client-side validation.
  ///   </para><para>
  ///     Does not detect an included date of 01.01.0001. 
  ///   </para>
  /// </remarks>
  private bool EvaluateIsValidTime (BocDateTimeValue control)
  {
    if (    control.ActualValueType == BocDateTimeValueType.Date
        ||  control.ActualValueType == BocDateTimeValueType.Undefined)
    {
      return true;
    }

    string timeValue = control.TimeTextBox.Text;

    //  Test for empty

    if (timeValue == null)
      return true;

    //  Test for empty
    timeValue = timeValue.Trim();    
    if (timeValue.Length == 0)
      return true;

    try
    {
      //  Is a valid/time value? If not, FormatException will be thrown
      DateTime dateTime = DateTime.Parse (
        timeValue, 
        Thread.CurrentThread.CurrentCulture,
        DateTimeStyles.NoCurrentDateDefault);

      //  If only time, date will default to 01.01.0001
      if (dateTime.Year != 1 || dateTime.Month != 1 || dateTime.Day != 1)
        return false;
    }
    catch (FormatException)
    {
      return false;
    }

    return true;
  }

  protected override bool ControlPropertiesValid()
  {
    if (! base.ControlPropertiesValid())
    {
      return false;
    }

    Control control = this.NamingContainer.FindControl(ControlToValidate);

    if (! (control is BocDateTimeValue))
    {
      throw new HttpException("Control '" + ControlToValidate + "' is not of type '" + typeof (BocDateTimeValue) + "'");
    }

    return true;
  } 

  [TypeConverter (typeof (BocDateTimeValueControlToStringConverter))]
  public new string ControlToValidate
  {
    get { return base.ControlToValidate; }
    set { base.ControlToValidate = value; }
  }

  [Browsable (false)]
  public new string ErrorMessage
  {
    get { return base.ErrorMessage; }
    set { base.ErrorMessage = value; }
  }

  public string RequiredErrorMessage
  {
    get { return _requiredErrorMessage; }
    set { _requiredErrorMessage = value; }
  }

  public string IncompleteErrorMessage
  {
    get { return _incompleteErrorMessage; }
    set { _incompleteErrorMessage = value; }
  }

  public string InvalidDateErrorMessage
  {
    get { return _invalidDateErrorMessage; }
    set { _invalidDateErrorMessage = value; }
  }

  public string InvalidTimeErrorMessage
  {
    get { return _invalidTimeErrorMessage; }
    set { _invalidTimeErrorMessage = value; }
  }

  public string InvalidDateAndTimeErrorMessage
  {
    get { return _invalidDateAndTimeErrorMessage; }
    set { _invalidDateAndTimeErrorMessage = value; }
  }


}

/// <summary>
///   Creates a VS.NET designer pick list for a property that references a 
///   <see cref="BocDateTimeValue"/> control.
/// </summary>
/// <remarks>
///   Use the <see cref="TypeConverter"/> attribute to assign this converter to a property.
/// </remarks>
public class BocDateTimeValueControlToStringConverter: ControlToStringConverter
{
  public BocDateTimeValueControlToStringConverter ()
    : base (typeof (BocDateTimeValue))
  {
  }
}

}
