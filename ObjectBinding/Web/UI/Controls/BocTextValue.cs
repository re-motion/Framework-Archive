using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit values that can be edited in a text box. </summary>
/// <include file='doc\include\Controls\BocTextValue.xml' path='BocTextValue/Class/*' />
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocTextValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
  //  constants

  /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultTextBoxWidth = "150pt";

  //  statics

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectNumericProperty), typeof (IBusinessObjectStringProperty), typeof (IBusinessObjectDateProperty), typeof (IBusinessObjectDateTimeProperty) };

  private static readonly object s_textChangedEvent = new object();

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocTextValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
    RequiredErrorMessage,
    /// <summary> The validation error message displayed when the text exceeds the maximum length. </summary>
    MaxLengthValidationMessage,
    /// <summary> The validation error message displayed when the text is no valid date/time value. </summary>
    InvalidDateAndTimeErrorMessage,
    /// <summary> The validation error message displayed when the text is no valid date value. </summary>
    InvalidDateErrorMessage,
    /// <summary> The validation error message displayed when the text is no valid integer. </summary>
    InvalidIntegerErrorMessage,
    /// <summary> The validation error message displayed when the text is no valid integer. </summary>
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

  /// <summary> Initializes a new instance of the <b>BocTextValue</b> type. </summary>
	public BocTextValue()
	{
    _textBox = new TextBox();
    _label = new Label ();
    _validators = new ArrayList();
	}

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  protected override void CreateChildControls()
  {
    _textBox.ID = ID + "_Boc_TextBox";
    _textBox.EnableViewState = false;
    Controls.Add (_textBox);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
  }

  /// <summary> Overrides the <see cref="Control.OnInit"/> method. </summary>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
  }

  /// <summary> Calls the <see cref="LoadPostData"/> method. </summary>
  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    return LoadPostData (postDataKey, postCollection);
  }

  /// <summary> Calls the <see cref="RaisePostDataChangedEvent"/> method. </summary>
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  /// <summary>
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\Controls\BocTextValue.xml' path='BocTextValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetRequestCollectionItem (Page, _textBox.UniqueID);
    bool isDataChanged = newValue != null && StringUtility.NullToEmpty (_text) != newValue;
    if (isDataChanged)
    {
      _text = newValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnTextChanged (EventArgs.Empty);
  }

  /// <summary> Fires the <see cref="TextChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnTextChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_textChangedEvent];
    if (eventHandler != null)
      eventHandler (this, e);
  }

  /// <summary> Overrides the <see cref="Control.OnPreRender"/> method. </summary>
  /// <remarks> 
  ///   Calls <see cref="BusinessObjectBoundWebControl.EnsureChildControlsPreRendered"/>
  ///   and <see cref="Page.RegisterRequiresPostBack"/>.
  /// </remarks>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    EnsureChildControlsPreRendered ();
    if (! IsDesignMode && ! IsReadOnly && Enabled)
      Page.RegisterRequiresPostBack (this);
  }

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
  }

  /// <summary> Overrides the <see cref="Control.Render"/> method. </summary>
  /// <remarks> 
  ///   Calls <see cref="BusinessObjectBoundWebControl.EnsureChildControlsPreRendered"/>.
  /// </remarks>
  protected override void Render (HtmlTextWriter writer)
  {
    EnsureChildControlsPreRendered ();

    base.Render (writer);
  }

  /// <summary> Overrides the <see cref="Control.RenderChildren"/> method. </summary>
  protected override void RenderChildren(HtmlTextWriter writer)
  {
    if (IsReadOnly)
    {
      _label.RenderControl (writer);
    }
    else
    {
      bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
      bool isTextBoxHeightEmpty = StringUtility.IsNullOrEmpty (_textBox.Style["height"]);
      if (! isControlHeightEmpty && isTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isTextBoxWidthEmpty = StringUtility.IsNullOrEmpty (_textBox.Style["width"]);
      if (isTextBoxWidthEmpty)
      {
        if (isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultTextBoxWidth);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }

      _textBox.RenderControl (writer);
    }
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
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

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocTextValue.xml' path='BocTextValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        Value = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.SaveValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocTextValue.xml' path='BocTextValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.CreateValidators"/> method. </summary>
  /// <include file='doc\include\Controls\BocTextValue.xml' path='BocTextValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly)
      return new BaseValidator[0];

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
    if (valueType == BocTextValueType.String)
    {
      if (! _textBoxStyle.MaxLength.IsNull)
      {
        LengthValidator lengthValidator = new LengthValidator();
        lengthValidator.ID = ID + "_ValidatorMaxLength";
        lengthValidator.ControlToValidate = TargetControl.ID;
        lengthValidator.MaximumLength = _textBoxStyle.MaxLength.Value;
        if (StringUtility.IsNullOrEmpty (_errorMessage))
        {
          string maxLengthMessage = GetResourceManager().GetString (ResourceIdentifier.MaxLengthValidationMessage);
          lengthValidator.ErrorMessage = string.Format (maxLengthMessage, _textBoxStyle.MaxLength.Value);            
        }
        else
        {
          lengthValidator.ErrorMessage = _errorMessage;
        }      
        validators.Add (lengthValidator);
      }
    }
    else if (valueType == BocTextValueType.DateTime)
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
    else if (valueType != BocTextValueType.Undefined)
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
        }
      }
      validators.Add (typeValidator);
      _validators.AddRange (validators);
    }
    
    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.PreRenderChildControls"/> method. </summary>
  protected override void PreRenderChildControls()
  {
    if (IsReadOnly)
    {
      string text = HttpUtility.HtmlEncode (_text);
      if (StringUtility.IsNullOrEmpty (text))
      {
        if (IsDesignMode)
        {
          text = c_designModeEmptyLabelContents;
          //  Too long, can't resize in designer to less than the content's width
          //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
        }
        else
        {
          text = "&nbsp;";
        }
      }
      _label.Text = text;
      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      _textBox.Text = _text;

      _textBox.ReadOnly = ! Enabled;
      _textBox.Width = Unit.Empty;
      _textBox.Height = Unit.Empty;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
    }
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    RefreshPropertiesFromObjectModel();
  }

  /// <summary>
  ///   Refreshes all properties of <see cref="BocTextValue"/> that depend on the current value of 
  ///   <see cref="BusinessObjectBoundWebControl.Property"/>.
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

  /// <summary> Returns the proper <see cref="BocTextValueType"/> for the passed <see cref="IBusinessObjectProperty"/>. </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to analyze. </param>
  /// <exception cref="NotSupportedException"> The specialized type of the <paremref name="property"/> is not supported. </exception>
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

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The value has the type specified in the <see cref="ValueType"/> property (<see cref="String"/>, 
  ///   <see cref="Int32"/>, <see cref="Double"/> or <see cref="DateTime"/>). If <see cref="ValueType"/> is not
  ///   set, the type is determined by the bound <see cref="BusinessObjectBoundWebControl.Property"/>.
  /// </value>
  /// <exception cref="FormatException"> 
  ///   The value of the <see cref="Text"/> property cannot be converted to the specified <see cref="ValueType"/>.
  /// </exception>
  [Description("Gets or sets the current value.")]
  [Browsable (false)]
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

  /// <summary>
  ///   Gets a flag describing whether it is save (i.e. accessing <see cref="Value"/> does not throw a 
  ///   <see cref="FormatException"/> or <see cref="OverflowException"/>) to read the contents of <see cref="Value"/>.
  /// </summary>
  /// <remarks> Valid values include <see langword="null"/>. </remarks>
  [Browsable(false)]
  public bool IsValidValue
  {
    get
    {
      try
      {
        //  Force the evaluation of Value
        if (Value != null)
          return true;
      }
      catch (FormatException)
      {
        return false;
      }
      catch (OverflowException)
      {
        return false;
      }

      return true;
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.ValueImplementation"/> property. </summary>
 protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> Gets or sets the string representation of the current value. </summary>
  /// <value> A string. The default value is <see cref="String.Empty"/>. </value>
  [Description("Gets or sets the string representation of the current value.")]
  [Category("Data")]
  [DefaultValue ("")]
  public string Text
  {
    get { return StringUtility.NullToEmpty (_text); }
    set { _text = value; }
  }

  /// <summary> Gets or sets the <see cref="BocTextValueType"/> assigned from an external source. </summary>
  /// <value> 
  ///   The externally set <see cref="BocTextValueType"/>. The default value is 
  ///   <see cref="BocTextValueType.Undefined"/>. 
  /// </value>
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
  ///   Gets the controls fixed <see cref="ValueType"/> or, if <see cref="BocTextValueType.Undefined"/>, 
  ///   the <see cref="BusinessObjectBoundWebControl.Property"/>'s value type.
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocTextValueType ActualValueType
  {
    get 
    {
      if (_valueType == BocTextValueType.Undefined && Property != null)
        _actualValueType = GetBocTextValueType (Property);
      return _actualValueType;
    }
  }

  /// <summary> Gets or sets the format string used to create the string value.  </summary>
  /// <value> 
  ///   A string passed to the <b>ToString</b> method of the object returned by <see cref="Value"/>.
  ///   The default value is <see cref="String.Empty"/>. 
  /// </value>
  /// <remarks>
  ///   <see cref="IFormattable"/> is used to format the value using this string. The default is "d" for date-only
  ///   values and "g" for date/time values (use "G" to display seconds too). 
  /// </remarks>
  [Description ("Gets or sets the format string used to create the string value. Format must be parsable by the value's type if the control is in edit mode.")]
  [Category ("Style")]
  [DefaultValue ("")]
  public string Format
  {
    get { return StringUtility.EmptyToNull (_format); }
    set { _format = value; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <remarks> Returns the <see cref="TextBox"/> if the control is in edit mode, otherwise the control itself. </remarks>
  public override Control TargetControl
  {
    get { return (_textBox == null) ? (Control) this : _textBox; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.IsDirty"/> property. </summary>
  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/> property. </summary>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return true; }
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

  /// <summary> This event is fired when the text is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler TextChanged
  {
    add { Events.AddHandler (s_textChangedEvent, value); }
    remove { Events.RemoveHandler (s_textChangedEvent, value); }
  }

  /// <summary>
  ///   The style that you want to apply to the <see cref="TextBox"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
  ///   the respective modes. Note that if you set one of the <b>Font</b> attributes (Bold, Italic etc.) to 
  ///   <see langword="true"/>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
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

  /// <summary> The style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public TextBoxStyle TextBoxStyle
  {
    get { return _textBoxStyle; }
  }

  /// <summary> The style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }


  /// <summary> Gets or sets the validation error message. </summary>
  /// <value> 
  ///   The error message displayed when validation fails. The default value is <see cref="String.Empty"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("Validation message displayed if there is an error.")]
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

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocTextValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocTextValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocTextValue"; } }
  #endregion
}

/// <summary> A list possible data types for the <see cref="BocTextValue"/> </summary>
public enum BocTextValueType
{
  /// <summary> 
  ///   Format the value as it's default string representation. 
  ///   No parsing is possible, <see cref="P:BoxTextValue.Value"/> will return a string. 
  /// </summary>
  Undefined,
  /// <summary> Interpret the value as a <see cref="String"/>. </summary>
  String,
  /// <summary> Interpret the value as an <see cref="Int32"/>. </summary>
  Integer,
  /// <summary> Interpret the value as a <see cref="DateTime"/> with the time component set to zero. </summary>
  Date,
  /// <summary> Interpret the value as a <see cref="DateTime"/>. </summary>
  DateTime,
  /// <summary> Interpret the value as a <see cref="Double"/>. </summary>
  Double
}

}
