using System;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
/// Styles for TextBox controls.
/// </summary>
public class TextBoxStyle: Style
{
  private int _columns = 0;
  private int _maxLength = 0;
  private bool _readOnly = false;
  private int _rows = 0;
  private TextBoxMode _textMode = TextBoxMode.SingleLine;
  private bool _wrap = true;

  public void ApplyStyle (TextBox textBox)
  {
    textBox.ApplyStyle (this);
    if (_maxLength != 0)
      textBox.MaxLength = _maxLength;
    if (_columns != 0)
      textBox.Columns = _columns;
    if (_rows != 0)
      textBox.Rows = _rows;
    if (_wrap != true)
      textBox.Wrap = _wrap;

    textBox.ReadOnly = _readOnly;
    textBox.TextMode = _textMode;
  }

  public override void CopyFrom (Style s)
  {
    base.CopyFrom (s);
    TextBoxStyle ts = s as TextBoxStyle;
    if (ts != null)
    {
      this.MaxLength = ts.MaxLength;
      this.Columns = ts.Columns;
      this.ReadOnly = ts.ReadOnly;
      this.Rows = ts.Rows;
      this.TextMode = ts.TextMode;
      this.Wrap = ts.Wrap;
    }
  }

  [Description("The width of the textbox in characters.")]
  [Category("Appearance")]
  [DefaultValue (0)]
  [NotifyParentProperty (true)]
  public int Columns
  {
    get { return _columns; }
    set { _columns = value; }
  }

  [Description("The maximum number of characters that can be entered.")]
  [Category("Behavior")]
  [DefaultValue (0)]
  [NotifyParentProperty (true)]
  public int MaxLength
  {
    get { return _maxLength; }
    set { _maxLength = value; }
  }

  [Description("Whether the text in the control can be changed or not.")]
  [Category("Behavior")]
  [DefaultValue (false)]
  [NotifyParentProperty (true)]
  public bool ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  [Description("The number of lines to display for a multiline textbox.")]
  [Category("Behavior")]
  [DefaultValue (0)]
  [NotifyParentProperty (true)]
  public int Rows
  {
    get { return _rows; }
    set { _rows = value; }
  }

  [Description("The behavior mode of the textbox.")]
  [Category("Behavior")]
  [DefaultValue (typeof (TextBoxMode), "SingleLine")]
  [NotifyParentProperty (true)]
  public TextBoxMode TextMode
  {
    get { return _textMode; }
    set { _textMode = value; }
  }

  [Description("Gets or sets a value indicating whether the text should be wrapped in edit mode.")]
  [Category("Behavior")]
  [DefaultValue (true)]
  [NotifyParentProperty (true)]
  public bool Wrap
  {
    get { return _wrap; }
    set { _wrap = value; }
  }
}

// obsolete since CompoundValidator supports Visible and EnableClientScript directly through ICompleteValidator
///// <summary>
///// Styles for validator controls.
///// </summary>
//public class ValidatorStyle: Style
//{
//  public enum OptionalValidatorDisplay
//  {
//    Undefined = -1,
//    Dynamic = ValidatorDisplay.Dynamic,
//    Static = ValidatorDisplay.Static,
//    None = ValidatorDisplay.None
//  }
//
//  private OptionalValidatorDisplay _display = OptionalValidatorDisplay.Undefined;
//  private NaBooleanEnum _enableClientScript = NaBooleanEnum.Undefined;
//
//  public void ApplyStyle (BaseValidator validator)
//  {
//    validator.ApplyStyle (this);
//    NaBoolean enableClientScript = _enableClientScript;
//    if (! enableClientScript.IsNull)
//      validator.EnableClientScript = (bool) enableClientScript;
//    if (_display == OptionalValidatorDisplay.Undefined)
//      validator.Display = (ValidatorDisplay) _display;
//  }
//
//  public override void CopyFrom (Style s)
//  {
//    base.CopyFrom (s);
//    ValidatorStyle vs = s as ValidatorStyle;
//    if (vs != null)
//    {
//      this.Display = vs.Display;
//      this.EnableClientScript = vs.EnableClientScript;
//    }
//  }
//
//  [Description("How the validator is displayed.")]
//  [Category("Appearance")]
//  [DefaultValue (typeof (OptionalValidatorDisplay), "Undefined")]
//  [NotifyParentProperty (true)]
//  public OptionalValidatorDisplay Display
//  {
//    get { return _display; }
//    set { _display = value; }
//  }
//
//  [Description("Indicates whether to perform validation on the client in up-level browsers.")]
//  [Category("Behavior")]
//  [DefaultValue (true)]
//  [NotifyParentProperty (true)]
//  public NaBooleanEnum EnableClientScript
//  {
//    get { return _enableClientScript; }
//    set { _enableClientScript = value; }
//  }
//}

}
