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
// readonly = deactivates checkbox
// defaultvalue by property and if property==null defaultvalue from iBOproperty
// label default = off
// value set -> test for null, apply default, set isdirty
/// <summary> This control can be used to display or edit a boolean value (true or false). </summary>
/// <include file='doc\include\Controls\BocCheckBox.xml' path='BocCheckBox/Class/*' />
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
[Obsolete("Work in Progress")]
public class BocCheckBox: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
	// constants

  private const string c_scriptFileUrl = "BocCheckBox.js";

  private const string c_defaultControlWidth = "100pt";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocCheckBox")]
  protected enum ResourceIdentifier
  {
    /// <summary> The descripton rendered next the check box when it is checked. </summary>
    TrueDescription,
    /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
    FalseDescription,
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectBooleanProperty) };

  private static readonly object s_checkedChangedEvent = new object();

  private static readonly string s_scriptFileKey = typeof (BocCheckBox).FullName + "_Script";
  private static readonly string s_startUpScriptKey = typeof (BocCheckBox).FullName+ "_Startup";

	// member fields
  private bool _isDirty = true;
  private NaBoolean _value = NaBoolean.Null;
  private NaBoolean _defaultValue = NaBoolean.Null;
  private bool _isActive = true;

  private HtmlInputCheckBox _checkBox;
  private Label _label;
  private Style _labelStyle;

  private NaBoolean _autoPostBack = NaBoolean.Null;

  private NaBooleanEnum _showDescription = NaBooleanEnum.Undefined;
  private string _trueDescription = string.Empty;
  private string _falseDescription = string.Empty;
  private string _nullDescription = string.Empty;

  private string _errorMessage;
  private ArrayList _validators;
  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>BocCheckBox</b> type. </summary>
	public BocCheckBox()
	{
    _labelStyle = new Style();
    _checkBox = new HtmlInputCheckBox();
    _label = new Label();
    _validators = new ArrayList();
	}

  // methods and properties

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  protected override void CreateChildControls()
  {
    _checkBox.ID = ID + "_Boc_CheckBox";
    _checkBox.EnableViewState = false;
    Controls.Add (_checkBox);

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
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\Controls\BocCheckBox.xml' path='BocCheckBox/LoadPostData/*' />
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    if (! _isActive)
      return false;

    string newValue = PageUtility.GetRequestCollectionItem (Page, _checkBox.UniqueID);
    bool newBooleanValue = ! StringUtility.IsNullOrEmpty (newValue);
    bool isDataChanged = _value != newBooleanValue;
    if (isDataChanged)
    {
      _value = newBooleanValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnCheckedChanged();
  }

  /// <summary> Fires the <see cref="CheckedChanged"/> event. </summary>
  protected virtual void OnCheckedChanged()
  {
    EventHandler eventHandler = (EventHandler) Events[s_checkedChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Overrides the <see cref="Control.OnPreRender"/> method. </summary>
  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);
    
    if (! IsDesignMode && ! IsReadOnly && Enabled)
      Page.RegisterRequiresPostBack (this);

    bool isReadOnly = IsReadOnly;

    string trueDescription = null;
    string falseDescription = null;
    string defaultTrueDescription = null;
    string defaultFalseDescription = null;
    if (IsDescriptionEnabled)
    {
      IResourceManager resourceManager = GetResourceManager();
      defaultTrueDescription = resourceManager.GetString (ResourceIdentifier.TrueDescription);
      defaultFalseDescription = resourceManager.GetString (ResourceIdentifier.FalseDescription);

      trueDescription = (StringUtility.IsNullOrEmpty (_trueDescription) ? defaultTrueDescription : _trueDescription);
      falseDescription = (StringUtility.IsNullOrEmpty (_falseDescription) ? defaultFalseDescription : _falseDescription);
    }
    string description;

    if (_value.IsTrue)
      description = trueDescription;
    else
      description = falseDescription;

    DetermineClientScriptLevel();

    if (_hasClientScript && ! isReadOnly && IsDescriptionEnabled)
    {
      string script;
      if (! HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (BocCheckBox), ResourceType.Html, c_scriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (
          s_scriptFileKey, 
          scriptUrl, 
          HtmlHeadAppender.Prioritiy.Library);
      }

      if (Enabled)
      {
        if (! Page.IsStartupScriptRegistered (s_startUpScriptKey))
        {
          script = string.Format (
              "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
              defaultTrueDescription, defaultFalseDescription);
          PageUtility.RegisterStartupScriptBlock (Page, s_startUpScriptKey, script);
        }
      }

      if (Enabled)
      {
        string label = IsDescriptionEnabled ? "document.getElementById ('" + _label.ClientID + "')" : "null";
        string checkBox = "document.getElementById ('" + _checkBox.ClientID + "')";
        script = "BocCheckBox_UpdateValue (" 
            + checkBox + ", "
            + label + ", " 
            + (StringUtility.IsNullOrEmpty (_trueDescription) ? "null" : "'" + _trueDescription + "'") + ", "
            + (StringUtility.IsNullOrEmpty (_falseDescription) ? "null" :"'" +  _falseDescription + "'") + ");";

        if (_autoPostBack.IsTrue)
          script += Page.GetPostBackEventReference (this) + ";";
        script += "return false;";
      }
      else
      {
        script = "return false;";
      }
      _checkBox.Attributes.Add (HtmlTextWriterAttribute.Onchange.ToString(), script);
      _checkBox.Attributes["title"] = description;
    }

    if (_value.IsNull)
      _value = GetDefaultValue();

    _checkBox.Checked = _value.IsTrue;
    _checkBox.Disabled = isReadOnly;
    
    if (IsDescriptionEnabled)
    {
      _label.Text = description;
      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_labelStyle);
      _label.Attributes["for"] = _checkBox.ClientID;
    }

    _isActive = ! isReadOnly && Enabled;
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
    if (StringUtility.IsNullOrEmpty (CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _value = (NaBoolean) values[1];
    _isActive = (bool) values[2];
    _isDirty = (bool)  values[3];

    _checkBox.Checked = _value.IsTrue;
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
  protected override object SaveViewState()
  {
    object[] values = new object[4];

    values[0] = base.SaveViewState();
    values[1] = _value;
    values[2] = _isActive;
    values[3] = _isDirty;

    return values;
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocCheckBox.xml' path='BocCheckBox/LoadValue/*' />
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
  /// <include file='doc\include\Controls\BocCheckBox.xml' path='BocCheckBox/SaveValue/*' />
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
  /// <include file='doc\include\Controls\BocCheckBox.xml' path='BocCheckBox/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    return new BaseValidator[0];
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

  /// <summary> Gets or sets the <see cref="IBusinessObjectBooleanProperty"/> object this control is bound to. </summary>
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
  
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new NaBooleanEnum Required
  {
    get { return base.Required; }
    set { base.Required = value; }
  }

  /// <summary> Gets a flag that determines whether the control is to be treated as a required value. </summary>
  /// <value> Always <see langword="false"/> since the checkbox has no undefined state in the user interface. </value>
  [Browsable(false)]
  public virtual bool IsRequired 
  {
    get { return false; }
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

  /// <summary> The boolean value to which this control defaults if the assigned value is <see langword="null"/>. </summary>
  /// <value> 
  ///   <see langword="true"/> or <see langword="false"/> to explicitly specify the default value or 
  ///   <see langword="null"/> to leave the decision to the object model. If the control is unbound and no default
  ///   value is specified, <see langword="false"/> is assumed as default value.
  /// </value>
  [Category("Behavior")]
  [Description("The boolean value to which this control defaults if the assigned value is null.")]
  [NotifyParentProperty(true)]
  public NaBoolean DefaultValue
  {
    get { return _defaultValue; }
    set { _defaultValue = value; }
  }

  protected bool GetDefaultValue()
  {
    if (_defaultValue.IsNull)
    {
      if (DataSource != null && DataSource.BusinessObject != null && Property != null)
      {
#warning Requires DefaultVlaue in IBooleanProperty
        return false;
      }
      else
      {
        return false;
      }
    }
    else
    {
      return _defaultValue.Value;
    }

  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <value> The <see cref="HyperLink"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _checkBox; }
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
  /// <value> Always <see langword="true"/>. </value>
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
  public bool ValidationValue
  {
    get { return _checkBox.Checked; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
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

  /// <summary> Gets the <see cref="System.Web.UI.HtmlControls.HtmlInputCheckBox"/> used for the value. </summary>
  [Browsable (false)]
  public HtmlInputCheckBox CheckBox
  {
    get { return _checkBox; }
  }

  /// <summary> Gets a flag that determines whether changing the checked state causes an automatic postback.</summary>
  /// <value> 
  ///   <see langword="NaBoolean.True"/> to enable automatic postbacks. 
  ///   Defaults to <see cref="NaBoolean.Null"/>, which is interpreted as <see langword="false"/>.
  /// </value>
  [Description("Automatically postback to the server after the checked state is modified. Undefined is interpreted as false.")]
  [Category("Behavior")]
  [DefaultValue (typeof(NaBoolean), "null")]
  [NotifyParentProperty (true)]
  public NaBoolean AutoPostBack
  {
    get { return _autoPostBack; }
    set { _autoPostBack = value; }
  }

  /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox. </summary>
  /// <value> 
  ///   <see langword="true"/> to enable the description. 
  ///   Defaults to <see cref="NaBoolean.Null"/>, which is interpreted as <see langword="false"/>.
  /// </value>
  [Description("The flag that determines whether to show the description next to the checkbox. Undefined is interpreted as false.")]
  [Category ("Appearance")]
  [DefaultValue (typeof(NaBooleanEnum), "Undefined")]
  public NaBooleanEnum ShowDescription
  {
    get { return _showDescription; }
    set { _showDescription = value; }
  }

  protected bool IsDescriptionEnabled
  {
    get { return _showDescription == NaBooleanEnum.True;}
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
  /// <value> 
  ///   The text displayed for <see langword="true"/>. The default value is <see cref="String.Empty"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("The description displayed when the checkbox is set to True.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string TrueDescription
  {
    get { return _trueDescription; }
    set { _trueDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
  /// <value> 
  ///   The text displayed for <see langword="false"/>. The default value is <see cref="String.Empty"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("The description displayed when the checkbox is set to False.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string FalseDescription
  {
    get { return _falseDescription; }
    set { _falseDescription = value; }
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
    get
    { 
      return _errorMessage; 
    }
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

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocCheckBoxValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocCheckBoxValue"; } }
  #endregion
}

}
