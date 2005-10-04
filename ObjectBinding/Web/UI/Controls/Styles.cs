using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web;
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
  private NaBoolean _autoPostBack = NaBoolean.Null;
  private NaInt32 _listBoxRows = NaInt32.Null;
  private NaInt32 _radioButtonListCellPadding = NaInt32.Null;
  private NaInt32 _radioButtonListCellSpacing = NaInt32.Null;
  private NaInt32 _radioButtonListRepeatColumns = NaInt32.Null;
  private RepeatDirection _radioButtonListRepeatDirection = RepeatDirection.Vertical;
  private RepeatLayout _radionButtonListRepeatLayout = RepeatLayout.Table;
  private TextAlign _radioButtonListTextAlign = TextAlign.Right;
  private bool _radioButtonListNullValueVisible = true;

  [Description("The type of control that is used in edit mode.")]
  [Category("Behavior")]
  [DefaultValue (ListControlType.DropDownList)]
  [NotifyParentProperty (true)]
  public ListControlType ControlType
  {
    get { return _controlType; }
    set { _controlType = value; }
  }

  [Description("Automatically postback to the server after the text is modified.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean AutoPostBack
  {
    get { return _autoPostBack; }
    set { _autoPostBack = value; }
  }

  [Description("The number of visible rows to display.")]
  [Category("Appearance")]
  [DefaultValue (typeof(NaInt32), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 ListBoxRows
  {
    get { return _listBoxRows; }
    set { _listBoxRows = value; }
  }

  [Description("The padding between each item.")]
  [Category("Layout")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 RadioButtonListCellPadding
  {
    get { return _radioButtonListCellPadding; }
    set { _radioButtonListCellPadding = value; }
  }

  [Description("The spacing between each item.")]
  [Category("Layout")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 RadioButtonListCellSpacing
  {
    get { return _radioButtonListCellSpacing; }
    set { _radioButtonListCellSpacing = value; }
  }

  [Description("The number of columns to use to lay out the items.")]
  [Category("Layout")]
  [DefaultValue (typeof(NaInt32), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 RadioButtonListRepeatColumns
  {
    get { return _radioButtonListRepeatColumns; }
    set { _radioButtonListRepeatColumns = value; }
  }

  [Description("The direction in which items are laid out.")]
  [Category("Layout")]
  [DefaultValue (RepeatDirection.Vertical)]
  [NotifyParentProperty (true)]
  public RepeatDirection RadioButtonListRepeatDirection
  {
    get { return _radioButtonListRepeatDirection; }
    set { _radioButtonListRepeatDirection = value; }
  }

  [Description("Whether items are repeated in a table or in-flow.")]
  [Category("Layout")]
  [DefaultValue (RepeatLayout.Table)]
  [NotifyParentProperty (true)]
  public RepeatLayout RadionButtonListRepeatLayout
  {
    get { return _radionButtonListRepeatLayout; }
    set { _radionButtonListRepeatLayout = value; }
  }

  [Description("The alignment of the text label with respect to each item.")]
  [Category("Appearance")]
  [DefaultValue (TextAlign.Right)]
  [NotifyParentProperty (true)]
  public TextAlign RadioButtonListTextAlign
  {
    get { return _radioButtonListTextAlign; }
    set { _radioButtonListTextAlign = value; }
  }

  [Description("A flag that determines whether to show the null value in the radio button list.")]
  [Category("Behavior")]
  [DefaultValue (true)]
  [NotifyParentProperty (true)]
  public bool RadioButtonListNullValueVisible
  {
    get { return _radioButtonListNullValueVisible; }
    set { _radioButtonListNullValueVisible = value; }
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
    if (! _autoPostBack.IsNull)
      listControl.AutoPostBack = _autoPostBack.Value;
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

    if (! _listBoxRows.IsNull)
      listBox.Rows = _listBoxRows.Value;
  }

  public void ApplyStyle (DropDownList dropDownList)
  {
    ApplyCommonStyle (dropDownList);
  }

  public void ApplyStyle (RadioButtonList radioButtonList)
  {
    ApplyCommonStyle (radioButtonList);
    
    if (! _radioButtonListCellPadding.IsNull)
      radioButtonList.CellPadding = _radioButtonListCellPadding.Value;
    
    if (! _radioButtonListCellSpacing.IsNull)
      radioButtonList.CellSpacing = _radioButtonListCellSpacing.Value;
    
    if (! _radioButtonListRepeatColumns.IsNull)
      radioButtonList.RepeatColumns = _radioButtonListRepeatColumns.Value;
    
    radioButtonList.RepeatDirection = _radioButtonListRepeatDirection;
    radioButtonList.TextAlign = _radioButtonListTextAlign;
    radioButtonList.RepeatLayout = _radionButtonListRepeatLayout;
  }
}

public class DropDownListStyle: Style
{
  private NaBoolean _autoPostBack = NaBoolean.Null;

  [Description("Automatically postback to the server after the text is modified.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean AutoPostBack
  {
    get { return _autoPostBack; }
    set { _autoPostBack = value; }
  }

  public void ApplyStyle (DropDownList dropDownList)
  {
    if (! _autoPostBack.IsNull)
      dropDownList.AutoPostBack = _autoPostBack.Value;
  }
}

/// <summary>
/// Styles for single row TextBox controls.
/// </summary>
public class SingleRowTextBoxStyle: Style
{
  private NaInt32 _columns = NaInt32.Null;
  private NaInt32 _maxLength = NaInt32.Null;
  private NaBoolean _readOnly = NaBoolean.Null;
  private NaBoolean _autoPostBack = NaBoolean.Null;
  private NaBoolean _checkClientSideMaxLength = NaBoolean.Null;

  public virtual void ApplyStyle (TextBox textBox)
  {
    textBox.ApplyStyle (this);
    
    if (! _maxLength.IsNull && ! _checkClientSideMaxLength.IsFalse)
      textBox.MaxLength = _maxLength.Value;
    
    if (! _columns.IsNull)
      textBox.Columns = _columns.Value;
    
    if (! _autoPostBack.IsNull)
      textBox.AutoPostBack = _autoPostBack.Value;
    
    if (! _readOnly.IsNull)
      textBox.ReadOnly = _readOnly.Value;
  }

  public override void CopyFrom (Style s)
  {
    base.CopyFrom (s);
    SingleRowTextBoxStyle ts = s as SingleRowTextBoxStyle;
    if (ts != null)
    {
      if (! _checkClientSideMaxLength.IsFalse)
        _maxLength = ts.MaxLength;
      _columns = ts.Columns;
      _readOnly = ts.ReadOnly;
    }
  }

  [Description("The width of the textbox in characters.")]
  [Category("Appearance")]
  [DefaultValue (typeof(NaInt32), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 Columns
  {
    get { return _columns; }
    set { _columns = value; }
  }

  [Description("The maximum number of characters that can be entered.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaInt32), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 MaxLength
  {
    get { return _maxLength; }
    set { _maxLength = value; }
  }

  [Description("Whether the text in the control can be changed or not.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean ReadOnly
  {
    get { return _readOnly; }
    set { _readOnly = value; }
  }

  [Description("Automatically postback to the server after the text is modified.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean AutoPostBack
  {
    get { return _autoPostBack; }
    set { _autoPostBack = value; }
  }

  [Description("Whether the text in the control can exceed its max length during input. If true, MaxLength is only used for validation after the input is completed.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean CheckClientSideMaxLength
  {
    get { return _checkClientSideMaxLength; }
    set { _checkClientSideMaxLength = value; }
  }

}

/// <summary>
/// Styles for TextBox controls.
/// </summary>
public class TextBoxStyle: SingleRowTextBoxStyle
{
  private const string c_scriptFileUrl = "TextBoxStyle.js";
  private static readonly string s_scriptFileKey = typeof (TextBoxStyle).FullName + "_Script";

  private NaInt32 _rows = NaInt32.Null;
  private TextBoxMode _textMode;
  private TextBoxMode _defaultTextMode = TextBoxMode.SingleLine;
  private NaBoolean _wrap = NaBoolean.Null;

  public TextBoxStyle (TextBoxMode defaultTextMode)
  {
    _defaultTextMode = defaultTextMode;
    _textMode = _defaultTextMode;
  }

  public TextBoxStyle() 
    : this (TextBoxMode.SingleLine)
  {
  }

  public override void ApplyStyle (TextBox textBox)
  {
    base.ApplyStyle (textBox);
    
    if (! _rows.IsNull)
      textBox.Rows = _rows.Value;

    if (! _wrap.IsNull)
      textBox.Wrap = _wrap.Value;

    if (   _textMode == TextBoxMode.MultiLine 
        && ! MaxLength.IsNull 
        && ! CheckClientSideMaxLength.IsFalse
        && ! ControlHelper.IsDesignMode ((Control) textBox)) 
    {
      if (! HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            textBox, null, typeof (TextBoxStyle), ResourceType.Html, c_scriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
      }
      textBox.Attributes.Add ("onkeydown", "return TextBoxStyle_OnKeyDown (this, " + MaxLength.Value + ");");
    }

    textBox.TextMode = _textMode;
  }

  public override void CopyFrom (Style s)
  {
    base.CopyFrom (s);
    TextBoxStyle ts = s as TextBoxStyle;
    if (ts != null)
    {
      this.Rows = ts.Rows;
      this.TextMode = ts.TextMode;
      this.Wrap = ts.Wrap;
    }
  }
  [Description("The number of lines to display for a multiline textbox.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaInt32), "null")]
  [NotifyParentProperty (true)]
  public NaInt32 Rows
  {
    get { return _rows; }
    set { _rows = value; }
  }

  [Description("The behavior mode of the textbox.")]
  [Category("Behavior")]
  [NotifyParentProperty (true)]
  public TextBoxMode TextMode
  {
    get { return _textMode; }
    set { _textMode = value; }
  }

  /// <summary> Controls the persisting of the <see cref="TextMode"/>. </summary>
  private bool ShouldSerializeTextMode()
  {
    return _textMode != _defaultTextMode;
  }

  /// <summary> Sets the <see cref="TextMode"/> to its default value. </summary>
  private void ResetTextMode()
  {
    _textMode = _defaultTextMode;
  }

  [Description("Gets or sets a value indicating whether the text should be wrapped in edit mode.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean Wrap
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
