using System;
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

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   This control can be used to display or edit a tri-state value (true, false, and undefined).
/// </summary>
/// <remarks>
///   The control is displayed using an <see cref="ImageButton"/> (edit mode only) 
///   or an <see cref="Image"/> (read-only mode only) to simulate a check box. It also offers 
///   a <see cref="Label"/> containing the string representation of the current value, rendered 
///   next to the image. Use the <see cref="ImageButton"/>, <see cref="Image"/>, and 
///   <see cref="Label"/> properties to access these controls directly.
/// </remarks>
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("ValidationValue")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocBooleanValue: BusinessObjectBoundModifiableWebControl
{
	// constants

  private const string c_trueDescription = "yes";
  private const string c_falseDescription = "no";
  private const string c_nullDescription = "undefined";
  private const string c_nullItemValidationMessage = "Please set the checkbox to yes or no.";

  private const string c_bocBooleanValueScriptUrl = "BocBooleanValue.js";

  private const string c_trueIcon = "CheckBoxTrue.gif";
  private const string c_falseIcon = "CheckBoxFalse.gif";
  private const string c_nullIcon = "CheckBoxNull.gif";

  // types

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectBooleanProperty) };

  private static readonly object s_eventCheckedChanged = new object();

	// member fields
  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="ImageButton"/> used in edit mode. </summary>
  private ImageButton _imageButton;

  /// <summary> The <see cref="Image"/> used in read-only mode. </summary>
  private Image _image;

  /// <summary> The <see cref="Label"/> used to provide textual representation of the check state. </summary>
  private Label _label;

  /// <summary> The <see cref="BocInputHidden"/> used to hold the check state. </summary>
  private BocInputHidden _hiddenField;

  /// <summary> The <see cref="CompareValidator"/> returned by <see cref="CreateValidators"/>. </summary>
  private CompareValidator _notNullItemValidator;

  /// <summary> The tristate value of the checkbox. </summary>
  private NaBoolean _value = NaBoolean.Null;

  /// <summary> The new tristate value of the checkbox. </summary>
  private NaBoolean _newValue = NaBoolean.Null;

  /// <summary> Flag that determines whether the client script will be rendered. </summary>
  private bool _hasClientScript = false;

  /// <summary> The <see cref="Style"/> applied the textboxes and the label. </summary>
  private Style _commonStyle = new Style();
  /// <summary> The <see cref="Style"/> applied to the <see cref="Label"/>. </summary>
  private Style _labelStyle = new Style();

  /// <summary> Flag that determines whether to show the description. </summary>
  private bool _showDescription = true;

  /// <summary> The description displayed when the checkbox is set to <c>True</c>. </summary>
  private string _trueDescription = string.Empty;
  /// <summary> The description displayed when the checkbox is set to <c>False</c>. </summary>
  private string _falseDescription = string.Empty;
  /// <summary> The description displayed when the checkbox is set to <c>null</c>. </summary>
  private string _nullDescription = string.Empty;

  // construction and disposing

	public BocBooleanValue()
	{
	}

  // methods and properties

  protected override void CreateChildControls()
  {
    _hiddenField = new BocInputHidden();
    _hiddenField.ID = ID + "_Boc_HiddenField";
    _hiddenField.EnableViewState = false;
    Controls.Add (_hiddenField);

    _imageButton = new ImageButton();
    _imageButton.ID = ID + "_Boc_ImageButton";
    _imageButton.EnableViewState = false;
    Controls.Add (_imageButton);

    _image = new Image();
    _image.ID = ID + "_Boc_Image";
    _image.EnableViewState = false;
    Controls.Add (_image);

    _label = new Label();
    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
 
    _notNullItemValidator = new CompareValidator();
  }

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
    _hiddenField.ValueChanged += new EventHandler(HiddenField_ValueChanged);
  }

  /// <summary>
  ///   Calls the parent's <c>OnLoad</c> method and prepares the binding information.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    if (! IsDesignMode)
    {
      string newValue = PageUtility.GetRequestCollectionItem (Page, _hiddenField.UniqueID);
      _newValue = NaBoolean.Parse (newValue);

      if (newValue != null && _newValue != _value)
        _isDirty = true;
    }
  }

  /// <summary> Fires the <see cref="CheckedChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnCheckedChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_eventCheckedChanged];
    if (eventHandler != null)
      eventHandler (this, e);
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
    EnsureChildControlsPreRendered();
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
    EnsureChildControlsPreRendered ();

    base.Render (writer);
  }

  /// <summary>
  ///   Calls the parents <c>LoadViewState</c> method and restores this control's specific data.
  /// </summary>
  /// <param name="savedState">
  ///   An <see cref="Object"/> that represents the control state to be restored.
  /// </param>
  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _value = (NaBoolean) values[1];
    _isDirty = (bool)  values[2];

    _hiddenField.Value = _value.ToString();
  }

  /// <summary>
  ///   Calls the parents <c>SaveViewState</c> method and saves this control's specific data.
  /// </summary>
  /// <returns>
  ///   Returns the server control's current view state.
  /// </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[3];

    values[0] = base.SaveViewState();
    values[1] = _value;
    values[2] = _isDirty;

    return values;
  }

  /// <summary>
  ///   Loads the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> or uses the cached
  ///   information if <paramref name="interim"/> is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to load the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      //Binding.EvaluateBinding();
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
  ///   is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to write the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      //Binding.EvaluateBinding();
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  /// <summary>
  ///   Generates the validators depending on the control's configuration.
  /// </summary>
  /// <remarks>
  ///   Generates a validator that checks that the selected item is not the null item if the 
  ///   control is in edit-mode and input is required.
  /// </remarks>
  /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
  public override BaseValidator[] CreateValidators()
  {
    if (! IsRequired)
      return new BaseValidator[]{};

    BaseValidator[] validators = new BaseValidator[1];

    _notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    _notNullItemValidator.ControlToValidate = ID;
    _notNullItemValidator.ValueToCompare = NaBoolean.NullString;
    _notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    _notNullItemValidator.ErrorMessage = c_nullItemValidationMessage;

    validators[0] = _notNullItemValidator;

    return validators;
  }
  
  /// <summary> Initializes the child controls. </summary>
  protected override void PreRenderChildControls()
  {
    bool isReadOnly = IsReadOnly;

    string trueIconUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocBooleanValue), ResourceType.Image, c_trueIcon);
    string falseIconUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocBooleanValue), ResourceType.Image, c_falseIcon);
    string nullIconUrl = ResourceUrlResolver.GetResourceUrl (
        this, Context, typeof (BocBooleanValue), ResourceType.Image, c_nullIcon);
    
    string trueDescription = 
        (StringUtility.IsNullOrEmpty (_trueDescription) ? c_trueDescription : _trueDescription);
    string falseDescription = 
        (StringUtility.IsNullOrEmpty (_falseDescription) ? c_falseDescription : _falseDescription);
    string nullDescription = 
        (StringUtility.IsNullOrEmpty (_nullDescription) ? c_nullDescription : _nullDescription);
    
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

      key = typeof (BocBooleanValue).FullName+ "_Startup";
      if (! Page.IsStartupScriptRegistered (key))
      {
        string trueValue = NaBoolean.True.ToString();
        string falseValue = NaBoolean.False.ToString();
        string nullValue = NaBoolean.Null.ToString();

        script = string.Format (
            "BocBooleanValue_InitializeGlobals ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');",
            trueValue, falseValue, nullValue, 
            trueDescription, falseDescription, nullDescription, 
            trueIconUrl, falseIconUrl, nullIconUrl);
        PageUtility.RegisterStartupScriptBlock (Page, key, script);
      }

      string requiredFlag = IsRequired ? "true" : "false";
      string imageButton = "document.getElementById ('" + _imageButton.ClientID + "')";
      string label = _showDescription ? "document.getElementById ('" + _label.ClientID + "')" : "null";
      string hiddenField = "document.getElementById ('" + _hiddenField.ClientID + "')";
      script = "BocBooleanValue_SelectNextCheckboxValue (" 
          + imageButton + ", " 
          + label + ", " 
          + hiddenField + ", "
          +  requiredFlag + ");"
          + "return false;";
      _label.Attributes.Add (HtmlTextWriterAttribute.Onclick.ToString(), script);
      _imageButton.Attributes.Add (HtmlTextWriterAttribute.Onclick.ToString(), script);
    }

    _hiddenField.Visible = ! isReadOnly;
    _imageButton.Visible = ! isReadOnly;
    _image.Visible = isReadOnly;

    if (isReadOnly)
    {
      _image.ImageUrl = imageUrl;
    }
    else
    {
      _hiddenField.Value = _value.ToString();
      _imageButton.ImageUrl = imageUrl;
    }

    if (_showDescription)
      _label.Text = description;

    _imageButton.AlternateText = description;
    _imageButton.Style["vertical-align"] = "text-bottom";

    _label.Height = Height;
    _label.ApplyStyle (_commonStyle);
    _label.ApplyStyle (_labelStyle);
  }

  
  private void HiddenField_ValueChanged(object sender, EventArgs e)
  {
    _value = _newValue;
    OnCheckedChanged (EventArgs.Empty);
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    //  nothing to do
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

  /// <summary>
  ///   The <see cref="IBusinessObjectBooleanProperty"/> object this control is bound to.
  /// </summary>
  /// <value>An <see cref="IBusinessObjectBooleanProperty"/> object.</value>
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
  
  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <value> 
  ///   The boolean value currently displayed 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
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

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using it's ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _imageButton; }
  }

  /// <summary>
  ///   Specifies whether the boolean value within the control has been changed 
  ///   since the last load/save operation.
  /// </summary>
  /// <remarks>
  ///   Initially, the value of <c>IsDirty</c> is <c>true</c>. The value is set to <c>false</c> 
  ///   during loading and saving values. Text changes by the user cause <c>IsDirty</c> to be 
  ///   reset to <c>false</c> during the loading phase of the request (i.e., before the page's 
  ///   <c>Load</c> event is raised).
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

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary> The string representation of the the tristate value of the checkbox. </summary>
  /// <remarks> Values can be "True", "False", and "null". </remarks>
  [Browsable (false)]
  public string ValidationValue
  {
    get { return _hiddenField.Value; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between posts to the server. </summary>
  [Category ("Action")]
  [Description ("Fires when the checked state of the control changes.")]
  public event EventHandler CheckedChanged
  {
    add { Events.AddHandler (s_eventCheckedChanged, value); }
    remove { Events.RemoveHandler (s_eventCheckedChanged, value); }
  }

  /// <summary>
  ///   The style that you want to apply to the <see cref="Label"/> 
  ///   used for displaying the description
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

  /// <summary> Gets the <see cref="Label"/> used for displaying the description. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary>
  ///   Gets or sets the flag that determines whether to show the description next to the checkbox.
  /// </summary>
  /// <value> <see langword="true"/> to enable the description. </value>
  [Description("The flag that determines whether to show the description next to the checkbox")]
  [Category ("Appearance")]
  [DefaultValue(true)]
  public bool ShowDescription
  {
    get { return _showDescription; }
    set { _showDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <c>True</c>. </summary>
  [Description("The description displayed when the checkbox is set to True.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string TrueDescription
  {
    get { return _trueDescription; }
    set { _trueDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <c>False</c>. </summary>
  [Description("The description displayed when the checkbox is set to False.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string FalseDescription
  {
    get { return _falseDescription; }
    set { _falseDescription = value; }
  }

  /// <summary> Gets or sets the description displayed when the checkbox is set to <c>null</c>. </summary>
  [Description("The description displayed when the checkbox is set to null.")]
  [Category ("Behavior")]
  [DefaultValue("")]
  public string NullDescription
  {
    get { return _nullDescription; }
    set { _nullDescription = value; }
  }

  /// <summary>
  ///   Gets or sets the validation message if the null item is selected 
  ///   but a valid selection is required.
  /// </summary>
  /// <remarks> 
  ///   Use this property to automatically assign a validation message by 
  ///   <see cref="Rubicon.Web.UI.Globalization.ResourceDispatcher"/>. 
  /// </remarks>
  [Description("Validation message if the null item is selected but a valid selection is required.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string NullItemErrorMessage
  {
    get { return _notNullItemValidator.ErrorMessage; }
    set { _notNullItemValidator.ErrorMessage = value; }
  }
}

/// <summary>
///   A special version of the <see cref="HtmlInputHidden"/> control that can raise a data changed
///   event when it's contents has been modified.
/// </summary>
/// <remarks>
///   .net 2.0 will provide such a control in it's class library.
/// </remarks>
internal class BocInputHidden: HtmlInputHidden, IPostBackDataHandler
{
  private static readonly object s_eventValueChanged = new object();

  protected virtual void RaisePostDataChangedEvent()
  {
    OnValueChanged (EventArgs.Empty);
  }

  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    this.RaisePostDataChangedEvent();
  }
 
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string oldValue = Value;
    string newValue = postCollection[postDataKey];
    if (oldValue != newValue)
    {
      Value = newValue;
      return true;
    }
    return false;
  }

  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    return this.LoadPostData (postDataKey, postCollection);
  }
 
  protected virtual void OnValueChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_eventValueChanged];
    if (eventHandler != null)
      eventHandler (this, e);
  }
 
  [Category ("Action")]
  [Description ("Fires when the value of the control changes.")]
  public event EventHandler ValueChanged
  {
    add { Events.AddHandler (s_eventValueChanged, value); }
    remove { Events.RemoveHandler (s_eventValueChanged, value); }
  }

}

}
