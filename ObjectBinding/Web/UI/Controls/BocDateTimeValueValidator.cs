using System;
using System.Collections;
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
public class DateValidator: BaseValidator
{
  protected override bool EvaluateIsValid()
  {
    string dateValue =  GetControlValidationValue (ControlToValidate);

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
public class TimeValidator: BaseValidator
{
  protected override bool EvaluateIsValid()
  {
    string timeValue =  GetControlValidationValue (ControlToValidate);

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
}

/// <summary>
///   Compound validator for <see cref="BocDateTimeValue"/> controls.
/// </summary>
/// <remarks>
///   This compound validator automatically creates the following child validators:
///   <list type="table">
///     <listheader>
///       <term>Validator</term>
///       <description>Condition</description>
///     </listheader>
///     <item>
///       <term><see cref="RequiredFieldValidator"/></term>
///       <description>
///         <para>
///           The validated <see cref="BocDateTimeValue"/> control's 
///           <c>IsRequired</c> property is <see langword="true"/>.
///         </para><para>
///           Only the date component will be validated for 
///           <see cref="BocDateTimeValue"/> controls.
///         </para>
///     </description>
///     </item>
///     <item>
///       <term><see cref="DateValidator"/></term>
///       <description>
///         The validated <see cref="BocDateTimeValue"/> control
///         is bound to an <see cref="IBusinessObjectDateTimeProperty"/> 
///         or an <see cref="IBusinessObjectDateProperty"/>.
///       </description>
///     </item>
///     <item>
///       <term><see cref="TimeValidator"/></term>
///       <description>
///         The validated <see cref="BocDateTimeValue"/> control
///         is bound to an <see cref="IBusinessObjectDateTimeProperty"/>.
///       </description>
///     </item>
///   </list>
/// </remarks>
public class BocDateTimeValueValidator: CompoundValidator
{
  public BocDateTimeValueValidator ()
    : base (typeof (BocDateTimeValue))
  {
  }

  [TypeConverter (typeof (BocDateTimeValueControlToStringConverter))]
  public override string ControlToValidate
  {
    get { return base.ControlToValidate; }
    set { base.ControlToValidate = value; }
  }

  public static BaseValidator[] CreateValidators (
    BocDateTimeValue dateTimeValueControl, 
    string baseID)
  {
    ArrayList validators = new ArrayList();

    BocDateTimeValueType valueType = dateTimeValueControl.ValueType;
    if (dateTimeValueControl.IsRequired)
    {
      //  Only validate the date field. The time can default to 00:00 if it is empty.
      if (    valueType == BocDateTimeValueType.DateTime
          ||  valueType == BocDateTimeValueType.Date)
      {
        RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
        requiredValidator.ID = baseID + "Required";
        requiredValidator.ControlToValidate = dateTimeValueControl.DateTextBox.ID;
        requiredValidator.ErrorMessage = "Enter a value.";
        requiredValidator.Display = ValidatorDisplay.Dynamic;
        validators.Add (requiredValidator);
      }
    }

    if (    valueType == BocDateTimeValueType.DateTime
        ||  valueType == BocDateTimeValueType.Date)
    {
      DateValidator dateValidator = new DateValidator();
      dateValidator.ID = baseID + "Date";
      dateValidator.ControlToValidate = dateTimeValueControl.DateTextBox.ID;
      dateValidator.ErrorMessage = "Unknown date format.";
      dateValidator.Display = ValidatorDisplay.Dynamic;
      validators.Add (dateValidator);
    }

    if (valueType == BocDateTimeValueType.DateTime)
    {
      TimeValidator timeValidator = new TimeValidator();
      timeValidator.ID = baseID + "Time";
      timeValidator.ControlToValidate = dateTimeValueControl.TimeTextBox.ID;
      timeValidator.ErrorMessage = "Unknown time format.";
      timeValidator.Display = ValidatorDisplay.Dynamic;
      validators.Add (timeValidator);
    }

    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }

  protected override void CreateChildValidators ()
  {
    if (ControlHelper.IsDesignMode (this, Context))
      return;

    BocDateTimeValue dateTimeValueControl = 
      NamingContainer.FindControl (ControlToValidate) as BocDateTimeValue;

    if (dateTimeValueControl == null)
      return;

    if (dateTimeValueControl.IsReadOnly)
      return;

    string baseID = this.ID + "_Validator";
    foreach (BaseValidator validator in CreateValidators (dateTimeValueControl, baseID))
      Controls.Add (validator);
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
