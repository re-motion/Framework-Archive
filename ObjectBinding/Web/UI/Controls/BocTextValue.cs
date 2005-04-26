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

  /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultTextBoxWidth = "150pt";

  //  statics

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectNumericProperty), typeof (IBusinessObjectStringProperty), typeof (IBusinessObjectDateProperty), typeof (IBusinessObjectDateTimeProperty) };

  private static readonly object s_textChangedEvent = new object();

  // types

  /// <summary> A list of control wide resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:IResourceManager.GetString (Enum)">IResourceManager.GetString (Enum)</see>. 
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocTextValue")]
  protected enum ResourceIdentifier
  {
    RequiredErrorMessage,
    MaxLengthValidationMessage,
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
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed between
  ///   post backs.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Sets the new value and the <see cref="IsDirty"/> flag if the value has changed.
  ///   </para><para>
  ///     Evaluates the value of the <see cref="TextBox"/>.
  ///   </para>
  ///   <note type="inheritinfo">
  ///     Overrive this method to change the way a data change is detected of the value is read from the 
  ///     <paramref name="postCollection"/>.
  ///   </note>
  /// </remarks>
  /// <param name="postDataKey"> The key identifier for this control. </param>
  /// <param name="postCollection"> The collection of all incoming name values.  </param>
  /// <returns>
  ///   <see langword="true"/> if the server control's state changes as a result of the post back; 
  ///   otherwise <see langword="false"/>.
  /// </returns>
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

  /// <summary> Called when the state of the control has changed between post backs. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnTextChanged (EventArgs.Empty);
  }

  /// <summary> Fires the <see cref="TextChanged"/> event. </summary>
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

  protected override void Render (HtmlTextWriter writer)
  {
    EnsureChildControlsPreRendered ();

    base.Render (writer);
  }

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

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }


  [Description("Gets or sets the string representation of the current value.")]
  [Category("Data")]
  public string Text
  {
    get 
    {
      return StringUtility.NullToEmpty (_text);
    }
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
  [Description ("Gets or sets the format string used to create the string value. Format must be parsable by the value's type if the control is in edit-mode.")]
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

  public override bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
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

  /// <summary> Gets or sets the validation error message. </summary>
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
