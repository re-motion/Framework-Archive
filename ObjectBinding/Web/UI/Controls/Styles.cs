using System;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding.Web.Controls
{

public enum ListControlType
{
  DropDownList = 0,
  ListBox = 1,
  RadioButtonList = 2
}

public class ListControlStyle: Style
{
  private ListControlType _controlType = ListControlType.DropDownList;
  private bool _autoPostback = false;
  private int _listBoxRows = 4;
  private int _radioButtonListCellPadding = -1;
  private int _radioButtonListCellSpacing = -1;
  private int _radioButtonListRepeatColumns = 0;
  private RepeatDirection _radioButtonListRepeatDirection = RepeatDirection.Vertical;
  private RepeatLayout _radionButtonListRepeatLayout = RepeatLayout.Table;
  private TextAlign _radioButtonListTextAlign = TextAlign.Right;

  [Description("The type of control that is used in edit mode.")]
  [Category("Behavior")]
  [DefaultValue (0)]
  [NotifyParentProperty (true)]
  public ListControlType ControlType
  {
    get { return _controlType; }
    set { _controlType = value; }
  }

  [Description("Automatically postback to the server after the text is modified.")]
  [Category("Behavior")]
  [DefaultValue (false)]
  [NotifyParentProperty (true)]
  public bool AutoPostback
  {
    get { return _autoPostback; }
    set { _autoPostback = value; }
  }

  [Description("The number of visible rows to display.")]
  [Category("Appearance")]
  [DefaultValue (4)]
  [NotifyParentProperty (true)]
  public int ListBoxRows
  {
    get { return _listBoxRows; }
    set { _listBoxRows = value; }
  }

  [Description("The padding between each item.")]
  [Category("Layout")]
  [DefaultValue (-1)]
  [NotifyParentProperty (true)]
  public int RadioButtonListCellPadding
  {
    get { return _radioButtonListCellPadding; }
    set { _radioButtonListCellPadding = value; }
  }

  [Description("The spacing between each item.")]
  [Category("Layout")]
  [DefaultValue (-1)]
  [NotifyParentProperty (true)]
  public int RadioButtonListCellSpacing
  {
    get { return _radioButtonListCellSpacing; }
    set { _radioButtonListCellSpacing = value; }
  }

  [Description("The number of columns to use to lay out the items.")]
  [Category("Layout")]
  [DefaultValue (0)]
  [NotifyParentProperty (true)]
  public int RadioButtonListRepeatColumns
  {
    get { return _radioButtonListRepeatColumns; }
    set { _radioButtonListRepeatColumns = value; }
  }

  [Description("The direction in which items are laid out.")]
  [Category("Layout")]
  [DefaultValue (1)]
  [NotifyParentProperty (true)]
  public RepeatDirection RadioButtonListRepeatDirection
  {
    get { return _radioButtonListRepeatDirection; }
    set { _radioButtonListRepeatDirection = value; }
  }

  [Description("Whether items are repeated in a table or in-flow.")]
  [Category("Layout")]
  [DefaultValue (0)]
  [NotifyParentProperty (true)]
  public RepeatLayout RadionButtonListRepeatLayout
  {
    get { return _radionButtonListRepeatLayout; }
    set { _radionButtonListRepeatLayout = value; }
  }

  [Description("The alignment of the text label with respect to each item.")]
  [Category("Appearance")]
  [DefaultValue (2)]
  [NotifyParentProperty (true)]
  public TextAlign RadioButtonListTextAlign
  {
    get { return _radioButtonListTextAlign; }
    set { _radioButtonListTextAlign = value; }
  }

  public ListControl Create (bool applyStyle)
  {
    ListControl control;
    switch (_controlType)
    {
      case ListControlType.DropDownList:
        control = new DropDownList ();
        break;
      case ListControlType.ListBox:
        control = new ListBox ();
        break;
      case ListControlType.RadioButtonList:
        control = new RadioButtonList ();
        break;
      default:
        throw new NotSupportedException ("Control type " + _controlType.ToString());
    }
    if (applyStyle)
      ApplyStyle (control);
    return control;
  }

  public void ApplyCommonStyle (ListControl listControl)
  {
    listControl.AutoPostBack = _autoPostback;
  }

  public void ApplyStyle (ListControl listControl)
  {
    if (listControl is ListBox)
      ApplyStyle ((ListBox) listControl);
    else if (listControl is DropDownList)
      ApplyStyle ((DropDownList) listControl);
    else if (listControl is RadioButtonList)
      ApplyStyle ((RadioButtonList) listControl);
    else
      ApplyCommonStyle (listControl);
  }

  public void ApplyStyle (ListBox listBox)
  {
    ApplyCommonStyle (listBox);
    listBox.Rows = _listBoxRows;
  }

  public void ApplyStyle (DropDownList dropDownList)
  {
    ApplyCommonStyle (dropDownList);
  }

  public void ApplyStyle (RadioButtonList radioButtonList)
  {
    ApplyCommonStyle (radioButtonList);
    radioButtonList.CellPadding = _radioButtonListCellPadding;
    radioButtonList.CellSpacing = _radioButtonListCellSpacing;
    radioButtonList.RepeatColumns = _radioButtonListRepeatColumns;
    radioButtonList.RepeatDirection = _radioButtonListRepeatDirection;
    radioButtonList.TextAlign = _radioButtonListTextAlign;
    radioButtonList.RepeatLayout = _radionButtonListRepeatLayout;
  }
}

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
  private bool _autoPostback = false;

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

    textBox.AutoPostBack = _autoPostback;

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

  [Description("Automatically postback to the server after the text is modified.")]
  [Category("Behavior")]
  [DefaultValue (false)]
  [NotifyParentProperty (true)]
  public bool AutoPostBack
  {
    get { return _autoPostback; }
    set { _autoPostback = value; }
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
