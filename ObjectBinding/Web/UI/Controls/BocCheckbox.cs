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

/// <summary> This control can be used to display or edit a boolean value (true or false). </summary>
/// <include file='doc\include\Controls\BocCheckBox.xml' path='BocCheckBox/Class/*' />
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocCheckBox: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
	// constants

  private const string c_scriptFileUrl = "BocCheckBox.js";
  private const string c_defaultControlWidth = "100pt";

  private const string c_trueIcon = "CheckBoxTrue.gif";
  private const string c_falseIcon = "CheckBoxFalse.gif";

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
  private bool _value = false;
  private NaBooleanEnum _defaultValue = NaBooleanEnum.Undefined;
  private bool _isActive = true;

  private HtmlInputCheckBox _checkBox;
  private Image _image;
  private Label _label;
  private Style _labelStyle;

  private NaBooleanEnum _autoPostBack = NaBooleanEnum.Undefined;

  private NaBooleanEnum _showDescription = NaBooleanEnum.Undefined;
  private string _trueDescription = string.Empty;
  private string _falseDescription = string.Empty;
  private string _nullDescription = string.Empty;

  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>BocCheckBox</b> type. </summary>
	public BocCheckBox()
	{
    _labelStyle = new Style();
    _checkBox = new HtmlInputCheckBox();
    _image = new Image();
    _label = new Label();
	}

  // methods and properties

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  protected override void CreateChildControls()
  {
    _checkBox.ID = ID + "_Boc_CheckBox";
    _checkBox.EnableViewState = false;
    Controls.Add (_checkBox);

    _image.ID = ID + "_Boc_Image";
    _image.EnableViewState = false;
    Controls.Add (_image);

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

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="WaiException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity ()
  {
    if (IsWaiDebuggingEnabled && IsWaiLevelAConformanceRequired)
    {
      if (_showDescription == NaBooleanEnum.True)
        throw new WaiException (1, this, "ShowDescription");
    }
  }

  /// <summary> Overrides the <see cref="Control.OnPreRender"/> method. </summary>
  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    bool isReadOnly = IsReadOnly;
    if (isReadOnly)
      PreRenderReadOnlyMode();
    else
      PreRenderEditMode();

    _isActive = ! isReadOnly && Enabled;
  }

  /// <summary> Pre-renders the child controls for edit mode. </summary>
  private void PreRenderEditMode()
  {
    if (! IsDesignMode && Enabled)
      Page.RegisterRequiresPostBack (this);

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
    string description = _value ? trueDescription : falseDescription;

    DetermineClientScriptLevel();

    if (_hasClientScript && IsDescriptionEnabled)
    {
      if (! HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (
            this, Context, typeof (BocCheckBox), ResourceType.Html, c_scriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (
            s_scriptFileKey, 
            scriptUrl, 
            HtmlHeadAppender.Priority.Library);
      }

      if (Enabled)
      {
        if (! Page.IsStartupScriptRegistered (s_startUpScriptKey))
        {
          string script = string.Format (
              "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
              defaultTrueDescription, defaultFalseDescription);
          PageUtility.RegisterStartupScriptBlock (Page, s_startUpScriptKey, script);
        }
      }

      string checkBoxScript;
      string labelScript;
      if (Enabled)
      {
        string label = IsDescriptionEnabled ? "document.getElementById ('" + _label.ClientID + "')" : "null";
        string checkBox = "document.getElementById ('" + _checkBox.ClientID + "')";
        string script = " (" 
            + checkBox + ", "
            + label + ", " 
            + (StringUtility.IsNullOrEmpty (_trueDescription) ? "null" : "'" + _trueDescription + "'") + ", "
            + (StringUtility.IsNullOrEmpty (_falseDescription) ? "null" :"'" +  _falseDescription + "'") + ");";

        if (IsAutoPostBackEnabled)
          script += Page.GetPostBackEventReference (this) + ";";
        checkBoxScript = "BocCheckBox_OnClick" + script;
        labelScript = "BocCheckBox_ToggleCheckboxValue" + script;
      }
      else
      {
        checkBoxScript = "return false;";
        labelScript = "return false;";
      }
      _checkBox.Attributes.Add ("onclick", checkBoxScript);
      _label.Attributes.Add ("onclick", labelScript);
    }

    _checkBox.Checked = _value;
    _checkBox.Disabled = ! Enabled;
    
    if (IsDescriptionEnabled)
    {
      _label.Text = description;
      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_labelStyle);
    }
  }

  /// <summary> Pre-renders the child controls for read-only mode. </summary>
  private void PreRenderReadOnlyMode()
  {
    string imageUrl = ResourceUrlResolver.GetResourceUrl (
        this,
        Context, 
        typeof (BocCheckBox), 
        ResourceType.Image, 
        _value ? c_trueIcon : c_falseIcon);

    string description;
    if (_value)
    {
      if (StringUtility.IsNullOrEmpty (_trueDescription))
        description = GetResourceManager().GetString (ResourceIdentifier.TrueDescription);
      else
        description = _trueDescription;
    }
    else
    {
      if (StringUtility.IsNullOrEmpty (_falseDescription))
        description = GetResourceManager().GetString (ResourceIdentifier.FalseDescription);
      else
        description = _falseDescription;
    }
    _image.ImageUrl = imageUrl;
    _image.AlternateText = description;
    _image.Style["vertical-align"] = "middle";

    if (IsDescriptionEnabled)
    {
      _label.Text = description;
      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_labelStyle);
    }
  }

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;
    bool isDisabled = ! Enabled;

    string backUpCssClass = CssClass; // base.CssClass and base.ControlStyle.CssClass
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
    {
      if (isReadOnly)
        CssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        CssClass += " " + CssClassDisabled;
    }
    string backUpAttributeCssClass = Attributes["class"];
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      if (isReadOnly)
        Attributes["class"] += " " + CssClassReadOnly;
      else if (isDisabled)
        Attributes["class"] += " " + CssClassDisabled;
    }
    
    base.AddAttributesToRender (writer);

    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
      CssClass = backUpCssClass;
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
      Attributes["class"] = backUpAttributeCssClass;
    
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      string cssClass = CssClassBase;
      if (isReadOnly)
        cssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
    }

    writer.AddStyleAttribute ("white-space", "nowrap");
    if (! IsReadOnly)
    {
      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (_label.Style["width"]);
      if (isLabelWidthEmpty && isControlWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
    }
  }

  /// <summary> Overrides the <see cref="WebControl.RenderContents"/> method. </summary>
  protected override void RenderContents(HtmlTextWriter writer)
  {
    if (IsWaiLevelAConformanceRequired)
      EvaluateWaiConformity ();

    if (IsReadOnly)
    {
      _image.RenderControl (writer);
      _label.RenderControl (writer);
    }
    else
    {
      _checkBox.RenderControl (writer);
      _label.RenderControl (writer);
    }
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _value = (bool) values[1];
    _isActive = (bool) values[2];
    _isDirty = (bool)  values[3];

    _checkBox.Checked = _value;
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

  private void DetermineClientScriptLevel() 
  {
    _hasClientScript = false;

    if (! ControlHelper.IsDesignMode (this, Context))
    {
      _hasClientScript = true;
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

  /// <summary> Gets a flag that determines whether the control is to be treated as a required value. </summary>
  /// <value> Always <see langword="false"/> since the checkbox has no undefined state in the user interface. </value>
  [Browsable(false)]
  public override bool IsRequired 
  {
    get { return false; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The boolean value currently displayed. If <see langword="null"/> is assigned, <see cref="GetDefaultValue"/>
  ///   is evaluated to get the value. The <see cref="IsDirty"/> flag is set in this case.
  /// </value>
  [Browsable(false)]
  public new object Value
  {
    get
    {
      return _value;
    }
    set
    {
      if (value == null)
      {
        _value = GetDefaultValue();
        _isDirty = true;
      }
      else
      {
        _value = (bool) value;
      }
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
  ///   <see cref="NaBooleanEnum.True"/> or <see cref="NaBooleanEnum.False"/> to explicitly specify the default value 
  ///   or <see cref="NaBooleanEnum.Undefined"/> to leave the decision to the object model. If the control is unbound 
  ///   and no default value is specified, <see cref="NaBooleanEnum.False"/> is assumed as default value.
  /// </value>
  [Category("Behavior")]
  [Description("The boolean value to which this control defaults if the assigned value is null.")]
  [NotifyParentProperty(true)]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum DefaultValue
  {
    get { return _defaultValue; }
    set { _defaultValue = value; }
  }

  /// <summary>
  ///   Evaluates the default value settings using the <see cref="DefaultValue"/> and the <see cref="Property"/>'s
  ///   default value.
  /// </summary>
  /// <returns>
  ///   <list type="bullet">
  ///     <item> 
  ///       If <see cref="DefaultValue"/> is set to <see cref="NaBooleanEnum.True"/> or 
  ///       <see cref="NaBooleanEnum.False"/>, <see langword="true"/> or <see langword="false"/> is returned 
  ///       respectivly.
  ///     </item>
  ///     <item>
  ///       If <see cref="DefaultValue"/> is set to <see cref="NaBooleanEnum.Undefined"/>, the <see cref="Property"/>
  ///       is queried for its default value using the <see cref="IBusinessObjectBooleanProperty.GetDefaultValue"/>
  ///       method.
  ///       <list type="bullet">
  ///         <item> 
  ///           If <see cref="IBusinessObjectBooleanProperty.GetDefaultValue"/> returns 
  ///           <see cref="NaBooleanEnum.True"/> or <see cref="NaBooleanEnum.False"/>, <see langword="true"/> or 
  ///           <see langword="false"/> is returned respectivly.
  ///         </item>
  ///         <item>
  ///           Otherwise <see langword="false"/> is returned.
  ///         </item>
  ///       </list>
  ///     </item>
  ///   </list>
  /// </returns>
  protected bool GetDefaultValue()
  {
    if (_defaultValue == NaBooleanEnum.Undefined)
    {
      if (DataSource != null && DataSource.BusinessObjectClass != null && DataSource.BusinessObject != null && Property != null)
      {
        NaBoolean defaultValue = Property.GetDefaultValue (DataSource.BusinessObjectClass);
        if (defaultValue.IsNull)
          return false;
        return defaultValue.Value;
      }
      else
      {
        return false;
      }
    }
    else
    {
      return _defaultValue == NaBooleanEnum.True ? true : false;
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

  /// <summary> Gets the <see cref="System.Web.UI.HtmlControls.HtmlInputCheckBox"/> used for the value in edit mode. </summary>
  [Browsable (false)]
  public HtmlInputCheckBox CheckBox
  {
    get { return _checkBox; }
  }

  /// <summary> Gets the <see cref="System.Web.UI.WebControls.Image"/> used for the value in read-only mode. </summary>
  [Browsable (false)]
  public Image Image
  {
    get { return _image; }
  }

  /// <summary> Gets a flag that determines whether changing the checked state causes an automatic postback.</summary>
  /// <value> 
  ///   <see langword="NaBooleanEnum.True"/> to enable automatic postbacks. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as 
  ///   <see langword="NaBooleanEnum.False"/>.
  /// </value>
  [Description("Automatically postback to the server after the checked state is modified. Undefined is interpreted as false.")]
  [Category("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  [NotifyParentProperty (true)]
  public NaBooleanEnum AutoPostBack
  {
    get { return _autoPostBack; }
    set { _autoPostBack = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="AutoPostBack"/> property. </summary>
  /// <value> <see langowrd="true"/> if <see cref="AutoPostBack"/> is <see cref="NaBooleanEnum.True"/>. </value>
  protected bool IsAutoPostBackEnabled
  {
    get { return _autoPostBack == NaBooleanEnum.True;}
  }

  /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to enable the description. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.False"/>.
  /// </value>
  [Description("The flag that determines whether to show the description next to the checkbox. Undefined is interpreted as false.")]
  [Category ("Appearance")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum ShowDescription
  {
    get { return _showDescription; }
    set { _showDescription = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="ShowDescription"/> property. </summary>
  /// <value>
  ///   <see langowrd="true"/> if WAI conformity is not required 
  ///   and <see cref="ShowDescription"/> is <see cref="NaBooleanEnum.True"/>. 
  /// </value>
  protected bool IsDescriptionEnabled
  {
    get { return ! IsWaiLevelAConformanceRequired && _showDescription == NaBooleanEnum.True;}
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

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocCheckBox</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocCheckBox"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <b>.bocCheckBox.readOnly</b> as a selector.</para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocCheckBox"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}

}
