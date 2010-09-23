// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit date/time values. </summary>
  /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/Class/*' />
  [ValidationProperty ("ValidationValue")]
  [DefaultEvent ("TextChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocDateTimeValue : BusinessObjectBoundEditableWebControl, IBocDateTimeValue, IPostBackDataHandler, IFocusableControl
  {
    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocDateTimeValue")]
    protected internal enum ResourceIdentifier
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
      /// <summary> The alternate text displayed for the date picker button. </summary>
      DataPickerButtonAlternateText
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof (IBusinessObjectDateTimeProperty) };

    private static readonly object s_dateTimeChangedEvent = new object();

    // member fields

    private readonly DateTimeFormatter _formatter = new DateTimeFormatter();
    private readonly DatePickerButton _datePickerButton;

    private readonly Style _commonStyle;
    private readonly SingleRowTextBoxStyle _dateTimeTextBoxStyle;
    private readonly SingleRowTextBoxStyle _dateTextBoxStyle;
    private readonly SingleRowTextBoxStyle _timeTextBoxStyle;
    private readonly Style _labelStyle;

    private string _internalDateValue;
    private string _internalTimeValue;

    /// <summary> A backup of the <see cref="DateTime"/> value. </summary>
    private DateTime? _savedDateTimeValue;

    private BocDateTimeValueType _valueType = BocDateTimeValueType.Undefined;
    private BocDateTimeValueType _actualValueType = BocDateTimeValueType.Undefined;

    private bool _showSeconds;
    private bool _provideMaxLength = true;
    private bool _enableClientScript = true;

    private string _errorMessage;
    private readonly ArrayList _validators;
    private const string c_dateTextBoxIDPostfix = "Boc_DateTextBox";
    private const string c_timeTextBoxIDPostfix = "Boc_TimeTextBox";

    // construction and disposing

    public BocDateTimeValue ()
    {
      _commonStyle = new Style();
      _dateTimeTextBoxStyle = new SingleRowTextBoxStyle();
      _dateTextBoxStyle = new SingleRowTextBoxStyle();
      _timeTextBoxStyle = new SingleRowTextBoxStyle();
      _labelStyle = new Style();
      _datePickerButton = new DatePickerButton();
      _validators = new ArrayList();
    }

    // methods and properties

    protected override void CreateChildControls ()
    {
      _datePickerButton.ID = ID + "_Boc_DatePicker";
      _datePickerButton.EnableViewState = false;
      Controls.Add (_datePickerButton);
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      Binding.BindingChanged += Binding_BindingChanged;
      if (!IsDesignMode)
        Page.RegisterRequiresPostBack (this);
    }

    public override void RegisterHtmlHeadContents (HttpContextBase httpContext, HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (httpContext, htmlHeadAppender);

      var factory = ServiceLocator.GetInstance<IBocDateTimeValueRendererFactory>();
      var renderer = factory.CreateRenderer (httpContext, this, ServiceLocator);
      renderer.RegisterHtmlHeadContents (htmlHeadAppender, this);

      _datePickerButton.RegisterHtmlHeadContents (httpContext, htmlHeadAppender);
    }

    /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (RequiresLoadPostData)
        return LoadPostData (postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
    ///   between postbacks.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadPostData/*' />
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      //  Date input field

      string newDateValue = PageUtility.GetPostBackCollectionItem (Page, GetDateTextboxUniqueID());
      bool isDateChanged = newDateValue != null
                           && StringUtility.NullToEmpty (_internalDateValue) != newDateValue;
      if (isDateChanged)
      {
        InternalDateValue = StringUtility.EmptyToNull (newDateValue);

        //  Reset the time in if the control is displayed in date mode and the date was changed
        if ((ActualValueType == BocDateTimeValueType.Date) && _savedDateTimeValue.HasValue)
          _savedDateTimeValue = _savedDateTimeValue.Value.Date;
        IsDirty = true;
      }

      //  Time input field

      string newTimeValue = PageUtility.GetPostBackCollectionItem (Page, GetTimeTextboxUniqueID());
      bool isTimeChanged = newTimeValue != null
                           && StringUtility.NullToEmpty (_internalTimeValue) != newTimeValue;
      if (isTimeChanged)
      {
        InternalTimeValue = StringUtility.EmptyToNull (newTimeValue);

        //  Reset the seconds if the control does not display seconds and the time was changed
        if (! ShowSeconds
            && _savedDateTimeValue.HasValue)
        {
          TimeSpan seconds = new TimeSpan (0, 0, _savedDateTimeValue.Value.Second);
          _savedDateTimeValue = _savedDateTimeValue.Value.Subtract (seconds);
        }
        IsDirty = true;
      }

      return isDateChanged || isTimeChanged;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (! IsReadOnly && Enabled)
        OnDateTimeChanged();
    }

    /// <summary> Fires the <see cref="DateTimeChanged"/> event. </summary>
    protected virtual void OnDateTimeChanged ()
    {
      EventHandler eventHandler = (EventHandler) Events[s_dateTimeChangedEvent];
      if (eventHandler != null)
        eventHandler (this, EventArgs.Empty);
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (DateTextBoxStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "DateTextBoxStyle.AutoPostBack");

        if (TimeTextBoxStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "TimeTextBoxStyle.AutoPostBack");

        if (DateTimeTextBoxStyle.AutoPostBack == true)
          WcagHelper.Instance.HandleWarning (1, this, "DateTimeTextBoxStyle.AutoPostBack");
      }

      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelDoubleARequired())
      {
        if (ActualValueType == BocDateTimeValueType.DateTime)
          WcagHelper.Instance.HandleError (2, this, "ActualValueType");
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender (e);

      LoadResources (GetResourceManager());

      _datePickerButton.ContainerControlID = UniqueID;
      _datePickerButton.TargetControlID = GetDateTextboxUniqueID();

      EvaluateWaiConformity();
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      var factory = ServiceLocator.GetInstance<IBocDateTimeValueRendererFactory> ();
      var renderer = factory.CreateRenderer (Context, this, ServiceLocator);
      renderer.Render (writer);
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);

      if (values[1] != null)
        _internalDateValue = (string) values[1];
      if (values[2] != null)
        _internalTimeValue = (string) values[2];
      _valueType = (BocDateTimeValueType) values[3];
      _actualValueType = (BocDateTimeValueType) values[4];
      _showSeconds = (bool) values[5];
      _provideMaxLength = (bool) values[6];
      _savedDateTimeValue = (DateTime?) values[7];
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[8];

      values[0] = base.SaveControlState();
      values[1] = _internalDateValue;
      values[2] = _internalTimeValue;
      values[3] = _valueType;
      values[4] = _actualValueType;
      values[5] = _showSeconds;
      values[6] = _provideMaxLength;
      values[7] = _savedDateTimeValue;

      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (! interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null)
        {
          DateTime? value = (DateTime?) DataSource.BusinessObject.GetProperty (Property);
          LoadValueInternal (value, interim);
        }
      }
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (DateTime? value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (DateTime? value, bool interim)
    {
      if (! interim)
      {
        Value = value;
        IsDirty = false;
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/SaveValue/*' />
    public override void SaveValue (bool interim)
    {
      if (!interim && IsDirty)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        {
          DataSource.BusinessObject.SetProperty (Property, Value);
          //  get_Value parses the internal representation of the date/time value
          //  set_Value updates the internal representation of the date/time value
          Value = Value;
          IsDirty = false;
        }
      }
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      if (IsDesignMode)
        return;
      base.LoadResources (resourceManager);

      string key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
      if (! string.IsNullOrEmpty (key))
        ErrorMessage = resourceManager.GetString (key);
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/CreateValidators/*' />
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly)
        return new BaseValidator[0];

      BaseValidator[] validators = new BaseValidator[1];

      BocDateTimeValueValidator dateTimeValueValidator = new BocDateTimeValueValidator();
      dateTimeValueValidator.ID = ID + "_ValidatorDateTime";
      dateTimeValueValidator.ControlToValidate = ID;
      if (string.IsNullOrEmpty (_errorMessage))
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
        dateTimeValueValidator.ErrorMessage = _errorMessage;
      validators[0] = dateTimeValueValidator;

      _validators.AddRange (validators);
      return validators;
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
    private void RefreshPropertiesFromObjectModel ()
    {
      if (Property != null)
      {
        if (_valueType == BocDateTimeValueType.Undefined)
          _actualValueType = GetBocDateTimeValueType (Property);
      }
    }

    /// <summary>
    ///   Evaluates the type of <paramref name="property"/> and returns the appropriate <see cref="BocDateTimeValueType"/>.
    /// </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be evaluated. </param>
    /// <returns> The matching <see cref="BocDateTimeValueType"/></returns>
    private BocDateTimeValueType GetBocDateTimeValueType (IBusinessObjectProperty property)
    {
      if (property == null)
        return BocDateTimeValueType.Undefined;

      IBusinessObjectDateTimeProperty dateTimeProperty = property as IBusinessObjectDateTimeProperty;
      if (dateTimeProperty == null)
        throw new NotSupportedException ("BocDateTimeValue does not support property type " + property.GetType());

      if (dateTimeProperty.Type == DateTimeType.Date)
        return BocDateTimeValueType.Date;
      else
        return BocDateTimeValueType.DateTime;
    }


    /// <summary> Gets or sets the current value. </summary>
    /// <value> 
    ///   The value has the type specified in the <see cref="ActualValueType"/> property. If the parsing fails,
    ///   <see langword="null"/> is returned.
    /// </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    [Browsable (false)]
    public new DateTime? Value
    {
      get
      {
        if (InternalDateValue == null && InternalTimeValue == null)
          return null;

        DateTime dateTimeValue = DateTime.MinValue;

        //  Parse Date

        if ((InternalDateValue == null) && ActualValueType != BocDateTimeValueType.Undefined)
        {
          //throw new FormatException ("The date component of the DateTime value is null.");
          return null;
        }

        try
        {
          if (!IsDesignMode || !string.IsNullOrEmpty (InternalDateValue))
            dateTimeValue = DateTime.Parse (InternalDateValue).Date;
        }
        catch (FormatException)
        {
          //throw new FormatException ("Error while parsing the date component (value: '" + InternalDateValue+ "') of the DateTime value. " + ex.Message);
          return null;
        }
        catch (IndexOutOfRangeException)
        {
          return null;
        }


        //  Parse Time

        if ((ActualValueType == BocDateTimeValueType.DateTime || ActualValueType == BocDateTimeValueType.Undefined)
            && InternalTimeValue != null)
        {
          try
          {
            if (! IsDesignMode
                || ! string.IsNullOrEmpty (InternalTimeValue))
              dateTimeValue = dateTimeValue.Add (DateTime.Parse (InternalTimeValue).TimeOfDay);
          }
          catch (FormatException)
          {
            //throw new FormatException ("Error while parsing the time component (value: '" + InternalTimeValue+ "')of the DateTime value. " + ex.Message);
            return null;
          }
          catch (IndexOutOfRangeException)
          {
            return null;
          }

          //  Restore the seconds if the control does not display them.
          if (! ShowSeconds
              && _savedDateTimeValue.HasValue)
            dateTimeValue = dateTimeValue.AddSeconds (_savedDateTimeValue.Value.Second);
        }
        else if (ActualValueType == BocDateTimeValueType.Date
                 && _savedDateTimeValue.HasValue)
        {
          //  Restore the time if the control is displayed in date mode.
          dateTimeValue = dateTimeValue.Add (_savedDateTimeValue.Value.TimeOfDay);
        }

        return dateTimeValue;
      }
      set
      {
        IsDirty = true;
        _savedDateTimeValue = value;

        if (!_savedDateTimeValue.HasValue)
        {
          InternalDateValue = null;
          InternalTimeValue = null;
          return;
        }

        try
        {
          InternalDateValue = _formatter.FormatDateValue (_savedDateTimeValue.Value);
        }
        catch (InvalidCastException e)
        {
          throw new ArgumentException ("Expected type '" + _actualValueType + "', but was '" + value.GetType().FullName + "'.", "value", e);
        }

        if (ActualValueType == BocDateTimeValueType.DateTime
            || ActualValueType == BocDateTimeValueType.Undefined)
        {
          try
          {
            InternalTimeValue = _formatter.FormatTimeValue (_savedDateTimeValue.Value, ShowSeconds);
          }
          catch (InvalidCastException e)
          {
            throw new ArgumentException (
                "Expected type '" + _actualValueType + "', but was '" + value.GetType().FullName + "'.", "value", e);
          }
        }
        else
          InternalTimeValue = null;
      }
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return IsDesignMode; }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = ArgumentUtility.CheckType<DateTime?> ("value", value); }
    }

    /// <summary> Gets or sets the displayed date string. </summary>
    protected virtual string InternalDateValue
    {
      get { return _internalDateValue; }
      set { _internalDateValue = value; }
    }

    /// <summary> Gets or sets the displayed time string. </summary>
    protected virtual string InternalTimeValue
    {
      get { return _internalTimeValue; }
      set { _internalTimeValue = value; }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   date and the time text boxes if the control is in edit mode, or an empty array 
    ///   if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      if (IsReadOnly)
        return new string[0];

      switch (ActualValueType)
      {
        case BocDateTimeValueType.DateTime:
        case BocDateTimeValueType.Undefined:
          return new[] { GetDateTextboxClientID(), GetTimeTextboxClientID() };
        case BocDateTimeValueType.Date:
          return new[] { GetDateTextboxClientID() };
        default:
          return new string[0];
      }
    }

    public string GetDateTextboxClientID ()
    {
      return ClientID + ClientIDSeparator + c_dateTextBoxIDPostfix;
    }

    public string GetTimeTextboxClientID ()
    {
      return ClientID + ClientIDSeparator + c_timeTextBoxIDPostfix;
    }

    /// <summary> The <see cref="BocDateTimeValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    /// <summary>
    ///   The <see cref="BocDateTimeValue"/> supports properties of types <see cref="IBusinessObjectDateTimeProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Returns always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <remarks> Returns the control itself. </remarks>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the date text box if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string FocusID
    {
      get { return IsReadOnly ? null : GetDateTextboxClientID(); }
    }

    /// <summary>
    ///   Gets the style that you want to apply to the date and time text boxes 
    ///   (edit mode) as well as the label (read-only mode).
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/CommonStyle/*' />
    [Category ("Style")]
    [Description ("The style that you want to apply to the date and the time TextBoxes (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary>
    ///   Gets the style that you want to apply to both the date and the time text box
    ///   (edit mode) only.
    /// </summary>
    /// <include file='doc\include\UI\Controls\BocDateTimeValue.xml' path='BocDateTimeValue/DateTimeTextBoxStyle/*' />
    [Category ("Style")]
    [Description ("The style that you want to apply to both the date and the time TextBoxes (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public SingleRowTextBoxStyle DateTimeTextBoxStyle
    {
      get { return _dateTimeTextBoxStyle; }
    }

    /// <summary> Gets the style that you want to apply to the date text box (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to only the date TextBox (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public SingleRowTextBoxStyle DateTextBoxStyle
    {
      get { return _dateTextBoxStyle; }
    }

    /// <summary> Gets the style that you want to apply to the time text box (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="DateTimeTextBoxStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to only the time TextBox (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public SingleRowTextBoxStyle TimeTextBoxStyle
    {
      get { return _timeTextBoxStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the Label (read-only mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style LabelStyle
    {
      get { return _labelStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="DatePickerButton"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category ("Style")]
    [Description ("The style that you want to apply to the Button (edit mode) only.")]
    [NotifyParentProperty (true)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public Style ButtonStyle
    {
      get { return _datePickerButton.DatePickerButtonStyle; }
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
      set
      {
        _enableClientScript = value;
        _datePickerButton.EnableClientScript = value;
      }
    }

    /// <summary> Gets or sets the <see cref="BocDateTimeValueType"/> assigned from an external source. </summary>
    /// <value> 
    ///   The externally set <see cref="BocDateTimeValueType"/>. The default value is 
    ///   <see cref="BocDateTimeValueType.Undefined"/>. 
    /// </value>
    [Description ("Gets or sets a fixed value type.")]
    [Category ("Data")]
    [DefaultValue (BocDateTimeValueType.Undefined)]
    public BocDateTimeValueType ValueType
    {
      get { return _valueType; }
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

    public override bool Enabled
    {
      get { return base.Enabled; }
      set
      {
        base.Enabled = value;
        _datePickerButton.Enabled = value;
      }
    }

    private string GetDateTextboxUniqueID ()
    {
      return UniqueID + IdSeparator + c_dateTextBoxIDPostfix;
    }

    private string GetTimeTextboxUniqueID ()
    {
      return UniqueID + IdSeparator + c_timeTextBoxIDPostfix;
    }

    string IBocDateTimeValue.DateTextboxClientID
    {
      get { return GetDateTextboxClientID(); }
    }

    string IBocDateTimeValue.TimeTextboxClientID
    {
      get { return GetTimeTextboxClientID(); }
    }

    string IBocDateTimeValue.DateTextboxID
    {
      get { return GetDateTextboxUniqueID(); }
    }

    string IBocDateTimeValue.TimeTextboxID
    {
      get { return GetTimeTextboxUniqueID(); }
    }

    string IBocDateTimeValue.GetDatePickerText ()
    {
      return GetResourceManager().GetString (ResourceIdentifier.DataPickerButtonAlternateText);
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

    /// <summary> Gets the <see cref="IDatePickerButton"/> used in edit mode for opening the date picker. </summary>
    [Browsable (false)]
    public IDatePickerButton DatePickerButton
    {
      get { return _datePickerButton; }
    }

    /// <summary>
    ///   Gets the entered date and time, seperated by a newline character.
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
    [Description ("Fires when the value of the control has changed.")]
    public event EventHandler DateTimeChanged
    {
      add { Events.AddHandler (s_dateTimeChangedEvent, value); }
      remove { Events.RemoveHandler (s_dateTimeChangedEvent, value); }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    public string ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;
        for (int i = 0; i < _validators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) _validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }


    public string DateString
    {
      get { return InternalDateValue; }
    }

    public string TimeString
    {
      get { return InternalTimeValue; }
    }
  }

  /// <summary> A list possible data types for the <see cref="BocDateTimeValue"/> </summary>
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
