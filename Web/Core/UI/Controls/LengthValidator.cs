using System;
using System.Web.UI.WebControls;

namespace Rubicon.Web.UI.Controls
{
public class LengthValidator : BaseValidator
{
  private int _minimumLength = -1;
  private int _maximumLength = -1;

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
    TextBox textBox = FindControl (ControlToValidate) as TextBox;
    if (textBox == null)
      throw new InvalidOperationException ("LengthValidator can only be applied to TextBox controls.");

    if (_minimumLength >= 0 && textBox.Text.Length < _minimumLength)
      return false;

    if (_maximumLength >= 0 && textBox.Text.Length > _maximumLength)
      return false;

    return true;
  }
  
  public int MinimumLength
  {
    get { return _minimumLength; }
    set { _minimumLength = value; }
  }

  public int MaximumLength
  {
    get { return _maximumLength; }
    set { _maximumLength = value; }
  }

}
}
