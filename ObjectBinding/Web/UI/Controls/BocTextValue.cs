using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

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
public class BocTextValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
  //  constants

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyLabelContents = "#";

  private const int c_defaultTextBoxWidthInPoints = 150;

  //  statics

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectNumericProperty), typeof (IBusinessObjectStringProperty), typeof (IBusinessObjectDateProperty), typeof (IBusinessObjectDateTimeProperty) };

  private static readonly object s_textChangedEvent = new object();

  // types

  /// <summary> A list of control wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocTextValue")]
  protected enum ResourceIdentifier
  {
    RequiredErrorMessage,
    InvalidDateAndTimeErrorMessage,
    InvalidDateErrorMessage,
    InvalidIntegerErrorMessage,
    InvalidDoubleErrorMessage
  }

  // fields

  private BocTextValueType _valueType = BocTextValueType.Undefined;
  private BocTextValueType _actualValueType = BocTextValueType.Undefined;

  private string _text = string.Empty;
  private bool _isDirty = true;
  private TextBox _textBox;
  private Label _label;

  private Style _commonStyle = new Style ();
  private TextBoxStyle _textBoxStyle = new TextBoxStyle ();
  private Style _labelStyle = new Style ();
  private string _format = null;

  private string _errorMessage;
  private ArrayList _validators;

	public BocTextValue()
	{
    _textBox = new TextBox();
    _label = new Label ();
    _validators = new ArrayList();
	}

  protected override void CreateChildControls()
  {
    _textBox.ID = ID + "_Boc_TextBox";
    _textBox.EnableViewState = false;
    Controls.Add (_textBox);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
    _textBox.TextChanged += new EventHandler(TextBox_TextChanged);
  }

  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    //  The data control's changed event is sufficient.
  }

  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetRequestCollectionItem (Page, _textBox.UniqueID);
    bool isDataChanged = StringUtility.NullToEmpty (_text) != newValue;
    if (isDataChanged)
    {
      _text = newValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  private void TextBox_TextChanged (object sender, EventArgs e)
  {
    OnTextChanged (EventArgs.Empty);
  }

  /// <summary>
  /// Fires the <see cref="TextChanged"/> event.
  /// </summary>
  /// <param name="e"> Empty. </param>
  protected virtual void OnTextChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_textChangedEvent];
    if (eventHandler != null)
      eventHandler (this, e);
  }

  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    RefreshPropertiesFromObjectModel();
  }

  /// <summary>
  ///   Refreshes all properties of <see cref="BocTextValue"/> that depend on the current value of 
  ///   <see cref="IBusinessObjectBoundControl.Property"/>.
  /// </summary>
  private void RefreshPropertiesFromObjectModel()
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

  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      //Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        Value = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
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

  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      //Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    EnsureChildControlsPreRendered ();
    if (! IsDesignMode && ! IsReadOnly)
      Page.RegisterRequiresPostBack (this);
  }

  protected override void Render (HtmlTextWriter writer)
  {
    EnsureChildControlsPreRendered ();
    base.Render (writer);
  }

  protected override void PreRenderChildControls()
  {
    bool isReadOnly = IsReadOnly;
    _textBox.Visible = ! isReadOnly;
    _label.Visible = isReadOnly;
    if (isReadOnly)
    {
      _label.Text = _text;

      if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
      {
        _label.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }

      _label.Width = Width;
      _label.Height = Height;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      _textBox.Text = _text;
      //  Provide a default width
      _textBox.Width = Unit.Point (c_defaultTextBoxWidthInPoints);

      _textBox.ReadOnly = ! Enabled;
      if (Width != Unit.Empty)
        _textBox.Width = Width;
      _textBox.Height = Height;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
    }
  }

  /// <summary> This event is fired when the text is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the selection changes.")]
  public event EventHandler TextChanged
  {
    add { Events.AddHandler (s_textChangedEvent, value); }
    remove { Events.RemoveHandler (s_textChangedEvent, value); }
  }

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

    _textBox.Text = _text;
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
  public new object Value
  {
    get 
    {
      string text = _text;
      if (text != null)
        text = text.Trim();

      if (StringUtility.IsNullOrEmpty (text))
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
        _text = null;
        return;
      }

      IFormattable formattable = value as IFormattable;
      if (formattable != null)
      {
        string format = Format;
        if (format == null)
        {
          if (_actualValueType == BocTextValueType.Date)
            format = "d";
          else if (_actualValueType == BocTextValueType.DateTime)
            format = "g";
        }
        _text = formattable.ToString (format, null);
      }
      else
      {
        _text = value.ToString();
      }
    }
  }

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }


  [Description("Gets or sets the string representation of the current value.")]
  [Category("Data")]
  public string Text
  {
    get { return StringUtility.NullToEmpty (_text); }
    set { _text = value; }
  }

  [Description("Gets or sets a fixed value type.")]
  [Category("Data")]
  [DefaultValue (BocTextValueType.Undefined)]
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
  ///   Gets or sets the format string used to create the string value. 
  /// </summary>
  /// <remarks>
  ///   <see cref="IFormattable"/> is used to format the value using this string. The default is "d" for date-only
  ///   values and "g" for date/time values (use "G" to display seconds too).
  /// </remarks>
  [Description ("Gets or sets the format string used to create the string value.")]
  [Category ("Style")]
  [DefaultValue (null)]
  public string Format
  {
    get { return StringUtility.EmptyToNull (_format); }
    set { _format = value; }
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
      RefreshPropertiesFromObjectModel();
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

  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  public override BaseValidator[] CreateValidators()
  {
    string baseID = ID + "_ValidatorDateTime";
    ArrayList validators = new ArrayList(2);
    IResourceManager resourceManager = GetResourceManager();

    if (IsRequired)
    {
      RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
      requiredValidator.ID = baseID + "Required";
      requiredValidator.ControlToValidate = TargetControl.ID;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
        requiredValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.RequiredErrorMessage);
      else
        requiredValidator.ErrorMessage = _errorMessage;

      validators.Add (requiredValidator);
    }

    BocTextValueType valueType = ActualValueType;
    if (valueType == BocTextValueType.DateTime)
    {
      DateTimeValidator typeValidator = new DateTimeValidator();
      typeValidator.ID = baseID + "Type";
      typeValidator.ControlToValidate = TargetControl.ID;
      if (StringUtility.IsNullOrEmpty (_errorMessage))
        typeValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      else
        typeValidator.ErrorMessage = _errorMessage;
      validators.Add (typeValidator);
    }
    else if (valueType != BocTextValueType.Undefined && valueType != BocTextValueType.String)
    {
      CompareValidator typeValidator = new CompareValidator();
      typeValidator.ID = baseID + "Type";
      typeValidator.ControlToValidate = TargetControl.ID;
      typeValidator.Operator = ValidationCompareOperator.DataTypeCheck;
      switch (valueType)
      {
        case BocTextValueType.Date:
        {
          typeValidator.Type = ValidationDataType.Date;
          if (StringUtility.IsNullOrEmpty (_errorMessage))
            typeValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.InvalidDateErrorMessage);
          else
            typeValidator.ErrorMessage = _errorMessage;
          break;
        }
        case BocTextValueType.Integer:
        {
          typeValidator.Type = ValidationDataType.Integer;
          if (StringUtility.IsNullOrEmpty (_errorMessage))
            typeValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.InvalidIntegerErrorMessage);
          else
            typeValidator.ErrorMessage = _errorMessage;
          break;
        }
        case BocTextValueType.Double:
        {
          typeValidator.Type = ValidationDataType.Double;
          if (StringUtility.IsNullOrEmpty (_errorMessage))
            typeValidator.ErrorMessage = resourceManager.GetString (ResourceIdentifier.InvalidDoubleErrorMessage);
          else
            typeValidator.ErrorMessage = _errorMessage;
          break;
        }
        default:
        {
          throw new ArgumentException ("Cannot convert " + valueType.ToString() + " to type " + typeof (ValidationDataType).FullName + ".");
          break;
        }
      }
      validators.Add (typeValidator);
      _validators.AddRange (validators);
    }
    
    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }

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

  /// <summary>
  ///   Validation message if the control is not filled correctly.
  /// </summary>
  [Description("Validation message if the control is not filled correctly.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string ErrorMessage
  {
    get { return _errorMessage; }
    set 
    {
      _errorMessage = value; 
      foreach (BaseValidator validator in _validators)
        validator.ErrorMessage = _errorMessage;
    }
  }
}

public enum BocTextValueType
{
  Undefined,
  String,
  Integer,
  Date,
  DateTime,
  Double
}

}
