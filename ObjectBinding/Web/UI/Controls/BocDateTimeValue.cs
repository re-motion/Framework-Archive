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
using Rubicon.Web.UI.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit date/time values. </summary>
/// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/Class/*' />
//  TODO: Date-Picker
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocDateTimeValue: BusinessObjectBoundModifiableWebControl
{
  //  constants

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyLabelContents = "#";
  /// <summary> String inserted between the date and the time text boxes. </summary>
  private const string c_dateTimeSpacer = "&nbsp;";
  /// <summary> String inserted before the date pciker button. </summary>
  private const string c_imageButtonSpacer = "&nbsp;";
  /// <summary> String inserted between the date and the time text boxes during design mode. </summary>
  private const string c_designModeDateTimeSpacer = " ";
  /// <summary> String inserted before the date pciker button during design mode. </summary>
  private const string c_designModeImageButtonSpacer = " ";

  const int c_defaultDateTextBoxWidthInPoints = 75;
  const int c_defaultTimeTextBoxWidthInPoints = 50;

  private const string c_requiredErrorMessage = "Please enter a value.";
  private const string c_incompleteErrorMessage = "Please enter a date.";
  private const string c_invalidDateAndTimeErrorMessage = "Unknown date and time format.";
  private const string c_invalidDateErrorMessage = "Unknown date format.";
  private const string c_invalidTimeErrorMessage = "Unknown time format.";

  // types

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectDateTimeProperty), typeof (IBusinessObjectDateProperty) };

	// member fields

  /// <summary> This event is fired when the date or time is changed in the UI. </summary>
  /// <remarks> The event is fired only if the date change is caused by the user. </remarks>
  public event EventHandler DateTimeChanged;

  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="TextBox"/> used in edit mode for the date component. </summary>
  private TextBox _dateTextBox = null;

  /// <summary> The <see cref="TextBox"/> used in edit mode for the time component. </summary>
  private TextBox _timeTextBox = null;

  /// <summary> The <see cref="Label"/> used in read-only mode. </summary>
  private Label _label = null;

  /// <summary> The <see cref="ImageButton"/> used in edit mode to enter the date using a date picker. </summary>
  private ImageButton _imageButton = null;

  /// <summary> The string displayed in the date text box. </summary>
  private string _internalDateValue = null;

  /// <summary>  The string displayed in the time text box. </summary>
  private string _internalTimeValue = null;

  /// <summary> The string enterd into the date text box by the user. </summary>
  private string _newInternalDateValue = null;

  /// <summary> The string enterd into the time text box by the user. </summary>
  private string _newInternalTimeValue = null;

  /// <summary> A backup of the <see cref="DateTime"/> value. </summary>
  private NaDateTime _savedDateTimeValue = NaDateTime.Null;

  /// <summary> The externally set <see cref="BocDateTimeValueType"/>. </summary>
  private BocDateTimeValueType _valueType = BocDateTimeValueType.Undefined;

  /// <summary> The <see cref="BocDateTimeValueType"/> this control is actually displaying. </summary>
  private BocDateTimeValueType _actualValueType = BocDateTimeValueType.Undefined;

  /// <summary> The <see cref="Style"/> applied the textboxes and the label. </summary>
  private Style _commonStyle = new Style();

  /// <summary> The <see cref="SingleRowTextBoxStyle"/> applied to both text boxes. </summary>
  private SingleRowTextBoxStyle _dateTimeTextBoxStyle = new SingleRowTextBoxStyle();

  /// <summary> The <see cref="SingleRowTextBoxStyle"/> applied to the <see cref="DateTextBox"/>. </summary>
  private SingleRowTextBoxStyle _dateTextBoxStyle = new SingleRowTextBoxStyle();

  /// <summary> The <see cref="SingleRowTextBoxStyle"/> applied to the <see cref="TimeTextBox"/>. </summary>
  private SingleRowTextBoxStyle _timeTextBoxStyle = new SingleRowTextBoxStyle();

  /// <summary> The <see cref="Style"/> applied to the <see cref="Label"/>. </summary>
  private Style _labelStyle = new Style();

  /// <summary> The <see cref="Style"/> applied to the <see cref="ImageButton"/>. </summary>
  private Style _imageButtonStyle = new Style();

  /// <summary> Flag that determines  whether to show the seconds.</summary>
  private bool _showSeconds = true;

  /// <summary> 
  ///   Flag that determines whether to provide an automatic maximun length for the text boxes.
  /// </summary>
  private bool _provideMaxLength = true;

  // construction and disposing

  /// <summary> Simple constructor. </summary>
	public BocDateTimeValue()
	{
    //  empty
	}

	// methods and properties

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
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

    _imageButton.ID = this.ID + "_ImageButton";
    _imageButton.EnableViewState = false;
    Controls.Add (_imageButton);

    _timeTextBox.ID = this.ID + "_TimeTextBox";
    _timeTextBox.EnableViewState = false;
    Controls.Add (_timeTextBox);

    _label.ID = this.ID + "_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
    _dateTextBox.TextChanged += new EventHandler (DateTimeTextBoxes_TextChanged);
    _timeTextBox.TextChanged += new EventHandler (DateTimeTextBoxes_TextChanged);
  }

  /// <summary>
  ///   Calls the parent's <c>OnLoad</c> method and prepares the binding information.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    Binding.EvaluateBinding();

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

      if (newInternalDateValue != null && _newInternalDateValue != _internalDateValue)
            _isDirty = true;


      //  Time input field

      string newInternalTimeValue = this.Page.Request.Form[_timeTextBox.UniqueID];
        
      if (newInternalTimeValue == "")
        _newInternalTimeValue = null;
      else if (newInternalTimeValue != null)
        _newInternalTimeValue = newInternalTimeValue;
      else
        _newInternalTimeValue = null;

      if (newInternalTimeValue != null && _newInternalTimeValue != _internalTimeValue)
        _isDirty = true;
    }
  }

  /// <summary>
  /// Fires the <see cref="DateTimeChanged"/> event.
  /// </summary>
  /// <param name="e"> Empty. </param>
  protected virtual void OnDateTimeChanged (EventArgs e)
  {
    if (DateTimeChanged != null)
      DateTimeChanged (this, e);
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

  /// <summary>
  ///   Calls the parents <c>LoadViewState</c> method and restores this control's specific data.
  /// </summary>
  /// <param name="savedState">
  ///   An <see cref="Object"/> that represents the control state to be restored.
  /// </param>
  protected override void LoadViewState(object savedState)
  {
    object[] values = (object[]) savedState;
    
    base.LoadViewState (values[0]);

    if ( values[1] != null)
      _internalDateValue = (string) values[1];
    if ( values[2] != null)
      _internalTimeValue = (string) values[2];
    _valueType = (BocDateTimeValueType) values[3];
    _actualValueType = (BocDateTimeValueType) values[4];
    _showSeconds = (bool) values[5];
    _provideMaxLength = (bool) values[6];
    _savedDateTimeValue = (NaDateTime) values[7];
    _isDirty = (bool) values[8];
  }

  /// <summary>
  ///   Calls the parents <c>SaveViewState</c> method and saves this control's specific data.
  /// </summary>
  /// <returns>
  ///   Returns the server control's current view state.
  /// </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[9];

    values[0] = base.SaveViewState();
    values[1] = _internalDateValue;
    values[2] = _internalTimeValue;
    values[3] = _valueType;
    values[4] = _actualValueType;
    values[5] = _showSeconds;
    values[6] = _provideMaxLength;
    values[7] = _savedDateTimeValue;
    values[8] = _isDirty;

    return values;
  }

  /// <summary>
  ///   Loads the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> or uses the cached
  ///   information if <paramref name="interim"/> is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="true"/> to load the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
    }
  }

  /// <summary>
  ///   Writes the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> if <paramref name="interim"/> 
  ///   is <see langword="true"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to write the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      Binding.EvaluateBinding();
      if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
      {
        DataSource.BusinessObject.SetProperty (Property, Value);

        //  get_Value parses the internal representation of the date/time value
        //  set_Value updates the internal representation of the date/time value
        Value = Value;
      }
    }
  }

  /// <summary>
  ///   Generates a <see cref="BocDateTimeValueValidator"/>.
  /// </summary>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public override BaseValidator[] CreateValidators()
  {
    BaseValidator[] validators = new BaseValidator[1];

    BocDateTimeValueValidator dateTimeValidator = new BocDateTimeValueValidator();

    dateTimeValidator.ID = this.ID + "_ValidatorDateTime";
    dateTimeValidator.ControlToValidate = ID;
    //  TODO: Get Message from ResourceProvider
    dateTimeValidator.RequiredErrorMessage = c_requiredErrorMessage;
    dateTimeValidator.IncompleteErrorMessage = c_incompleteErrorMessage;
    dateTimeValidator.InvalidDateAndTimeErrorMessage = c_invalidDateAndTimeErrorMessage;
    dateTimeValidator.InvalidDateErrorMessage = c_invalidDateErrorMessage;
    dateTimeValidator.InvalidTimeErrorMessage = c_invalidTimeErrorMessage;

    validators[0] = dateTimeValidator;

    //  No validation that only enabled enum values get selected and saved.
    //  This behaviour mimics the Fabasoft enum behaviour

    return validators;
  }

  /// <summary> Initializes the child controls. </summary>
  protected override void InitializeChildControls()
  {
    bool isReadOnly = IsReadOnly;

    _dateTextBox.Visible = ! isReadOnly;
    _timeTextBox.Visible = ! isReadOnly;
    _label.Visible = isReadOnly;
    _imageButton.Visible = ! isReadOnly;

    if (isReadOnly)
    {
      if (IsDesignMode &&  StringUtility.IsNullOrEmpty (_label.Text))
      {
        _label.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        object internalValue = Value;

        if (internalValue != null)
        {
          DateTime dateTime = (DateTime)internalValue;

          if (ActualValueType == BocDateTimeValueType.DateTime)
            _label.Text = FormatDateTimeValue (dateTime, isReadOnly);
          else if (ActualValueType == BocDateTimeValueType.Date)
            _label.Text = FormatDateValue (dateTime, isReadOnly);
          else
            _label.Text = string.Empty;
        }
        else
        {
          _label.Text = string.Empty;
        }
      }

      _label.Width = this.Width;
      _label.Height = this.Height;
      
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else // Edit Mode
    {
      const double dateTextBoxWidthRelative = (double) c_defaultDateTextBoxWidthInPoints 
        / (c_defaultDateTextBoxWidthInPoints + c_defaultTimeTextBoxWidthInPoints);
      
      const double timeTextBoxWidthRelative = (double) c_defaultTimeTextBoxWidthInPoints 
        / (c_defaultDateTextBoxWidthInPoints + c_defaultTimeTextBoxWidthInPoints);

      if (ProvideMaxLength)
      {
        _dateTextBox.MaxLength = GetDateMaxLength();
        _timeTextBox.MaxLength = GetTimeMaxLength();
      }

      _dateTextBox.Text = InternalDateValue;
      _timeTextBox.Text = InternalTimeValue;

      //  Prevent a collapsed control
      _dateTextBox.Width = Unit.Point (c_defaultDateTextBoxWidthInPoints);
      _timeTextBox.Width = Unit.Point (c_defaultTimeTextBoxWidthInPoints);

      //  Insert Spacer between Date and Time text boxes
      
      LiteralControl dateTimeSpacer = null;

      if (    ActualValueType == BocDateTimeValueType.DateTime
          ||  IsDesignMode && ActualValueType == BocDateTimeValueType.Undefined)
      {
        int timeTextBoxIndex = Controls.IndexOf (_timeTextBox);

        if (! IsDesignMode)
          dateTimeSpacer = new LiteralControl (c_dateTimeSpacer);
        else
          dateTimeSpacer = new LiteralControl (c_designModeDateTimeSpacer);

        Controls.AddAt (timeTextBoxIndex, dateTimeSpacer);
      }


      //  Insert Spacer before image button

      LiteralControl imageButtonSpacer = null;

      if (    ActualValueType == BocDateTimeValueType.DateTime
          ||  ActualValueType == BocDateTimeValueType.Date
          ||  IsDesignMode && ActualValueType == BocDateTimeValueType.Undefined)
      {
        int imageButtonIndex = Controls.IndexOf (_imageButton);

        if (! IsDesignMode)
          imageButtonSpacer = new LiteralControl (c_imageButtonSpacer);
        else
          imageButtonSpacer = new LiteralControl (c_designModeImageButtonSpacer);

        Controls.AddAt (imageButtonIndex, imageButtonSpacer);
      }

      Unit dateTextBoxWidth = Unit.Empty;
      Unit timeTextBoxWidth = Unit.Empty;

      if (! Width.IsEmpty)
      {
        int imageButtonWidth = 0;

        //  Icon width approximation
        switch (Width.Type)
        {
          case UnitType.Percentage:
          {
            imageButtonWidth = 20;
            break;
          }
          case UnitType.Pixel:
          {
            imageButtonWidth = 30;
            break;
          }
          case UnitType.Point:
          {
            imageButtonWidth = 15;
            break;
          }
          default:
          {
            break;
          }
        }

        int innerControlWidthValue = (int) (Width.Value - imageButtonWidth);
        innerControlWidthValue = (innerControlWidthValue > 0) ? innerControlWidthValue : 0;

        int dateTextBoxWidthValue = 0;
        int timeTextBoxWidthValue = 0;

        //  Calculate the widths
        if (ActualValueType == BocDateTimeValueType.DateTime)
        {
          dateTextBoxWidthValue = (int)(innerControlWidthValue * dateTextBoxWidthRelative);            
          timeTextBoxWidthValue = (int)(innerControlWidthValue * timeTextBoxWidthRelative);
        }
        else if (ActualValueType == BocDateTimeValueType.Date)
        {
          dateTextBoxWidthValue = innerControlWidthValue;            
        }
        else if (     IsDesignMode
                  &&  ActualValueType == BocDateTimeValueType.Undefined)
        {
          dateTextBoxWidthValue = (int)(innerControlWidthValue * dateTextBoxWidthRelative);            
          timeTextBoxWidthValue = (int)(innerControlWidthValue * timeTextBoxWidthRelative);
        }
         
        //  Assign the widths
        switch (Width.Type)
        {
          case UnitType.Percentage:
          {
            dateTextBoxWidth = Unit.Percentage (dateTextBoxWidthValue);            
            timeTextBoxWidth = Unit.Percentage (timeTextBoxWidthValue);            
            break;
          }
          case UnitType.Pixel:
          {
            dateTextBoxWidth = Unit.Pixel (dateTextBoxWidthValue);            
            timeTextBoxWidth = Unit.Pixel (timeTextBoxWidthValue);            
            break;
          }
          case UnitType.Point:
          {
            dateTextBoxWidth = Unit.Point (dateTextBoxWidthValue);            
            timeTextBoxWidth = Unit.Point (timeTextBoxWidthValue);            
            break;
          }
          default:
          {
            break;
          }
        }

        if (dateTextBoxWidthValue == 0)
          dateTextBoxWidth = Unit.Empty;

        if (timeTextBoxWidthValue == 0)
          timeTextBoxWidth = Unit.Empty;
     }

      if (ActualValueType == BocDateTimeValueType.DateTime)
      {
        _dateTextBox.Visible = true;
        _timeTextBox.Visible = true;
      }
      else if (ActualValueType == BocDateTimeValueType.Date)
      {
        _dateTextBox.Visible = true;
        _timeTextBox.Visible = false;

        if (dateTimeSpacer != null)
          dateTimeSpacer.Visible = false;
      }
      else if (IsDesignMode && ActualValueType == BocDateTimeValueType.Undefined)
      {
        _dateTextBox.Visible = true;
        _timeTextBox.Visible = true;
      }
      else 
      {
        _dateTextBox.Visible = false;
        _timeTextBox.Visible = false;
        
        if (dateTimeSpacer != null)
          dateTimeSpacer.Visible = false;
        
        if (imageButtonSpacer != null)
          imageButtonSpacer.Visible = false;
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

      _dateTextBox.Style["vertical-align"] = "middle";
      _timeTextBox.Style["vertical-align"] = "middle";
      _imageButton.Style["vertical-align"] = "middle";

      if (! dateTextBoxWidth.IsEmpty)
        _dateTextBox.Width = dateTextBoxWidth;
      if (! timeTextBoxWidth.IsEmpty)
        _timeTextBox.Width = timeTextBoxWidth;

      _dateTextBox.Height = this.Height;
      _timeTextBox.Height = this.Height;

      _dateTextBox.ApplyStyle (_commonStyle);
      _dateTimeTextBoxStyle.ApplyStyle (_dateTextBox);
      _dateTextBoxStyle.ApplyStyle (_dateTextBox);

      _timeTextBox.ApplyStyle (_commonStyle);
      _dateTimeTextBoxStyle.ApplyStyle (_timeTextBox);
      _timeTextBoxStyle.ApplyStyle (_timeTextBox);

      //  Common style not useful with image button
      //  _imageButton.ApplyStyle (_commonStyle);
      _imageButton.ApplyStyle (_imageButtonStyle);
    }
  }
  
  /// <summary> Formats the <see cref="DateTime"/> value according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read only mode. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value. </returns>
  protected virtual string FormatDateTimeValue (DateTime dateValue, bool isReadOnly)
  {
    isReadOnly = false;

    if (isReadOnly)
    {
      if (ShowSeconds)
      {
        //  F:  dddd, MMMM dd yyyy, hh, mm, ss
        return dateValue.ToString ("F");
      }
      else
      {
        //  f:  dddd, MMMM dd yyyy, hh, mm
        return dateValue.ToString ("f");
      }
    }
    else
    {
      if (ShowSeconds)
      {
        //  G:  yyyy, mm, dd, hh, mm, ss
        return dateValue.ToString ("G");
      }
      else
      {
        //  g:  yyyy, mm, dd, hh, mm
        return dateValue.ToString ("g");
      }
    }
  }

  /// <summary> Formats the <see cref="DateTime"/> value according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value. </returns>
  protected virtual string FormatDateTimeValue (DateTime dateValue)
  {
    return FormatDateTimeValue (dateValue, false);
  }

  /// <summary> 
  ///   Formats the <see cref="DateTime"/> value's date component according to the current culture.
  /// </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read only mode. </param>
  /// <returns> 
  ///   A formatted string representing the <see cref="DateTime"/> value's date component. 
  /// </returns>
  protected virtual string FormatDateValue (DateTime dateValue, bool isReadOnly)
  {
    isReadOnly = false;

    if (isReadOnly)
    {
      //  D:  dddd, MMMM dd yyyy
      return dateValue.ToString ("D");
    }
    else
    {
      //  d:  yyyy, mm, dd
      return dateValue.ToString ("d");
    }
  }

  /// <summary> 
  ///   Formats the <see cref="DateTime"/> value's date component according to the current culture.
  /// </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> 
  ///   A formatted string representing the <see cref="DateTime"/> value's date component. 
  /// </returns>
  protected virtual string FormatDateValue (DateTime dateValue)
  {
    return FormatDateValue (dateValue, false);
  }

  /// <summary> 
  ///   Formats the <see cref="DateTime"/> value's time component according to the current culture.
  /// </summary>
  /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read only mode. </param>
  /// <returns> 
  ///   A formatted string representing the <see cref="DateTime"/> value's time component. 
  /// </returns>
  protected virtual string FormatTimeValue (DateTime timeValue, bool isReadOnly)
  {
    //  ignore Read-Only

    if (ShowSeconds)
    {
      //  T: hh, mm, ss
      return timeValue.ToString ("T");
    }
    else
    {
      //  T: hh, mm
      return timeValue.ToString ("t");
    }
  }

  /// <summary> 
  ///   Formats the <see cref="DateTime"/> value's time component according to the current culture.
  /// </summary>
  /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> 
  ///   A formatted string representing the <see cref="DateTime"/> value's time component. 
  /// </returns>
  protected virtual string FormatTimeValue (DateTime timeValue)
  {
    return FormatTimeValue (timeValue, false);
  }

  /// <summary> Calculates the maximum length for required for entering the date component. </summary>
  /// <returns> The length. </returns>
  protected virtual int GetDateMaxLength()
  {
    string maxDate = new DateTime (2000, 12, 31).ToString ("d");
    
    return maxDate.Length;
  }

  /// <summary> Calculates the maximum length for required for entering the time component. </summary>
  /// <returns> The length. </returns>
  protected virtual int GetTimeMaxLength()
  {
    string maxTime = "";

    if (ShowSeconds)
      maxTime = new DateTime (1, 1, 1, 23, 30, 30).ToString ("T");
    else
      maxTime = new DateTime (1, 1, 1, 23, 30, 30).ToString ("t");

    return maxTime.Length;
  }

  /// <summary>
  ///   Raises this control's <see cref="DateTimeChanged"/> event if the value was changed 
  ///   through the text boxes.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void DateTimeTextBoxes_TextChanged (object sender, EventArgs e)
  {
    bool isDateChanged = _newInternalDateValue != _internalDateValue;
    bool isTimeChanged = _newInternalTimeValue != _internalTimeValue;

    if (isDateChanged)
    {
      InternalDateValue = _newInternalDateValue;
      
      //  Reset the time in if the control is displayed in date mode and the date was changed
      if (    ActualValueType == BocDateTimeValueType.Date
          &&  ! _savedDateTimeValue.IsNull)
      {
         _savedDateTimeValue = _savedDateTimeValue.Date;
      }
    }

    if (isTimeChanged)
    {
      InternalTimeValue = _newInternalTimeValue;
      
      //  Reset the seconds if the control does not display seconds and the time was changed
      if (    ! ShowSeconds
          &&  ! _savedDateTimeValue.IsNull)
      {
          TimeSpan seconds = new TimeSpan (0, 0, _savedDateTimeValue.Second);
         _savedDateTimeValue = _savedDateTimeValue.Subtract (seconds);
      }
    }

    if (isDateChanged || isTimeChanged)
      OnDateTimeChanged (EventArgs.Empty);
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    if (_valueType == BocDateTimeValueType.Undefined)
      _actualValueType = GetBocDateTimeValueType (Property);
  }

  /// <summary>
  ///   Evaluates the type of <paramref name="Property"/> and returns the appropriate 
  ///   <see cref="BocDateTimeValueType"/>.
  /// </summary>
  /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be evaluated. </param>
  /// <returns> The matching <see cref="BocDateTimeValueType"/></returns>
  private BocDateTimeValueType GetBocDateTimeValueType (IBusinessObjectProperty property)
  {
    if (property == null)
      return BocDateTimeValueType.Undefined;

    if (property is IBusinessObjectDateTimeProperty)
      return BocDateTimeValueType.DateTime;
    else if (property is IBusinessObjectDateProperty)
      return BocDateTimeValueType.Date;
    else
      throw new NotSupportedException ("BocDateTimeValue does not support property type " + property.GetType());
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <value> 
  ///   The value has the type specified in the <see cref="ActualValueType"/> property.
  /// </value>
  /// <exception cref="FormatException">
  ///   The value of the <see cref="InternalDateValue"/> and <see cref="InternalTimeValue"/> 
  ///   properties cannot be converted to the specified <see cref="ActualValueType"/>.
  /// </exception>
  [Browsable(false)]
  public new object Value
  {
    get 
    {
      if (InternalDateValue == null && InternalTimeValue == null)
        return null;

      DateTime dateTimeValue = DateTime.MinValue;

      //  Parse Date

      if (    ActualValueType == BocDateTimeValueType.DateTime
          ||  ActualValueType == BocDateTimeValueType.Date)
      {
        if (InternalDateValue == null)
        {
          throw new FormatException ("The date component of the DateTime value is null.");
        }

        try
        {
          dateTimeValue = DateTime.Parse (InternalDateValue).Date;
        }
        catch (FormatException ex)
        {
          throw new FormatException ("Error while parsing the date component (value: '" + InternalDateValue+ "') of the DateTime value. " + ex.Message);
        }
      }


      //  Parse Time

      if (ActualValueType == BocDateTimeValueType.DateTime
          &&  InternalTimeValue != null)
      {        
        try
        {
          dateTimeValue = dateTimeValue.Add (DateTime.Parse (InternalTimeValue).TimeOfDay);

          //  Restore the seconds if the control does not display them.
          if (    ! ShowSeconds
              &&  ! _savedDateTimeValue.IsNull)
          {
            dateTimeValue = dateTimeValue.AddSeconds (_savedDateTimeValue.Second);
          }
        }
        catch (FormatException ex)
        {
          throw new FormatException ("Error while parsing the time component (value: '" + InternalTimeValue+ "')of the DateTime value. " + ex.Message);
        }
      }
      else if (     ActualValueType == BocDateTimeValueType.Date
                &&  ! _savedDateTimeValue.IsNull)
      {
        //  Restore the time if the control is displayed in date mode.
        dateTimeValue = dateTimeValue.Add (_savedDateTimeValue.Time);
      }

      return dateTimeValue;
    }
    set 
    {
      if (value == null)
      {
        InternalDateValue = null;
        InternalTimeValue = null;
        _savedDateTimeValue = NaDateTime.Null;
        return;
      }

      DateTime dateTimeValue = (DateTime) value;
      _savedDateTimeValue = new NaDateTime (dateTimeValue);

      if (    ActualValueType == BocDateTimeValueType.DateTime
          ||  ActualValueType == BocDateTimeValueType.Date)
      {
        try
        {
          InternalDateValue = FormatDateValue (dateTimeValue);
        }
        catch  (InvalidCastException e)
        {
          throw new ArgumentException ("Expected type '" + _actualValueType.ToString() + "', but was '" + value.GetType().FullName + "'.", "value", e);
        }
      }
      else
      {
        InternalDateValue = null;
      }

      if (ActualValueType == BocDateTimeValueType.DateTime)
      {
        try
        {
          InternalTimeValue = FormatTimeValue (dateTimeValue);
        }
        catch  (InvalidCastException e)
        {
          throw new ArgumentException ("Expected type '" + _actualValueType.ToString() + "', but was '" + value.GetType().FullName + "'.", "value", e);
        }
      }
      else
      {
        InternalTimeValue = null;
      }
    }
  }

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> The string displayed in the date text box. </summary>
  protected virtual string InternalDateValue
  {
    get{ return _internalDateValue; }
    set
    {
      if (_internalDateValue == value)
        return;

      _internalDateValue = value;
    }
  }

  /// <summary> The string displayed in the time text box. </summary>
  protected virtual string InternalTimeValue
  {
    get{ return _internalTimeValue; }
    set
    {
      if (_internalTimeValue == value)
        return;

      _internalTimeValue = value;
    }
  }

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

  /// <summary>
  ///   The list of<see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
  ///   implementations that can be bound to this control.
  /// </summary>
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
  ///   The style that you want to apply to the TextBox (edit mode) and the Label 
  ///   (read-only mode).
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Use the <see cref="DateTimeTextBoxStyle"/>, <see cref="DateTextBoxStyle"/>, 
  ///     <see cref="TimeTextBoxStyle"/>, and <see cref="LabelStyle"/> to assign individual 
  ///     style settings for the respective modes. 
  ///   </para><para>
  ///     Note that if you set one of the <c>Font</c> 
  ///     attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden 
  ///     using <see cref="DateTimeTextBoxStyle"/>, <see cref="DateTextBoxStyle"/>, 
  ///     <see cref="TimeTextBoxStyle"/>, and <see cref="LabelStyle"/> properties.
  ///   </para><para>
  ///     Note that if you set one of the <c>Width</c> attribute, that it will be applied to
  ///     both the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/> as well as the 
  ///     <see cref="Label"/> as is. If the control is bound to an
  ///     <see cref="IBusinessObjectDateTimeProperty"/>, it will therefor show different 
  ///     widths depending on whether it is in read-only mode or not. It is recommended to set 
  ///     the width in the styles of the individual sub-controls instead.
  ///   </para>
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
  ///   The style that you want to apply to both the date and the time TextBoxes 
  ///   (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     These style settings override the styles defined in <see cref="CommonStyle"/>.
  ///   </para><para>
  ///     Note that if you set one of the <c>Font</c> 
  ///     attributes (Bold, Italic etc.) to <c>true</c>, this cannot be overridden using 
  ///     <see cref="DateTimeTextBoxStyle"/> and <see cref="DateTextBoxStyle"/> properties.
  ///   </para>
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
    get { return _imageButtonStyle; }
  }

  /// <summary> Gets the <see cref="TextBox"/> used in edit mode for the date component. </summary>
  [Browsable (false)]
  public TextBox DateTextBox
  {
    get { return _dateTextBox; }
  }

  /// <summary> Gets the <see cref="TextBox"/> used in edit mode for the time component. </summary>
  [Browsable (false)]
  public TextBox TimeTextBox
  {
    get { return _timeTextBox; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="ImageButton"/> used in edit mode for opening the date picker. </summary>
  [Browsable (false)]
  public ImageButton ImageButton
  {
    get { return _imageButton; }
  }

  /// <summary> Flag that determines whether to display the seconds. </summary>
  /// <value> <see langword="true"/> to enable the seconds. </value>
  [Category ("Appearance")]
  [Description ("Ture to display the seconds. ")]
  [DefaultValue (true)]
  public bool ShowSeconds
  {
    get { return _showSeconds; }
    set { _showSeconds = value; }
  }

  /// <summary> Flag that determines whether to apply an automatic maximum length to the text boxes. </summary>
  /// <value> <see langword="true"/> to enable the maximum length. </value>
  [Category ("Behavior")]
  [Description (" True to automatically limit the maxmimum length of the date and time input fields. ")]
  [DefaultValue (true)]
  public bool ProvideMaxLength
  {
    get { return _provideMaxLength; }
    set { _provideMaxLength = value; }
  }

  /// <summary> The <see cref="BocDateTimeValueType"/> assigned from an external source. </summary>
  [Description("Gets or sets a fixed value type.")]
  [Category("Data")]
  [DefaultValue(typeof(BocDateTimeValueType), "Undefined")]
  public BocDateTimeValueType ValueType
  {
    get 
    {
      return _valueType; 
    }
    set 
    {
      if (_valueType != value)
      {
        _valueType = value;
        _actualValueType = value;
        if (_valueType != BocDateTimeValueType.Undefined)
        {
          InternalDateValue = string.Empty;
          InternalTimeValue = string.Empty;
        }
      }
    }
  }

  /// <summary> The <see cref="BocDateTimeValueType"/> actually used by the cotnrol. </summary>
  [Browsable (false)]
  public BocDateTimeValueType ActualValueType
  {
    get 
    {
      Binding.EvaluateBinding();
      return _actualValueType; 
    }
  }

  /// <summary> The URL of the image used by the <see cref="ImageButton"/>. </summary>
  [Browsable (false)]
  protected virtual string ImageButtonImageUrl
  {
    get { return "Calendar.gif"; }
  }

  /// <summary>
  ///   The contents of the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/>, 
  ///   seperated by a newline character.
  /// </summary>
  [Browsable (false)]
  public string ValidationValue
  {
    get
    {
      if (ActualValueType == BocDateTimeValueType.DateTime)
        return InternalDateValue + "\n" + InternalTimeValue;
      else if (ActualValueType == BocDateTimeValueType.Date)
        return InternalDateValue + "\n" + "";
      else
        return "\n";

    }
  }
}

public enum BocDateTimeValueType
{
  Undefined,
  DateTime,
  Date
}

}
