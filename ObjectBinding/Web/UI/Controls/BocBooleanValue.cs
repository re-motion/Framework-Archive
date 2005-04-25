using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design;
using System.Globalization;
using System.Text;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit a tri-state value (true, false, and undefined). </summary>
/// <include file='doc\include\Controls\BocBooleanValue.xml' path='BocBooleanValue/Class/*' />
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocBooleanValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
	// constants

  private const string c_bocBooleanValueScriptUrl = "BocBooleanValue.js";

  private const string c_trueIcon = "CheckBoxTrue.gif";
  private const string c_falseIcon = "CheckBoxFalse.gif";
  private const string c_nullIcon = "CheckBoxNull.gif";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocBooleanValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The descripton rendered next the check box when it is checked. </summary>
    TrueDescription,
    /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
    FalseDescription,
    /// <summary> The descripton rendered next the check box when its state is undefined.  </summary>
    NullDescription,
    /// <summary> The validation error message displayed when the null item is selected. </summary>
    NullItemValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectBooleanProperty) };

  private static readonly object s_checkedChangedEvent = new object();
  private const string c_defaultControlWidth = "100pt";

	// member fields
  private bool _isDirty = true;
  private NaBoolean _value = NaBoolean.Null;

  private HyperLink _hyperLink;
  private Image _image;
  private Label _label;
  private HiddenField _hiddenField;
  private Style _labelStyle;

  private bool _showDescription = true;
  private string _trueDescription = string.Empty;
  private string _falseDescription = string.Empty;
  private string _nullDescription = string.Empty;

  private string _errorMessage;
  private ArrayList _validators;
  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>BocBooleanValue</b> type. </summary>
	public BocBooleanValue()
	{
    _labelStyle = new Style();
    _hiddenField = new HiddenField();
    _hyperLink = new HyperLink();
    _image = new Image();
    _label = new Label();
    _validators = new ArrayList();
	}

  // methods and properties

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  protected override void CreateChildControls()
  {
    _hiddenField.ID = ID + "_Boc_HiddenField";
    _hiddenField.EnableViewState = false;
    Controls.Add (_hiddenField);

    _hyperLink.ID = ID + "_Boc_HyperLink";
    _hyperLink.EnableViewState = false;
    Controls.Add (_hyperLink);

    _image.ID = ID + "_Boc_Image";
    _image.EnableViewState = false;
    _hyperLink.Controls.Add (_image);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
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
  ///   between post backs.
  /// </summary>
  /// <include file='doc\include\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadPostData/*' />
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetRequestCollectionItem (Page, _hiddenField.UniqueID);
    NaBoolean newNaValue = NaBoolean.Null;
    if (newValue != null)
      newNaValue = NaBoolean.Parse (newValue);
    bool isDataChanged = newValue != null && _value != newNaValue;
    if (isDataChanged)
    {
      _value = newNaValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between post backs. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnCheckedChanged (EventArgs.Empty);
  }

  /// <summary> Fires the <see cref="CheckedChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnCheckedChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_checkedChangedEvent];
    if (eventHandler != null)
      eventHandler (this, e);
  }

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    writer.AddStyleAttribute ("white-space", "nowrap");
    if (! IsReadOnly)
    {
      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (_label.Style["width"]);
      if (isLabelWidthEmpty && isControlWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
    }
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
    EnsureChildControlsPreRendered();
    if (! IsDesignMode && ! IsReadOnly && Enabled)
      Page.RegisterRequiresPostBack (this);
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

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _value = (NaBoolean) values[1];
    _isDirty = (bool)  values[2];

    _hiddenField.Value = _value.ToString();
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
  protected override object SaveViewState()
  {
    object[] values = new object[3];

    values[0] = base.SaveViewState();
    values[1] = _value;
    values[2] = _isDirty;

    return values;
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadValue/*' />
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
  /// <include file='doc\include\Controls\BocBooleanValue.xml' path='BocBooleanValue/SaveValue/*' />
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
  /// <include file='doc\include\Controls\BocBooleanValue.xml' path='BocBooleanValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];

    CompareValidator notNullItemValidator = new CompareValidator();
    notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    notNullItemValidator.ControlToValidate = ID;
    notNullItemValidator.ValueToCompare = NaBoolean.NullString;
    notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
      notNullItemValidator.ErrorMessage = GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
    else
      notNullItemValidator.ErrorMessage = _errorMessage;
    validators[0] = notNullItemValidator;

    _validators.AddRange (validators);
    return validators;
  }
  
  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.PreRenderChildControls"/> method. </summary>
  protected override void PreRenderChildControls()
  {
    bool isReadOnly = IsReadOnly;

    string trueIconUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocBooleanValue), ResourceType.Image, c_trueIcon);
    string falseIconUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocBooleanValue), ResourceType.Image, c_falseIcon);
    string nullIconUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocBooleanValue), ResourceType.Image, c_nullIcon);
    
    IResourceManager resourceManager = GetResourceManager();
    string defaultTrueDescription = resourceManager.GetString (ResourceIdentifier.TrueDescription);
    string defaultFalseDescription = resourceManager.GetString (ResourceIdentifier.FalseDescription);
    string defaultNullDescription = resourceManager.GetString (ResourceIdentifier.NullDescription);

    string trueDescription = 
        (StringUtility.IsNullOrEmpty (_trueDescription) ? defaultTrueDescription : _trueDescription);
    string falseDescription = 
        (StringUtility.IsNullOrEmpty (_falseDescription) ? defaultFalseDescription : _falseDescription);
    string nullDescription = 
        (StringUtility.IsNullOrEmpty (_nullDescription) ? defaultNullDescription : _nullDescription);
    
    string imageUrl;
    string description;

    if (_value == NaBoolean.True)
    {
      imageUrl = trueIconUrl;
      description = trueDescription;
    }
    else if (_value == NaBoolean.Null)
    {
      imageUrl = nullIconUrl;
      description = nullDescription;
    }
    else
    {
      imageUrl = falseIconUrl;
      description = falseDescription;
    }

    DetermineClientScriptLevel();

    if (_hasClientScript && ! isReadOnly)
    {
      string script;

      string key = typeof (BocBooleanValue).FullName + "_Script";
      if (! HtmlHeadAppender.Current.IsRegistered (key))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (BocBooleanValue), ResourceType.Html, c_bocBooleanValueScriptUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
      }

      if (Enabled)
      {
        key = typeof (BocBooleanValue).FullName+ "_Startup";
        if (! Page.IsStartupScriptRegistered (key))
        {
          string trueValue = NaBoolean.True.ToString();
          string falseValue = NaBoolean.False.ToString();
          string nullValue = NaBoolean.Null.ToString();

          script = string.Format (
              "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');",
              trueValue, falseValue, nullValue, 
              defaultTrueDescription, defaultFalseDescription, defaultNullDescription, 
              trueIconUrl, falseIconUrl, nullIconUrl);
          PageUtility.RegisterStartupScriptBlock (Page, key, script);
        }
      }

      if (Enabled)
      {
        string requiredFlag = IsRequired ? "true" : "false";
        string image = "document.getElementById ('" + _image.ClientID + "')";
        string label = _showDescription ? "document.getElementById ('" + _label.ClientID + "')" : "null";
        string hiddenField = "document.getElementById ('" + _hiddenField.ClientID + "')";
        script = "BocBooleanValue_SelectNextCheckboxValue (" 
            + image + ", " 
            + label + ", " 
            + hiddenField + ", "
            + requiredFlag + ", "
            + (StringUtility.IsNullOrEmpty (_trueDescription) ? "null" : "'" + _trueDescription + "'") + ", "
            + (StringUtility.IsNullOrEmpty (_falseDescription) ? "null" :"'" +  _falseDescription + "'") + ", "
            + (StringUtility.IsNullOrEmpty (_nullDescription) ? "null" : "'" + _nullDescription + "'") + ");"
            + "return false;";
      }
      else
      {
        script = "return false;";
      }
      _label.Attributes.Add (HtmlTextWriterAttribute.Onclick.ToString(), script);
      _hyperLink.Attributes.Add (HtmlTextWriterAttribute.Onclick.ToString(), script);
      _hyperLink.Attributes.Add ("onKeyDown", "BocBooleanValue_OnKeyDown (this);");
      _hyperLink.Style["padding"] = "0px";
      _hyperLink.Style["border"] = "none";
      _hyperLink.Style["background-color"] = "transparent";
      _hyperLink.NavigateUrl = "#";
    }

    if (!isReadOnly)
      _hiddenField.Value = _value.ToString();
    _hiddenField.Visible = ! isReadOnly;
    _image.ImageUrl = imageUrl;
    _image.AlternateText = description;
    _image.Style["vertical-align"] = "middle";

    if (_showDescription)
      _label.Text = description;

    _label.Width = Unit.Empty;
    _label.Height = Unit.Empty;
    _label.ApplyStyle (_labelStyle);
  }

  private void DetermineClientScriptLevel() 
  {
    _hasClientScript = false;

    if (! ControlHelper.IsDesignMode (this, Context))
    {
      _hasClientScript = true;
      //bool isVersionHigherThan55 = Context.Request.Browser.MajorVersion >= 6
      //                        ||   Context.Request.Browser.MajorVersion == 5 
      //                          && Context.Request.Browser.MinorVersion >= 0.5;
      //bool isInternetExplorer55AndHigher = 
      //    Context.Request.Browser.Browser == "IE" && isVersionHigherThan55;
      //
      //_hasClientScript = isInternetExplorer55AndHigher;
    }
  }

  /// <summary> The <see cref="IBusinessObjectBooleanProperty"/> object this control is bound to. </summary>
  /// <value> An instance of type <see cref="IBusinessObjectBooleanProperty"/>. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectBooleanProperty Property
  {
    get { return (IBusinessObjectBooleanProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectBooleanProperty));

      base.Property = (IBusinessObjectBooleanProperty) value; 
    }
  }
  
  /// <summary> Gets or sets the current value. </summary>
  /// <value> The boolean value currently displayed or <see langword="null"/> if no item / the null item is selected. </value>
  [Browsable(false)]
  public new object Value
  {
    get
    {
      if (_value.IsNull)
        return null;
      else if (_value.IsTrue)
        return true;
      else
        return false;
    }
    set
    {
      if (value == null)
        _value = NaBoolean.Null;
      else if ((bool) value)
        _value = NaBoolean.True;
      else
        _value = NaBoolean.False;
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.ValueImplementation"/> property. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <remarks> Returns the <see cref="HyperLink"/> if the control is in edit-mode, otherwise the control itself. </remarks>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _hyperLink; }
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
  /// <value> Returns always <see langword="true"/>. </value>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary> Gets the string representation of this control's <see cref="Value"/>. </summary>
  /// <remarks> 
  ///   <para>
  ///     Values can be <c>True</c>, <c>False</c>, and <c>null</c>. 
  ///   </para><para>
  ///     This property is used for validation.
  ///   </para>
  /// </remarks>
  [Browsable (false)]
  public string ValidationValue
  {
    get { return _hiddenField.Value; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between post backs. </summary>
  [Category ("Action")]
  [Description ("Fires when the checked state of the control changes.")]
  public event EventHandler CheckedChanged
  {
    add { Events.AddHandler (s_checkedChangedEvent, value); }
    remove { Events.RemoveHandler (s_checkedChangedEvent, value); }
  }

  /// <summary>
  ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
  ///   description. 
  /// </summary>
  [Category("Style")]
  [Description("The style that you want to apply to the label used for displaying the description.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.Label"/> used for displaying the description. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.HyperLink"/> used for provding click functionality. </summary>
  [Browsable (false)]
  public HyperLink HyperLink
  {
    get { return _hyperLink; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.Image"/> used for displaying the checkbox icon. </summary>
  [Browsable (false)]
  public Image Image
  {
    get { return _image; }
  }

  /// <summary> Gets the <see cref="HiddenField"/> used for posting the value back to the server.  </summary>
  [Browsable (false)]
  public HiddenField HiddenField
  {
    get { return _hiddenField; }
  }

  /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox. </summary>
  /// <value> <see langword="true"/> to enable the description. </value>
  [Description("The flag that determines whether to show the description next to the checkbox")]
  [Category ("Appearance")]
  [DefaultValue(true)]
  public bool ShowDescription
  {
    get { return _showDescription; }
    set { _showDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
  [Description("The description displayed when the checkbox is set to True.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string TrueDescription
  {
    get { return _trueDescription; }
    set { _trueDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
  [Description("The description displayed when the checkbox is set to False.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string FalseDescription
  {
    get { return _falseDescription; }
    set { _falseDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="null"/>. </summary>
  [Description("The description displayed when the checkbox is set to null.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string NullDescription
  {
    get { return _nullDescription; }
    set { _nullDescription = value; }
  }

  /// <summary> Gets or sets the validation error message. </summary>
  [Description("Validation error message displayed if the selcted value is invalid.")]
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

}
