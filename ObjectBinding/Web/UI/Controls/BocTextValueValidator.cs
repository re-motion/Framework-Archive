using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   Creates a VS.NET designer pick list for a property that references a <see cref="BocTextValue"/> control.
/// </summary>
/// <remarks>
///   Use the <see cref="TypeConverter"/> attribute to assign this converter to a property.
/// </remarks>
public class BocTextValueControlToStringConverter: ControlToStringConverter
{
  public BocTextValueControlToStringConverter ()
    : base (typeof (BocTextValue))
  {
  }
}

/// <summary>
///   Validates date/time values in the current culture.
/// </summary>
/// <remarks>
///   This class does not provide client-side validation.
/// </remarks>
public class DateTimeValidator: BaseValidator
{
  protected override bool EvaluateIsValid()
  {
    string text = GetControlValidationValue (ControlToValidate);
    if (text == null )
      return true;
    text = text.Trim();
    if (text.Length == 0)
      return true;

    try
    {
      DateTime.Parse (text);
    }
    catch (FormatException)
    {
      return false;
    }
    return true;
  }
}

/// <summary>
///   Compound validator for <see cref="FscValue"/> controls.
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
///       <description>The validated <see cref="FscValue"/> control's <c>IsRequired</c> property is true.</description>
///     </item>
///   </list>
/// </remarks>
public class BocTextValueValidator: CompoundValidator
{
  public BocTextValueValidator ()
    : base (typeof (BocTextValue))
  {
  }

  [TypeConverter (typeof (BocTextValueControlToStringConverter))]
  public override string ControlToValidate
  {
    get { return base.ControlToValidate; }
    set { base.ControlToValidate = value; }
  }

  protected override void CreateChildValidators ()
  {
    if (this.Site != null && this.Site.DesignMode)
      return;

    BocTextValue textValueControl = NamingContainer.FindControl (ControlToValidate) as BocTextValue;
    if (textValueControl == null)
      return;
    if (textValueControl.IsReadOnly)
      return;

    string controlToValidateId = textValueControl.ID + "_TextBox";

    if (textValueControl.IsRequired)
    {
      RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
      requiredValidator.ID = this.ID + "_RequiredValidator";
      requiredValidator.ControlToValidate = controlToValidateId;
      requiredValidator.ErrorMessage = "Enter a value.";
      requiredValidator.Display = ValidatorDisplay.Dynamic;
      Controls.Add (requiredValidator);
    }

    BocTextValueType valueType = textValueControl.ActualValueType;
    if (valueType == BocTextValueType.DateTime)
    {
      DateTimeValidator typeValidator = new DateTimeValidator();
      typeValidator.ID = this.ID + "_TypeValidator";
      typeValidator.ControlToValidate = controlToValidateId;
      typeValidator.ErrorMessage = "Wrong type.";
      typeValidator.Display = ValidatorDisplay.Dynamic;
      Controls.Add (typeValidator);
    }
    else if (valueType != BocTextValueType.Undefined && valueType != BocTextValueType.String)
    {
      CompareValidator typeValidator = new CompareValidator();
      typeValidator.ID = this.ID + "_TypeValidator";
      typeValidator.ControlToValidate = controlToValidateId;
      typeValidator.Operator = ValidationCompareOperator.DataTypeCheck;
      typeValidator.Type = GetValidationDataType (valueType);
      typeValidator.ErrorMessage = "Wrong type.";
      typeValidator.Display = ValidatorDisplay.Dynamic;
      //typeValidator.EnableClientScript = false;
      Controls.Add (typeValidator);
    }
  }

  private ValidationDataType GetValidationDataType (BocTextValueType fscValueType)
  {
    switch (fscValueType)
    {
      case BocTextValueType.Date:
        return ValidationDataType.Date;
      case BocTextValueType.DateTime:
        return ValidationDataType.Date;
      case BocTextValueType.Integer:
        return ValidationDataType.Integer;
      case BocTextValueType.Double:
        return ValidationDataType.Double;
      default:
        throw new ArgumentException ("Cannot convert " + fscValueType.ToString() + " to type " + typeof (ValidationDataType).FullName + ".");
    }
  }
}

}
