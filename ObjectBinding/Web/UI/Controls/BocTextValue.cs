using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

public enum BocTextValueType
{
  Undefined,
  String,
  Integer,
  Date,
  DateTime,
  Double
}

/// <summary>
///   This control can be used to display or edit values that can be edited in a text box.
/// </summary>
/// <remarks>
///   <para>
///     The <see cref="BocTextValueType"/> enumeration defines the types that can be handled by <c>BocTextValue</c>.
///     The control can act as a data bound control or as a disconnected control. Use the <see cref="Value"/>
///     property to get or set values in their native data type, or the <see cref="Text"/> property to get or
///     set the string representation.
///   </para><para>
///     The control is displayed using a text box in edit mode, and using a label in read-only mode. Use the
///     <see cref="TextBox"/> and <c>Label</c> properties to access these controls directly.
///   </para>
/// </remarks>
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocTextValue: BusinessObjectBoundModifiableWebControl
{
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectNumericProperty), typeof (IBusinessObjectStringProperty), typeof (IBusinessObjectDateProperty), typeof (IBusinessObjectDateTimeProperty) };

  /// <summary>
  ///   This event is fired when the text is changed in the UI.
  /// </summary>
  /// <remarks>
  ///   The event is fired only if the text change is caused by the user.
  /// </remarks>
  public event EventHandler TextChanged;

  private BocTextValueType _valueType = BocTextValueType.Undefined;
  private BocTextValueType _actualValueType = BocTextValueType.Undefined;

  private string _text = string.Empty;
  private string _newText = null;
  private bool _isDirty = true;
  private TextBox _textBox = null;
  private Label _label = null;

  [Browsable (false)]
  public TextBox TextBox
  {
    get { return _textBox; }
  }

  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

	public BocTextValue()
	{
    _textBox = new BocTextValueTextBox(this);
    _label = new Label ();
	}

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _textBox.ID = this.ID + "_TextBox";
    _textBox.EnableViewState = false;
    Controls.Add (_textBox);

    _label.ID = this.ID + "_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);

    if (! (this.Site != null && this.Site.DesignMode))
    {
      string newValue = this.Page.Request.Form[_textBox.UniqueID];
      if (newValue != null)
        _newText = newValue;
    }

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
  }

  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    if (_newText != null && _newText != _text)
      _isDirty = true;
  }

  internal void HandleTextChange()
  {
    if (_newText != null && _newText != _text)
    {
      Text = _newText;
      OnTextChanged (EventArgs.Empty);
    }
  }

  /// <summary>
  /// Fires the <see cref="TextChanged"/> event.
  /// </summary>
  /// <param name="e"> Empty. </param>
  protected virtual void OnTextChanged (EventArgs e)
  {
    // _isDirty = true; // moved to OnLoad
    if (TextChanged != null)
      TextChanged (this, e);
  }

  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    if (Property != null)
    {
      if (_valueType == BocTextValueType.Undefined)
        _actualValueType = GetBocTextValueType (Property);
      
      IBusinessObjectStringProperty stringProperty = Property as IBusinessObjectStringProperty;
      if (stringProperty != null)
      {
        NaInt32 length = stringProperty.MaxLength;
        if (! length.IsNull)
          _textBox.MaxLength = length.Value;
      }
    }
  }

  public override void LoadValue()
  {
    Binding.EvaluateBinding();
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
    {
      Value = DataSource.BusinessObject.GetProperty (Property);
      _isDirty = false;
    }
  }

  private BocTextValueType GetBocTextValueType (IBusinessObjectProperty property)
  {
    if (property is IBusinessObjectStringProperty)
      return BocTextValueType.String;
    else if (property is IBusinessObjectInt32Property)
      return BocTextValueType.Integer;
    else if (property is IBusinessObjectDoubleProperty)
      return BocTextValueType.Double;
    else if (property is IBusinessObjectDateProperty)
      return BocTextValueType.Date;
    else if (property is IBusinessObjectDateTimeProperty)
      return BocTextValueType.DateTime;
    else
      throw new NotSupportedException ("BocTextValue does not support property type " + property.GetType());
  }

  public override void SaveValue()
  {
    Binding.EvaluateBinding();
    if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
      DataSource.BusinessObject.SetProperty (Property, Value);
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    EnsureChildControlsInitialized ();
  }

  protected override void Render (HtmlTextWriter writer)
  {
    EnsureChildControlsInitialized ();
    base.Render (writer);
  }

  protected override void InitializeChildControls()
  {
    bool isReadOnly = IsReadOnly;
    _textBox.Visible = ! isReadOnly;
    _label.Visible = isReadOnly;
    if (isReadOnly)
    {
      _label.Text = _text;

      _label.Width = this.Width;
      _label.Height = this.Height;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      _textBox.Width = this.Width;
      _textBox.Height = this.Height;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
    }
  }

  private Style _commonStyle = new Style ();
  private TextBoxStyle _textBoxStyle = new TextBoxStyle ();
  private Style _labelStyle = new Style ();

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
  ///   the respective modes. Note that if you set one of the <c>Font</c> attributes (Bold, Italic etc.) to 
  ///   <c>true</c>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
  ///   properties.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public TextBoxStyle TextBoxStyle
  {
    get { return _textBoxStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the Label (read-only mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    _text = (string) values[1];
    _valueType = (BocTextValueType) values[2];
    _actualValueType = (BocTextValueType) values[3];
    _isDirty = (bool)  values[4];
  }

  protected override object SaveViewState()
  {
    object[] values = new object[5];
    values[0] = base.SaveViewState();
    values[1] = _text;
    values[2] = _valueType;
    values[3] = _actualValueType;
    values[4] = _isDirty;
    return values;
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <value> 
  ///   The value has the type specified in the <see cref="ValueType"/> property (<see cref="String"/>, <see cref="Int32"/>, 
  ///   <see cref="Double"/> or <see cref="DateTime"/>).
  /// </value>
  /// <exception cref="FormatException">The value of the <see cref="Text"/> property cannot be converted to the specified <see cref="ValueType"/>.</exception>
  [Description("Gets or sets the current value.")]
  [Browsable(false)]
  public override object Value
  {
    get 
    {
      string text = _text;
      if (text != null)
        text = text.Trim();

      if (text == null || text.Length == 0)
        return null;

      if (_actualValueType == BocTextValueType.Integer)
        return int.Parse (text);
      else if (_actualValueType == BocTextValueType.Double)
        return double.Parse (text);
      else if (_actualValueType == BocTextValueType.Date)
        return DateTime.Parse (text).Date; 
      else if (_actualValueType == BocTextValueType.DateTime)
        return DateTime.Parse (text); 
      else 
        return (string) text;
    }

    set 
    { 
      if (value == null)
      {
        _text = string.Empty;
        return;
      }
      try
      {
        if (_actualValueType == BocTextValueType.Integer)
          _text = ((int) value).ToString();
        else if (_actualValueType == BocTextValueType.Double)
          _text = ((double) value).ToString();
        else if (_actualValueType == BocTextValueType.Date)
          _text = ((DateTime) value).ToString ("d");
        else if (_actualValueType == BocTextValueType.DateTime)
          _text = ((DateTime) value).ToString ("G");
        else
          _text = (string) value;
      }
      catch (InvalidCastException e)
      {
        throw new ArgumentException ("Expected type " + _actualValueType.ToString() + ", but was " + value.GetType().FullName, "value", e);
      }
    }
  }

  [Description("Gets or sets the string representation of the current value.")]
  [Category("Data")]
  public string Text
  {
    get { return _text; }
    set { _text = StringUtility.NullToEmpty (value); }
  }

  [Description("Gets or sets a fixed value type.")]
  [Category("Data")]
  [DefaultValue(typeof(BocTextValueType), "Undefined")]
  public BocTextValueType ValueType
  {
    get { return _valueType; }
    set 
    {
      if (_valueType != value)
      {
        _valueType = value;
        _actualValueType = value;
        if (_valueType != BocTextValueType.Undefined)
          _text = string.Empty;
      }
    }
  }

  /// <summary>
  /// Gets the controls fixed value type or, if undefined, the property's value type.
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocTextValueType ActualValueType
  {
    get 
    {
      Binding.EvaluateBinding();
      return _actualValueType;
    }
  }

  public override Control TargetControl
  {
    get { return (_textBox != null) ? _textBox : (Control) this; }
  }

  /// <summary>
  ///   Specifies whether the text within the control has been changed since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> during loading
  ///   and saving values. Text changes by the user cause <c>IsDirty</c> to be reset to <c>false</c> during the
  ///   loading phase of the request (i.e., before the page's <c>Load</c> event is raised).
  /// </remarks>
  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  public override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

}

/// <summary>
///   A special version of TextBox that allows the containing BocTextValue control to correctly 
///   dispatch changed events in the correct phase of the control lifecycle.
/// </summary>
internal class BocTextValueTextBox: TextBox, IPostBackDataHandler
{
  private BocTextValue _parent;
  public BocTextValueTextBox (BocTextValue parent)
  {
    _parent = parent;
  }

  // IPostBackDataHandler Members

  public void RaisePostDataChangedEvent()
  {
    // let parent control decide what to
    _parent.HandleTextChange();    
  }

  public bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    // always call RaisePostDataChangedEvent 
    return true;
  }

  public override string Text
  {
    get { return _parent.Text; }
    set { _parent.Text = value; }
  }

}

}
