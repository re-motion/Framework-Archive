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

/// <summary> This control can be used to display or edit a list of strings. </summary>
/// <include file='doc\include\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/Class/*' />
[ValidationProperty ("Text")]
[DefaultEvent ("TextChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocMultilineTextValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
	// constants

  /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultTextBoxWidth = "150pt";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocMultilineTextValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
    RequiredValidationMessage,
    /// <summary> The validation error message displayed when entered text exceeds the maximum length. </summary>
    MaxLengthValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectStringProperty) };

  private static readonly object s_textChangedEvent = new object();

	// member fields
  private bool _isDirty = true;

  private TextBox _textBox;
  private Label _label;

  private Style _commonStyle;
  private TextBoxStyle _textBoxStyle;
  private Style _labelStyle;

  /// <summary> The concatenated string built from the string array. Uses <c>\r\n</c> as delimiter. </summary>
  private string _internalValue = null;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>BocMultilineTextValue</b> type. </summary>
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

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  protected override void CreateChildControls()
  {
    _textBox.ID = ID + "_Boc_TextBox";
    _textBox.EnableViewState = false;
    Controls.Add (_textBox);

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
  /// <include file='doc\include\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadPostData/*' />
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

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    OnTextChanged();
  }

  /// <summary> Fires the <see cref="TextChanged"/> event. </summary>
  protected virtual void OnTextChanged ()
  {
    EventHandler eventHandler = (EventHandler) Events[s_textChangedEvent];
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

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
  }

  /// <summary> Overrides the <see cref="WebControl.RenderContents"/> method. </summary>
  protected override void RenderContents (HtmlTextWriter writer)
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

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _internalValue = (string) values[1];
    _isDirty = (bool)  values[2];

    _textBox.Text = Text;
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
  protected override object SaveViewState()
  {
    object[] values = new object[3];

    values[0] = base.SaveViewState();
    values[1] = _internalValue;
    values[2] = _isDirty;

    return values;
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadValue/*' />
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
  /// <include file='doc\include\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/SaveValue/*' />
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
  /// <include file='doc\include\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/CreateValidators/*' />
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

  /// <summary> Gets or sets the <see cref="IBusinessObjectStringProperty"/> object this control is bound to. </summary>
  /// <value> An <see cref="IBusinessObjectStringProperty"/> object. </value>
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
  
  /// <summary> Gets or sets the current value. </summary>
  /// <value> The <see cref="String"/> array currently displayed or <see langword="null"/> if no text is entered. </value>
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.ValueImplementation"/> property. </summary>
  /// <value> The value must be of type <b>string[]</b>. </value>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (string[]) value; }
  }

  /// <summary> Gets or sets the string representation of the current value. </summary>
  /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is <see cref="String.Empty"/>. </remarks>
  [Description("The string representation of the current value.")]
  [Category("Data")]
  [DefaultValue ("")]
  public string Text
  {
    get { return StringUtility.NullToEmpty (_internalValue); }
    set { _internalValue = value; }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <value> The <see cref="TextBox"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _textBox; }
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

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/> property. </summary>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return isList;
  }

  /// <summary> Overrides <see cref="Rubicon.Web.UI.ISmartControl.UseLabel"/>. </summary>
  /// <value> Always <see langword="true"/>. </value>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary> Occurs when the <see cref="Value"/> property changes between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler TextChanged
  {
    add { Events.AddHandler (s_textChangedEvent, value); }
    remove { Events.RemoveHandler (s_textChangedEvent, value); }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
  ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
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

  /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public TextBoxStyle TextBoxStyle
  {
    get { return _textBoxStyle; }
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

  /// <summary> Gets the <see cref="TextBox"/> used in edit mode. </summary>
  [Browsable (false)]
  public TextBox TextBox
  {
    get { return _textBox; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
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
  /// <summary> Gets the CSS-Class applied to the <see cref="BocMultilineTextValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocMultilineTextValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocMultilineTextValue"; } }
  #endregion
}
}
