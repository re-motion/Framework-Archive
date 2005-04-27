using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;
using System.Text;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit date/time values. </summary>
/// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocDateTimeValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
  //  constants

  /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";

  private const string c_defaultControlWidth = "150pt";
  private const int c_defaultDatePickerLengthInPoints = 150;

  private const string c_datePickerPopupForm = "DatePickerForm.aspx";
  private const string c_datePickerScriptUrl = "DatePicker.js";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocDateTimeValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The validation error message displayed when no input is provided. </summary>
    RequiredErrorMessage,
    /// <summary> The validation error message displayed when the input is incomplete. </summary>
    IncompleteErrorMessage,
    /// <summary> The validation error message displayed when both the date and the time component invalid. </summary>
    InvalidDateAndTimeErrorMessage,
    /// <summary> The validation error message displayed when the date component is invalid. </summary>
    InvalidDateErrorMessage,
    /// <summary> The validation error message displayed when the time component is invalid. </summary>
    InvalidTimeErrorMessage,
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectDateTimeProperty), typeof (IBusinessObjectDateProperty) };

  private static readonly object s_dateTimeChangedEvent = new object();

	// member fields

  private bool _isDirty = true;

  private TextBox _dateTextBox;
  private TextBox _timeTextBox;
  private Label _label;
  private HyperLink _datePickerButton;

  private Style _commonStyle;
  private SingleRowTextBoxStyle _dateTimeTextBoxStyle;
  private SingleRowTextBoxStyle _dateTextBoxStyle;
  private SingleRowTextBoxStyle _timeTextBoxStyle;
  private Style _labelStyle;
  private Style _datePickerButtonStyle;

  private string _internalDateValue = null;
  private string _internalTimeValue = null;
  /// <summary> A backup of the <see cref="DateTime"/> value. </summary>
  private NaDateTime _savedDateTimeValue = NaDateTime.Null;

  private BocDateTimeValueType _valueType = BocDateTimeValueType.Undefined;
  private BocDateTimeValueType _actualValueType = BocDateTimeValueType.Undefined;

  private Unit _datePickerPopupWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
  private Unit _datePickerPopupHeight = Unit.Point (c_defaultDatePickerLengthInPoints);

  private bool _showSeconds = false;
  private bool _provideMaxLength = true;
  private bool _enableClientScript = true;

  private string _errorMessage;
  private ArrayList _validators;

  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  // construction and disposing

  /// <summary> Initializes a new instance of the <see cref="BocDateTimeValue"/> class. </summary>
	public BocDateTimeValue()
	{
    _commonStyle = new Style();
    _dateTimeTextBoxStyle = new SingleRowTextBoxStyle();
    _dateTextBoxStyle = new SingleRowTextBoxStyle();
    _timeTextBoxStyle = new SingleRowTextBoxStyle();
    _labelStyle = new Style();
    _datePickerButtonStyle = new Style();
    _dateTextBox = new TextBox();
    _datePickerButton = new HyperLink();
    _timeTextBox = new TextBox();
    _label = new Label();
    _validators = new ArrayList();
	}

	// methods and properties

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  protected override void CreateChildControls()
  {
    _dateTextBox.ID = ID + "_Boc_DateTextBox";
    _dateTextBox.EnableViewState = false;
    Controls.Add (_dateTextBox);

    _datePickerButton.ID = ID + "_Boc_DatePickerButton";
    _datePickerButton.EnableViewState = false;
    Controls.Add (_datePickerButton);

    _timeTextBox.ID = ID + "_Boc_TimeTextBox";
    _timeTextBox.EnableViewState = false;
    Controls.Add (_timeTextBox);

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
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed between
  ///   post backs.
  /// </summary>
  /// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    //  Date input field

    string newDateValue = PageUtility.GetRequestCollectionItem (Page, _dateTextBox.UniqueID);
    bool isDateChanged =   newDateValue != null 
                        && StringUtility.NullToEmpty (_internalDateValue) != newDateValue;
    if (isDateChanged)
    {
      InternalDateValue = StringUtility.EmptyToNull (newDateValue);
      
      //  Reset the time in if the control is displayed in date mode and the date was changed
      if (   ActualValueType == BocDateTimeValueType.Date
          && ! _savedDateTimeValue.IsNull)
      {
         _savedDateTimeValue = _savedDateTimeValue.Date;
      }
      _isDirty = true;
    }

    //  Time input field

    string newTimeValue = PageUtility.GetRequestCollectionItem (Page, _timeTextBox.UniqueID);
    bool isTimeChanged =   newTimeValue != null 
                        && StringUtility.NullToEmpty (_internalTimeValue) != newTimeValue;
    if (isTimeChanged)
    {
      InternalTimeValue = StringUtility.EmptyToNull (newTimeValue);
      
      //  Reset the seconds if the control does not display seconds and the time was changed
      if (   ! ShowSeconds
          && ! _savedDateTimeValue.IsNull)
      {
          TimeSpan seconds = new TimeSpan (0, 0, _savedDateTimeValue.Second);
         _savedDateTimeValue = _savedDateTimeValue.Subtract (seconds);
      }
      _isDirty = true;
    }

    return isDateChanged || isTimeChanged;
  }

  /// <summary> Called when the state of the control has changed between post backs. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnDateTimeChanged (EventArgs.Empty);
  }

  /// <summary> Fires the <see cref="DateTimeChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnDateTimeChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_dateTimeChangedEvent];
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

    //  First call
    EnsureChildControlsPreRendered ();
    if (! IsDesignMode && ! IsReadOnly)
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
    //  Second call has practically no overhead
    //  Required to get optimum designer support.
    EnsureChildControlsPreRendered ();

    base.Render (writer);
  }

  /// <summary> Overrides the <see cref="Control.RenderChildren"/> method. </summary>
  protected override void RenderChildren (HtmlTextWriter writer)
  {
    if (IsReadOnly)
    {
      _label.RenderControl (writer);
    }
    else
    {
      bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
      bool isDateTextBoxHeightEmpty = StringUtility.IsNullOrEmpty (_dateTextBox.Style["height"]);
      bool isTimeTextBoxHeightEmpty = StringUtility.IsNullOrEmpty (_timeTextBox.Style["height"]);
      if (! isControlHeightEmpty && isDateTextBoxHeightEmpty && isTimeTextBoxHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isDateTextBoxWidthEmpty = StringUtility.IsNullOrEmpty (_dateTextBox.Style["width"]);
      bool isTimeTextBoxWidthEmpty = StringUtility.IsNullOrEmpty (_timeTextBox.Style["width"]);
      if (isDateTextBoxWidthEmpty && isTimeTextBoxWidthEmpty)
      {
        if (isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }

      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.AddStyleAttribute ("display", "inline");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin table

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);  // Begin tr

      bool hasDateField =    ActualValueType == BocDateTimeValueType.DateTime
                          || ActualValueType == BocDateTimeValueType.Date
                          || ActualValueType == BocDateTimeValueType.Undefined;
      bool hasTimeField =    ActualValueType == BocDateTimeValueType.DateTime
                          || ActualValueType == BocDateTimeValueType.Undefined;
      bool hasDatePicker =    hasDateField 
                           && (   _enableClientScript && IsDesignMode 
                               || _hasClientScript);

      string dateTextBoxSize = string.Empty;
      string timeTextBoxSize = string.Empty;
      if (hasDateField && hasTimeField && ShowSeconds)
      {
        dateTextBoxSize = "55%";
        timeTextBoxSize = "45%";
      }
      else if (hasDateField && hasTimeField)
      {
        dateTextBoxSize = "60%";
        timeTextBoxSize = "40%";
      }
      else if (hasDateField)
      {
        dateTextBoxSize = "90%";
      }

      if (hasDateField)
      {
        if (_dateTextBoxStyle.Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, dateTextBoxSize);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, _dateTextBoxStyle.Width.ToString());
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

        if (! isControlHeightEmpty && isDateTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
        if (! StringUtility.IsNullOrEmpty (dateTextBoxSize))
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        _dateTextBox.RenderControl (writer);  
      
        writer.RenderEndTag(); // End td
      }

      if (hasDatePicker)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.AddStyleAttribute ("padding-left", "3pt");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        _datePickerButton.RenderControl (writer);  
        writer.RenderEndTag(); // End td
      }

      if (hasTimeField)
      {
        if (_timeTextBoxStyle.Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, timeTextBoxSize);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, _timeTextBoxStyle.Width.ToString());
        if (hasDateField)
          writer.AddStyleAttribute ("padding-left", "3pt");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

        if (! isControlHeightEmpty && isTimeTextBoxHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
        if (! StringUtility.IsNullOrEmpty (timeTextBoxSize))
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        _timeTextBox.RenderControl (writer);  
      
        writer.RenderEndTag(); // End td
      }

      writer.RenderEndTag(); // End tr
      writer.RenderEndTag(); // End table
    }
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
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

    _dateTextBox.Text = _internalDateValue;
    _timeTextBox.Text = _internalTimeValue;
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
        _isDirty = false;
      }
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.SaveValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
      {
        DataSource.BusinessObject.SetProperty (Property, Value);

        //  get_Value parses the internal representation of the date/time value
        //  set_Value updates the internal representation of the date/time value
        Value = Value;
      }
    }
  }

  /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundModifiableWebControl.CreateValidators"/> method. </summary>
  /// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    BocDateTimeValueValidator dateTimeValueValidator = new BocDateTimeValueValidator();
    dateTimeValueValidator.ID = ID + "_ValidatorDateTime";
    dateTimeValueValidator.ControlToValidate = ID;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      IResourceManager resourceManager = GetResourceManager();
      dateTimeValueValidator.RequiredErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.RequiredErrorMessage);
      dateTimeValueValidator.IncompleteErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.IncompleteErrorMessage);
      dateTimeValueValidator.InvalidDateAndTimeErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      dateTimeValueValidator.InvalidDateErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.InvalidDateErrorMessage);
      dateTimeValueValidator.InvalidTimeErrorMessage = 
          resourceManager.GetString (ResourceIdentifier.InvalidTimeErrorMessage);
    }
    else
    {
      dateTimeValueValidator.ErrorMessage = _errorMessage;
    }
    validators[0] = dateTimeValueValidator;

    //  No validation that only enabled enum values get selected and saved.
    //  This behaviour mimics the Fabasoft enum behaviour

    _validators.AddRange (validators);
    return validators;
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.PreRenderChildControls"/> method. </summary>
  protected override void PreRenderChildControls()
  {
    bool isReadOnly = IsReadOnly;

    if (IsReadOnly)
    {
      PreRenderReadOnlyValue();
    }
    else
    {
      PreRenderEditModeValueDate();
      PreRenderEditModeValueTime();
      PreRenderEditModeValueDatePicker();
    }
  }
  
  /// <summary> Prerenders the <see cref="Label"/>. </summary>
  private void PreRenderReadOnlyValue()
  {
    if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
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
        DateTime dateTime = (DateTime) internalValue;

        if (ActualValueType == BocDateTimeValueType.DateTime)
          _label.Text = FormatDateTimeValue (dateTime, true);
        else if (ActualValueType == BocDateTimeValueType.Date)
          _label.Text = FormatDateValue (dateTime, true);
        else
          _label.Text = dateTime.ToString();
      }
      else
      {
        _label.Text = "&nbsp;";
      }
    }

    _label.Height = Unit.Empty;
    _label.Width = Unit.Empty;
    _label.ApplyStyle (_commonStyle);
    _label.ApplyStyle (_labelStyle);
  }

  /// <summary> Prerenders the <see cref="DateTextBox"/>. </summary>
  private void PreRenderEditModeValueDate()
  {
    if (ProvideMaxLength)
      _dateTextBox.MaxLength = GetDateMaxLength();
    _dateTextBox.Text = InternalDateValue;
    _dateTextBox.ReadOnly = ! Enabled;
    _dateTextBox.Width = Unit.Empty;
    _dateTextBox.Height = Unit.Empty;
    _dateTextBox.ApplyStyle (_commonStyle);
    _dateTimeTextBoxStyle.ApplyStyle (_dateTextBox);
    _dateTextBoxStyle.ApplyStyle (_dateTextBox);
  }

  /// <summary> Prerenders the <see cref="TimeTextBox"/>. </summary>
  private void PreRenderEditModeValueTime()
  {
    if (ProvideMaxLength)
      _timeTextBox.MaxLength = GetTimeMaxLength();
    _timeTextBox.Text = InternalTimeValue;
    _timeTextBox.ReadOnly = ! Enabled;
    _timeTextBox.Height = Unit.Empty;
    _timeTextBox.Width = Unit.Empty;
    _timeTextBox.ApplyStyle (_commonStyle);
    _dateTimeTextBoxStyle.ApplyStyle (_timeTextBox);
    _timeTextBoxStyle.ApplyStyle (_timeTextBox);
  }

  /// <summary> Prerenders the date picker. </summary>
  private void PreRenderEditModeValueDatePicker()
  {
    DetermineClientScriptLevel();
    if (_hasClientScript && Enabled)
    {
      string key = typeof (BocDateTimeValue).FullName;
      if (! HtmlHeadAppender.Current.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (DatePickerPage), ResourceType.Html, c_datePickerScriptUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
      }
    }

    //  TODO: BocDateTimeValue: When creating a DatePickerButton, move this block into the button
    //  and remove RenderContents.
    if (   _enableClientScript && IsDesignMode 
        || _hasClientScript)
    {
      string imageUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocDateTimeValue), ResourceType.Image, DatePickerImageUrl);
      if (imageUrl == null)
        _datePickerButton.ImageUrl = DatePickerImageUrl;  
      else
        _datePickerButton.ImageUrl = imageUrl; 

      string script;
      if (_hasClientScript && Enabled)
      {
        string pickerActionButton = "this";
        string pickerActionContainer = "document.getElementById ('" + ClientID + "')";
        string pickerActionTarget = "document.getElementById ('" + _dateTextBox.ClientID + "')";
        
        string pickerUrl = "'" + ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (DatePickerPage), ResourceType.UI, c_datePickerPopupForm) + "'";
        
        Unit popUpWidth = _datePickerPopupWidth;
        if (popUpWidth.IsEmpty)
          popUpWidth = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerWidth = "'" + popUpWidth.ToString() + "'";
        
        Unit popUpHeight = _datePickerPopupHeight;
        if (popUpHeight.IsEmpty)
          popUpHeight = Unit.Point (c_defaultDatePickerLengthInPoints);
        string pickerHeight = "'" + popUpHeight.ToString() + "'";

        script = "DatePicker_ShowDatePicker("
            + pickerActionButton + ", "
            + pickerActionContainer + ", "
            + pickerActionTarget + ", "
            + pickerUrl + ", "
            + pickerWidth + ", "
            + pickerHeight + ");"
            + "return false;";
       }
      else
      {
        script = "return false;";
      }
      _datePickerButton.NavigateUrl = "#";
      _datePickerButton.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = script;
    }

    _datePickerButton.Style["padding"] = "0px";
    _datePickerButton.Style["border"] = "none";
    _datePickerButton.Style["background-color"] = "transparent";
    _datePickerButton.ApplyStyle (_datePickerButtonStyle);
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

  /// <summary> Formats the <see cref="DateTime"/> value's date component according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read only mode. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value's date component. </returns>
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

  /// <summary> Formats the <see cref="DateTime"/> value's date component according to the current culture. </summary>
  /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value's date component. </returns>
  protected virtual string FormatDateValue (DateTime dateValue)
  {
    return FormatDateValue (dateValue, false);
  }

  /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
  /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <param name="isReadOnly"> <see langword="true"/> if the control is in read only mode. </param>
  /// <returns>  A formatted string representing the <see cref="DateTime"/> value's time component. </returns>
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

  /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
  /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
  /// <returns> A formatted string representing the <see cref="DateTime"/> value's time component.  </returns>
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

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
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
      if (_valueType == BocDateTimeValueType.Undefined)
        _actualValueType = GetBocDateTimeValueType (Property);
    }
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

  private void DetermineClientScriptLevel() 
  {
    _hasClientScript = false;

    if (! IsDesignMode)
    {
      if (EnableClientScript) 
      {
        bool isVersionGreaterOrEqual55 = 
               Context.Request.Browser.MajorVersion >= 6
            ||    Context.Request.Browser.MajorVersion == 5 
               && Context.Request.Browser.MinorVersion >= 0.5;
        bool isInternetExplorer55AndHigher = 
            Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

        _hasClientScript = isInternetExplorer55AndHigher;

        // // The next set of checks involve looking at the capabilities of the
        // // browser making the request.
        // //
        // // The DatePicker needs to verify whether the browser has EcmaScript (JavaScript)
        // // version 1.2+, and whether the browser supports DHTML, and optionally,
        // // DHTML behaviors.
        //
        // HttpBrowserCapabilities browserCaps = Page.Request.Browser;
        // bool hasEcmaScript = (browserCaps.EcmaScriptVersion.CompareTo(new Version(1, 2)) >= 0);
        // bool hasDOM = (browserCaps.MSDomVersion.Major >= 4);
        // bool hasBehaviors = (browserCaps.MajorVersion > 5) ||
        //                     ((browserCaps.MajorVersion == 5) && (browserCaps.MinorVersion >= .5));
        //
        // _hasClientScript = hasEcmaScript && hasDOM;
      }
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The value has the type specified in the <see cref="ActualValueType"/> property. If the parsing fails,
  ///   <see langword="null"/> is returned.
  /// </value>
  [Browsable(false)]
  public new object Value
  {
    get 
    {
      if (InternalDateValue == null && InternalTimeValue == null)
        return null;

      DateTime dateTimeValue = DateTime.MinValue;

      //  Parse Date

      if (   InternalDateValue == null
          && ActualValueType != BocDateTimeValueType.Undefined)
      {
        //throw new FormatException ("The date component of the DateTime value is null.");
        return null;
      }

      try
      {
        if (   ! IsDesignMode
            || ! StringUtility.IsNullOrEmpty (InternalDateValue))
        {
          dateTimeValue = DateTime.Parse (InternalDateValue).Date;
        }
      }
      catch (FormatException)
      {
        //throw new FormatException ("Error while parsing the date component (value: '" + InternalDateValue+ "') of the DateTime value. " + ex.Message);
        return null;
      }


      //  Parse Time

      if (   (   ActualValueType == BocDateTimeValueType.DateTime
              || ActualValueType == BocDateTimeValueType.Undefined)
          && InternalTimeValue != null)
      {        
        try
        {
          if (   ! IsDesignMode
              || ! StringUtility.IsNullOrEmpty (InternalTimeValue))
          {
            dateTimeValue = dateTimeValue.Add (DateTime.Parse (InternalTimeValue).TimeOfDay);
          }
        }
        catch (FormatException)
        {
          //throw new FormatException ("Error while parsing the time component (value: '" + InternalTimeValue+ "')of the DateTime value. " + ex.Message);
          return null;
        }

          //  Restore the seconds if the control does not display them.
          if (   ! ShowSeconds
              && ! _savedDateTimeValue.IsNull)
          {
            dateTimeValue = dateTimeValue.AddSeconds (_savedDateTimeValue.Second);
          }
      }
      else if (    ActualValueType == BocDateTimeValueType.Date
                && ! _savedDateTimeValue.IsNull)
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

      try
      {
        InternalDateValue = FormatDateValue (dateTimeValue);
      }
      catch  (InvalidCastException e)
      {
        throw new ArgumentException ("Expected type '" + _actualValueType.ToString() + "', but was '" + value.GetType().FullName + "'.", "value", e);
      }

      if (   ActualValueType == BocDateTimeValueType.DateTime
          || ActualValueType == BocDateTimeValueType.Undefined)
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.ValueImplementation"/> property. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> Gets or sets the string displayed in the <see cref="DateTextBox"/>. </summary>
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

  /// <summary> Gets or sets the string displayed in the <see cref="TimeTextBox"/>. </summary>
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <remarks> Returns the <see cref="DateTextBox"/> if the control is in edit-mode, otherwise the control itself. </remarks>
  public override Control TargetControl
  {
    get { return IsReadOnly ? (Control) this : _dateTextBox; }
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

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/> 
  ///   (edit mode) and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/CommonStyle/*' />
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
  ///   Gets the style that you want to apply to both the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/>
  ///   (edit mode) only.
  /// </summary>
  /// <include file='doc\include\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/DateTimeTextBoxStyle/*' />
  [Category("Style")]
  [Description("The style that you want to apply to both the date and the time TextBoxes (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle DateTimeTextBoxStyle
  {
    get { return _dateTimeTextBoxStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="DateTextBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to only the date TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle DateTextBoxStyle
  {
    get { return _dateTextBoxStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="TimeTextBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to only the time TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public SingleRowTextBoxStyle TimeTextBoxStyle
  {
    get { return _timeTextBoxStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
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

  /// <summary> Gets the style that you want to apply to the <see cref="DatePickerButton"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the Button (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style ButtonStyle
  {
    get { return _datePickerButtonStyle; }
  }

  /// <summary> Gets or sets the width of the IFrame used to display the date picker. </summary>
  /// <value> The <see cref="Unit"/> value used for the width. The default value is <b>150pt</b>. </value>
  [Category ("Appearance")]
  [Description("The width of the IFrame used to display the date picker.")]
  [DefaultValue (typeof (Unit), "150pt")]
  public Unit DatePickerPopupWidth
  {
    get { return _datePickerPopupWidth; }
    set { _datePickerPopupWidth = value; }
  }

  /// <summary> Gets or sets the height of the IFrame used to display the date picker. </summary>
  /// <value> The <see cref="Unit"/> value used for the height. The default value is <b>150pt</b>. </value>
  [Category ("Appearance")]
  [Description("The height of the IFrame used to display the date picker.")]
  [DefaultValue (typeof (Unit), "150pt")]
  public Unit DatePickerPopupHeight
  {
    get { return _datePickerPopupHeight; }
    set { _datePickerPopupHeight = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to display the seconds. </summary>
  /// <value> <see langword="true"/> to enable the seconds. The default value is <see langword="false"/>. </value>
  [Category ("Appearance")]
  [Description ("True to display the seconds. ")]
  [DefaultValue (false)]
  public bool ShowSeconds
  {
    get { return _showSeconds; }
    set { _showSeconds = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to apply an automatic maximum length to the text boxes. </summary>
  /// <value> <see langword="true"/> to enable the maximum length. The default value is <see langword="true"/>. </value>
  [Category ("Behavior")]
  [Description (" True to automatically limit the maxmimum length of the date and time input fields. ")]
  [DefaultValue (true)]
  public bool ProvideMaxLength
  {
    get { return _provideMaxLength; }
    set { _provideMaxLength = value; }
  }

  /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
  /// <value> <see langword="true"/> to enable the client script. The default value is <see langword="true"/>. </value>
  [Category ("Behavior")]
  [Description (" True to enable the client script for the pop-up calendar. ")]
  [DefaultValue (true)]
  public bool EnableClientScript
  {
    get { return _enableClientScript; }
    set { _enableClientScript = value; }
  }

  /// <summary> Gets or sets the <see cref="BocDateTimeValueType"/> assigned from an external source. </summary>
  /// <value> 
  ///   The externally set <see cref="BocDateTimeValueType"/>. The default value is 
  ///   <see cref="BocDateTimeValueType.Undefined"/>. 
  /// </value>
  [Description("Gets or sets a fixed value type.")]
  [Category ("Data")]
  [DefaultValue (BocDateTimeValueType.Undefined)]
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

  /// <summary> Gets the <see cref="BocDateTimeValueType"/> actually used by the cotnrol. </summary>
  [Browsable (false)]
  public BocDateTimeValueType ActualValueType
  {
    get 
    {
      RefreshPropertiesFromObjectModel();
      return _actualValueType; 
    }
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

  /// <summary> Gets the <see cref="HyperLink"/> used in edit mode for opening the date picker. </summary>
  [Browsable (false)]
  public HyperLink DatePickerButton
  {
    get { return _datePickerButton; }
  }

  /// <summary> The URL of the image used by the <see cref="DatePickerButton"/>. </summary>
  [Browsable (false)]
  protected virtual string DatePickerImageUrl
  {
    get { return "DatePicker.gif"; }
  }

  /// <summary>
  ///   Gets the contents of the <see cref="DateTextBox"/> and the <see cref="TimeTextBox"/>, 
  ///   seperated by a newline character.
  /// </summary>
  /// <remarks> This property is used for validation. </remarks>
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

  /// <summary> This event is fired when the date or time is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control changes.")]
  public event EventHandler DateTimeChanged
  {
    add { Events.AddHandler (s_dateTimeChangedEvent, value); }
    remove { Events.RemoveHandler (s_dateTimeChangedEvent, value); }
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
  /// <summary> Gets the CSS-Class applied to the <see cref="BocDateTimeValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocDateTimeValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocDateTimeValue"; } }
  #endregion
}

/// <summary>
///   A list possible data types for the <see cref="BocDateTimeValue"/>
/// </summary>
public enum BocDateTimeValueType
{
  /// <summary> No formatting applied. </summary>
  Undefined,
  /// <summary> The value is displayed as a date and time value. </summary>
  DateTime,
  /// <summary> Only the date component is displayed. </summary>
  Date
}

}
