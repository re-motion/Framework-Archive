using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Globalization;
using System.Text;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   This control can be used to display or edit a list of strings.
/// </summary>
/// <remarks>
///   The control is displayed using a <see cref="TextBox"/> in edit mode, 
///   and using a <see cref="Label"/> in read-only mode. Use the
///   <see cref="TextBox"/> and <see cref="Label"/> properties to access these controls directly.
/// </remarks>
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocMultilineTextValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
	// constants

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultTextBoxWidth = "150pt";

  // types
  /// <summary> A list of control wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocMultilineTextValue")]
  protected enum ResourceIdentifier
  {
    RequiredValidationMessage,
    MaxLengthValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectStringProperty) };

  private static readonly object s_textChangedEvent = new object();

	// member fields
  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="TextBox"/> used in edit mode. </summary>
  private TextBox _textBox;
  /// <summary> The <see cref="Label"/> used in read-only mode. </summary>
  private Label _label;

  /// <summary> The <see cref="Style"/> applied the <see cref="TextBox"/> and the <see cref="Label"/>. </summary>
  private Style _commonStyle;
  /// <summary> The <see cref="TextBoxStyle"/> applied to the <see cref="TextBox"/>. </summary>
  private TextBoxStyle _textBoxStyle;
  /// <summary> The <see cref="Style"/> applied to the <see cref="Label"/>. </summary>
  private Style _labelStyle;

  /// <summary>  The concatenated string build from the string array. </summary>
  /// <remarks> Uses <c>\r\n</c> as separation characters. </remarks>
  private string _internalValue = null;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

	public BocMultilineTextValue()
	{
    _commonStyle = new Style();
    _textBoxStyle = new TextBoxStyle (TextBoxMode.MultiLine);
    _labelStyle = new Style();
    _textBox = new TextBox();
    _label = new Label();
    _validators = new ArrayList();
	}

  // methods and properties

  protected override void CreateChildControls()
  {
    _textBox.ID = ID + "_Boc_TextBox";
    _textBox.EnableViewState = false;
    Controls.Add (_textBox);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
  }

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    _textBox.TextChanged += new EventHandler(TextBox_TextChanged);
  }
  
  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    return LoadPostData (postDataKey, postCollection);
  }

  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetRequestCollectionItem (Page, _textBox.UniqueID);
    bool isDataChanged = newValue != null && StringUtility.NullToEmpty (_internalValue) != newValue;
    if (isDataChanged)
    {
      _internalValue = newValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  protected virtual void RaisePostDataChangedEvent()
  {
    //  The data control's changed event is sufficient.
  }

  private void TextBox_TextChanged(object sender, EventArgs e)
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
    if (! IsDesignMode && ! IsReadOnly && Enabled)
      Page.RegisterRequiresPostBack (this);
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
    _internalValue = (string) values[1];
    _isDirty = (bool)  values[2];

    _textBox.Text = Text;
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
    values[1] = _internalValue;
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
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, Value);
    }
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
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
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    ArrayList validators = new ArrayList (2);

    RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
    requiredValidator.ID = ID + "_ValidatorRequired";
    requiredValidator.ControlToValidate = TargetControl.ID;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      requiredValidator.ErrorMessage = 
          GetResourceManager().GetString (ResourceIdentifier.RequiredValidationMessage);
    }
    else
    {
      requiredValidator.ErrorMessage = _errorMessage;
    }      
    validators.Add (requiredValidator);

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

    _validators.AddRange (validators);
    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }
  
  /// <summary> Prerenders the child controls. </summary>
  protected override void PreRenderChildControls()
  {
    if (IsReadOnly)
    {
      string[] lines = Value;
      string text = null;
      if (lines != null)
      {
        for (int i = 0; i < lines.Length; i++)
          lines[i] = HttpUtility.HtmlEncode (lines[i]);
        text = StringUtility.ConcatWithSeparator (lines, "<br />");
      }
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
      _textBox.Text = Text;

      _textBox.ReadOnly = ! Enabled;
      _textBox.Width = Unit.Empty;
      _textBox.Height = Unit.Empty;
      _textBox.ApplyStyle (_commonStyle);
      _textBoxStyle.ApplyStyle (_textBox);
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectStringProperty"/> object 
  ///   this control is bound to.
  /// </summary>
  /// <value>An <see cref="IBusinessObjectStringProperty"/> object.</value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectStringProperty Property
  {
    get { return (IBusinessObjectStringProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectStringProperty));
      base.Property = (IBusinessObjectStringProperty) value; 
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
  public new string[] Value
  {
    get 
    {
      string text = _internalValue;
      if (text != null)
        text = text.Trim();

      if (StringUtility.IsNullOrEmpty (text))
      {
        return null;
      }
      else
      {
        //  Allows for an optional \r
        string temp = _internalValue.Replace ("\r", "");
        return temp.Split ('\n');
      }
    }
    set
    {
      if (value == null)
        _internalValue = null;
      else
        _internalValue = StringUtility.ConcatWithSeparator (value, "\r\n");
    }
  }

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (string[]) value; }
  }

  /// <summary> Gets or sets the string representation of the current value. </summary>
  /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. </remarks>
  [Description("The string representation of the current value.")]
  [Category("Data")]
  [DefaultValue ("")]
  public string Text
  {
    get 
    { 
      return StringUtility.NullToEmpty (_internalValue);
    }
    set 
    {
      _internalValue = value;
    }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using its ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _textBox; }
  }

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

  /// <summary>
  ///   Indicates whether properties with the specified multiplicity are supported.
  /// </summary>
  /// <returns>
  ///   <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is 
  ///   supported.
  /// </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between posts to the server. </summary>
  [Category ("Action")]
  [Description ("Fires when the checked state of the control changes.")]
  public event EventHandler TextChanged
  {
    add { Events.AddHandler (s_textChangedEvent, value); }
    remove { Events.RemoveHandler (s_textChangedEvent, value); }
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <c>Font</c> 
  ///   attributes (Bold, Italic etc.) to <c>true</c>, this cannot be overridden using 
  ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> properties.
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
  ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public TextBoxStyle TextBoxStyle
  {
    get { return _textBoxStyle; }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  /// <summary> Gets the <see cref="TextBox"/> used in edit-mode. </summary>
  [Browsable (false)]
  public TextBox TextBox
  {
    get { return _textBox; }
  }

  /// <summary> Gets the <see cref="Label"/> used for in readonly mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets or sets the validation message if no input is provided but the control requries input. </summary>
  [Description("Validation message if no input is provided but the control requires input.")]
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
