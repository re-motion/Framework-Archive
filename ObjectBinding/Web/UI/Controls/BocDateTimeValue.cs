using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

public enum BocDateTimeValueType
{
  Undefined,
  DateTime,
  Date
}

  /// <summary>
/// </summary>
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocDateTimeValue: BusinessObjectBoundModifiableWebControl
{
  //  constants

  // types

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectDateTimeProperty), typeof (IBusinessObjectDateProperty) };

  /// <summary> This event is fired when the date is changed in the UI. </summary>
  /// <remarks> The event is fired only if the date change is caused by the user. </remarks>
  public event EventHandler DateTimeChanged;

	// member fields

//  private BocTextValueType _valueType = BocTextValueType.Undefined;
//  private BocTextValueType _actualValueType = BocTextValueType.Undefined;

//  TODO: current/new values
//  private string _text = string.Empty;
//  private string _newText = null;

  private bool _isDirty = true;

  private TextBox _dateTextBox = null;
  private TextBox _timeTextBox = null;
  private Label _label = null;
  private ImageButton _imageButton = null;

  /// <summary> 
  ///   The <see cref=""/> of the current object.
  /// </summary>
  private NaDateTime _internalValue = NaDateTime.Null;

  /// <summary>  </summary>
  private string _newInternalDateValue = null;

  /// <summary>  </summary>
  private string _newInternalTimeValue = null;

  private Style _commonStyle = new Style();
  private SingleRowTextBoxStyle _dateTimeTextBoxStyle = new SingleRowTextBoxStyle();
  private SingleRowTextBoxStyle _dateTextBoxStyle = new SingleRowTextBoxStyle();
  private SingleRowTextBoxStyle _timeTextBoxStyle = new SingleRowTextBoxStyle();
  private Style _labelStyle = new Style();
  private Style _buttonStyle = new Style();

  private bool _showSeconds = false;
  private BocDateTimeValueType _valueType = BocDateTimeValueType.Undefined;

  // construction and disposing

	public BocDateTimeValue()
	{
    //  empty

    //  Moved to OnInit
    //    _dateTextBox = new BocTextBox (this);
    //    _timeTextBox = new BocTextBox (this);
    //    _dateLabel = new Label();
    //    _timeLabel = new Label();
	}

	// methods and properties

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _dateTextBox = new TextBox();
    _timeTextBox = new TextBox();
    _label = new Label();
    _imageButton = new ImageButton();

    _dateTextBox.ID = this.ID + "_DateTextBox";
    _dateTextBox.EnableViewState = false;
    Controls.Add (_dateTextBox);

    _timeTextBox.ID = this.ID + "_TimeTextBox";
    _timeTextBox.EnableViewState = false;
    Controls.Add (_timeTextBox);

    _label.ID = this.ID + "_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);

    _imageButton.ID = this.ID + "_ImageButton";
    _imageButton.EnableViewState = false;
    Controls.Add (_imageButton);

    //  Moved to OnLoad
    //    if (! IsDesignMode)
    //    {
    //      string newValue = this.Page.Request.Form[_textBox.UniqueID];
    //      if (newValue != null)
    //        _newText = newValue;
    //    }

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
  }

  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    if (! IsDesignMode)
    {
      //  Date input field

      string newInternalDateValue = this.Page.Request.Form[_dateTextBox.UniqueID];

      if (newInternalDateValue == "")
        _newInternalDateValue = null;
      else if (newInternalDateValue != null)
        _newInternalDateValue = newInternalDateValue;
      else
        _newInternalDateValue = null;
      if (    newInternalDateValue != null 
          &&  _newInternalDateValue != null
          && _internalValue != NaDateTime.Null)
      {
        try 
        {
          NaDateTime newDateValue = NaDateTime.Parse (newInternalDateValue);

          if (newDateValue.Date != _internalValue.Date)
            _isDirty = true;
        }
        catch (FormatException)
        {}
      }

      //  Time input field

      string newInternalTimeValue = this.Page.Request.Form[_timeTextBox.UniqueID];
        
      if (newInternalTimeValue == "")
        _newInternalTimeValue = null;
      else if (newInternalTimeValue != null)
        _newInternalTimeValue = newInternalTimeValue;
      else
        _newInternalTimeValue = null;

      if (    newInternalTimeValue != null 
          &&  _newInternalTimeValue != null 
          &&  _internalValue != NaDateTime.Null)
      {
        try 
        {
          NaDateTime newTimeValue = NaDateTime.Parse (newInternalTimeValue);

          if (newTimeValue.Time != _internalValue.Time)
            _isDirty = true;
        }
        catch (FormatException)
        {}
      }
    }
  }


  /// <summary>
  ///   Calls the parent's <c>OnPreRender</c> method and ensures that the sub-controls are 
  ///   properly initialized.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    //  First call
    EnsureChildControlsInitialized ();
  }

  /// <summary>
  ///   Calls the parent's <c>Render</c> method and ensures that the sub-controls are 
  ///   properly initialized.
  /// </summary>
  /// <param name="writer"> 
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content. 
  /// </param>
  protected override void Render (HtmlTextWriter writer)
  {
    //  Second call has practically no overhead
    //  Required to get optimum designer support.
    EnsureChildControlsInitialized ();

    base.Render (writer);
  }

  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    _internalValue = (NaDateTime) values[1];
    _valueType = (BocDateTimeValueType) values[2];
    _isDirty = (bool) values[3];
  }

  protected override object SaveViewState()
  {
    object[] values = new object[4];
    values[0] = base.SaveViewState();
    values[1] = _internalValue;
    values[2] = _valueType;
    values[3] = _isDirty;
    return values;
  }

  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        Value = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
    }
  }

  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      Binding.EvaluateBinding();
      if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  //  private BocTextValueType GetBocTextValueType (IBusinessObjectProperty property)
  //  {
  //    if (property is IBusinessObjectStringProperty)
  //      return BocTextValueType.String;
  //    else if (property is IBusinessObjectInt32Property)
  //      return BocTextValueType.Integer;
  //    else if (property is IBusinessObjectDoubleProperty)
  //      return BocTextValueType.Double;
  //    else if (property is IBusinessObjectDateProperty)
  //      return BocTextValueType.Date;
  //    else if (property is IBusinessObjectDateTimeProperty)
  //      return BocTextValueType.DateTime;
  //    else
  //      throw new NotSupportedException ("BocTextValue does not support property type " + property.GetType());
  //  }

  /// <summary>
  ///   Generates the validators depending on the control's configuration.
  /// </summary>
  /// <remarks>
  ///   
  /// </remarks>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public override BaseValidator[] CreateValidators()
  {
    if (! IsRequired)
      return new BaseValidator[]{};

    BaseValidator[] validators = new BaseValidator[0];

    //  TODO: validation

    return validators;
  }

  protected override void InitializeChildControls()
  {
    bool isReadOnly = IsReadOnly;

    _dateTextBox.Visible = ! isReadOnly;
    _timeTextBox.Visible = ! isReadOnly;
    _label.Visible = isReadOnly;
    _imageButton.Visible = ! isReadOnly;

    //  TODO: Initialize Child Controls
    if (isReadOnly)
    {
      if (IsDesignMode &&  StringUtility.IsNullOrEmpty (_label.Text))
      {
        //  nothing
        //  _dateLabel.Text = c_nullDisplayName;
        //  _dateLabel.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        //  TODO: DateTime / Date switching
        if (InternalValue != NaDateTime.Null)
        {
          _label.Text = InternalValue.Value.ToString();
        }
        else
        {
          _label.Text = "";
        }
      }

      //      _label.Width = this.Width;
      //      _label.Height = this.Height;
      
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      if (    IsDesignMode 
          &&  StringUtility.IsNullOrEmpty (_dateTextBox.Text)
          &&  StringUtility.IsNullOrEmpty (_timeTextBox.Text))
      {
      }
      else
      {
        //  TODO: DateTime / Date switching
        if (InternalValue != NaDateTime.Null)
        {
          _dateTextBox.Text = InternalValue.Date.ToShortDateString();
          _timeTextBox.Text = InternalValue.Time.ToString();
        }
        else
        {
          _dateTextBox.Text = "";
          _timeTextBox.Text = "";
        }
      }

      string imageUrl = ResourceUrlResolver.GetResourceUrl (
        this, 
        typeof (BocDateTimeValue), 
        ResourceType.Image, 
        ImageButtonImageUrl);

      if (imageUrl == null)
        _imageButton.ImageUrl = ImageButtonImageUrl;  
      else
        _imageButton.ImageUrl = imageUrl;

      _dateTextBox.Style["vertical-align"] = "bottom";
      _timeTextBox.Style["vertical-align"] = "bottom";
      _imageButton.Style["vertical-align"] = "middle";

      //      _textBox.Width = this.Width;
      //      _textBox.Height = this.Height;

      _dateTextBox.ApplyStyle (_commonStyle);
      _dateTextBoxStyle.ApplyStyle (_dateTextBox);
      _dateTextBoxStyle.ApplyStyle (_dateTextBox);

      _timeTextBox.ApplyStyle (_commonStyle);
      _timeTextBoxStyle.ApplyStyle (_timeTextBox);
      _timeTextBoxStyle.ApplyStyle (_timeTextBox);

      _imageButton.ApplyStyle (_commonStyle);
      _imageButton.ApplyStyle (_buttonStyle);
    }
  }


  /// <summary>
  /// Fires the <see cref="DateTimeChanged"/> event.
  /// </summary>
  /// <param name="e"> Empty. </param>
  protected virtual void OnDateTimeChanged (EventArgs e)
  {
    // _isDirty = true; // moved to OnLoad

    //  TODO: fire DateTimeChanged
    //    if (DateTimeChanged != null)
    //      DateTimeChanged (this, e);
  }

  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    if (Property != null)
    {
      _valueType = GetBocDateTimeValueType (Property);
    }
    else
    {
      //  Test if usefull
      //_valueType = BocDateTimeValueType.Undefined;
    }
  }


  private BocDateTimeValueType GetBocDateTimeValueType (IBusinessObjectProperty property)
  {
    if (property is IBusinessObjectDateTimeProperty)
      return BocDateTimeValueType.DateTime;
    else if (property is IBusinessObjectInt32Property)
      return BocDateTimeValueType.Date;
    else
      throw new NotSupportedException ("BocDateTimeValue does not support property type " + property.GetType());
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
      if (InternalValue != NaDateTime.Null)
        return _internalValue.Value; 
      else
        return null;
    }
    set 
    {
      if (value == null)
      {
        InternalValue = NaDateTime.Null;
        return;
      }

      try
      {
        switch (_valueType)
        {
          case BocDateTimeValueType.DateTime:
          {
            InternalValue = (DateTime) value;
            break;
          }
          case BocDateTimeValueType.Date:
          {
            InternalValue = (DateTime) value;
            break;
          }
          case BocDateTimeValueType.Undefined:
          {
            InternalValue = NaDateTime.Null;
            break;
          }
          default:
          {
            InternalValue = NaDateTime.Null;
            break;
          }
        }
      }
      catch  (InvalidCastException e)
      {
        throw new ArgumentException ("Expected type '" + _valueType.ToString() + "', but was '" + value.GetType().FullName + "'.", "value", e);
      }
    }
  }

  protected virtual NaDateTime InternalValue
  {
    get{ return _internalValue; }
    set
    {
      if (_internalValue == value)
        return;

      _internalValue = value;
    }
  }

  //  [Description("Gets or sets the string representation of the current value.")]
  //  [Category("Data")]
  //  public string Text
  //  {
  //    get { return _text; }
  //    set { _text = StringUtility.NullToEmpty (value); }
  //  }

  //  [Description("Gets or sets a fixed value type.")]
  //  [Category("Data")]
  //  [DefaultValue(typeof(BocTextValueType), "Undefined")]
  //  public BocTextValueType ValueType
  //  {
  //    get { return _valueType; }
  //    set 
  //    {
  //      if (_valueType != value)
  //      {
  //        _valueType = value;
  //        _actualValueType = value;
  //        if (_valueType != BocTextValueType.Undefined)
  //          _text = string.Empty;
  //      }
  //    }
  //  }

  //  /// <summary>
  //  /// Gets the controls fixed value type or, if undefined, the property's value type.
  //  /// </summary>
  //  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  //  [Browsable (false)]
  //  public BocTextValueType ActualValueType
  //  {
  //    get 
  //    {
  //      Binding.EvaluateBinding();
  //      return _actualValueType;
  //    }
  //  }

  public override Control TargetControl
  {
    get { return (_dateTextBox != null) ? _dateTextBox : (Control) this; }
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

  /// <summary> Overrides <see cref="Rubicon.Web.UI.Controls.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return true; }
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
  ///   The style that you want to apply to both the date and the time TextBoxes (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to both the date and the time TextBoxes (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle DateTimeTextBoxStyle
  {
    get { return _dateTimeTextBoxStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the date TextBox (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to only the date TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle DateTextBoxStyle
  {
    get { return _dateTextBoxStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the time TextBox (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to only the time TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle TimeTextBoxStyle
  {
    get { return _timeTextBoxStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the Label
  ///   (read-only mode) only.
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

  /// <summary>
  ///   The style that you want to apply to the Button (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Button (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style ButtonStyle
  {
    get { return _buttonStyle; }
  }

  [Browsable (false)]
  public TextBox DateTextBox
  {
    get { return _dateTextBox; }
  }

  [Browsable (false)]
  public TextBox TimeTextBox
  {
    get { return _timeTextBox; }
  }

  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  [Browsable (false)]
  public ImageButton ImageButton
  {
    get { return _imageButton; }
  }

  public bool ShowSeconds
  {
    get { return _showSeconds; }
    set { _showSeconds = value; }
  }

  public BocDateTimeValueType ValueType
  {
    get { return _valueType; }
  }

  protected virtual string ImageButtonImageUrl
  {
    get { return "Calendar.gif"; }
  }
}

}
