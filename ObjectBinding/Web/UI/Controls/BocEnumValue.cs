using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit enumeration values. </summary>
/// <include file='doc\include\Controls\BocEnumValue.xml' path='BocEnumValue/Class/*' />
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocEnumValue: BusinessObjectBoundModifiableWebControl, IPostBackDataHandler
{
	// constants

  private const string c_nullIdentifier = "--null--";

  /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultListControlWidth = "150pt";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocEnumValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> The text rendered for the null item in the list. </summary>
    NullDisplayName,
    /// <summary> The validation error message displayed when the null item is selected. </summary>
    NullItemValidationMessage
  }

  // static members
	
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectEnumerationProperty) };

  private static readonly object s_selectionChangedEvent = new object();

	// member fields

  private bool _isDirty = true;

  private ListControl _listControl;
  private Label _label;

  private object _value = null;
  private string _internalValue = null;
  private string _oldInternalValue = null;
  private IEnumerationValueInfo _enumerationValueInfo = null;
 
  private Style _commonStyle;
  private ListControlStyle _listControlStyle;
  private Style _labelStyle;

  /// <summary> State field for special behaviour during load view state. </summary>
  /// <remarks> Used by <see cref="RefreshEnumListSelectedValue"/>. </remarks>
  private bool _isExecutingLoadViewState;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

  /// <summary> Initializes a new instance of the <b>BocEnumValue</b> class. </summary>
	public BocEnumValue()
	{
    _commonStyle = new Style ();
    _listControlStyle = new ListControlStyle ();
    _labelStyle = new Style ();
    _label = new Label();
    _validators = new ArrayList();
	}

	// methods and properties

  /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
  /// <remarks>
  ///   If the <see cref="ListControl"/> could not be created from <see cref="ListControlStyle"/>,
  ///   the control is set to read-only.
  /// </remarks>
  protected override void CreateChildControls()
  {
    _listControl = _listControlStyle.Create (false);
    if (_listControl == null)
    {
      _listControl = new DropDownList();
      ReadOnly = NaBoolean.True;
    }
    _listControl.ID = ID + "_Boc_ListControl";
    _listControl.EnableViewState = true;
    Controls.Add (_listControl);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);

  }

  /// <summary> Overrides the <see cref="Control.OnInit"/> method. </summary>
  protected override void OnInit (EventArgs e)
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
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\Controls\BocEnumValue.xml' path='BocEnumValue/LoadPostData/*' />
  protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetRequestCollectionItem (Page, _listControl.UniqueID);
    bool isDataChanged = false;
    if (newValue != null)
    {
      if (_internalValue == null && newValue != c_nullIdentifier)
        isDataChanged = true;
      else if (_internalValue != null && newValue != _internalValue)
        isDataChanged = true;
    }

    if (isDataChanged)
    {
      if (newValue == c_nullIdentifier)
        InternalValue = null;
      else
        InternalValue = newValue;
      _isDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    RefreshEnumListSelectedValue();
    OnSelectionChanged();
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  protected virtual void OnSelectionChanged()
  {
    EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Overrides the <see cref="Control.OnPreRender"/> method. </summary>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    if (! IsDesignMode && ! IsReadOnly && Enabled)
      Page.RegisterRequiresPostBack (this);
  }

  /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    if (StringUtility.IsNullOrEmpty (CssClass))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
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
      bool isListControlHeightEmpty = StringUtility.IsNullOrEmpty (_listControl.Style["height"]);
      if (! isControlHeightEmpty && isListControlHeightEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
      bool isListControlWidthEmpty = StringUtility.IsNullOrEmpty (_listControl.Style["width"]);
      if (isListControlWidthEmpty)
      {
        if (isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultListControlWidth);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }

      _listControl.RenderControl (writer);
    }
  }

  /// <summary> Overrides the <see cref="Control.LoadViewState"/> method. </summary>
  protected override void LoadViewState (object savedState)
  {
    _isExecutingLoadViewState = true;

    object[] values = (object[]) savedState;

    base.LoadViewState (values[0]);
    _value = values[1];
    if (values[2] != null)
      _internalValue = (string) values[2];
    _isDirty = (bool)  values[3];

    _isExecutingLoadViewState = false;
  }

  /// <summary> Overrides the <see cref="Control.SaveViewState"/> method. </summary>
  protected override object SaveViewState()
  {
    object[] values = new object[4];

    values[0] = base.SaveViewState();
    values[1] = _value;
    values[2] = _internalValue;
    values[3] = _isDirty;

    return values;
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.LoadValue"/> method. </summary>
  /// <include file='doc\include\Controls\BocEnumValue.xml' path='BocEnumValue/LoadValue/*' />
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
  /// <include file='doc\include\Controls\BocEnumValue.xml' path='BocEnumValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];
    
    CompareValidator notNullItemValidator = new CompareValidator();
    notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    notNullItemValidator.ControlToValidate = TargetControl.ID;
    notNullItemValidator.ValueToCompare = c_nullIdentifier;
    notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      notNullItemValidator.ErrorMessage = 
          GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
    }
    else
    {
      notNullItemValidator.ErrorMessage = _errorMessage;
    }      
    validators[0] = notNullItemValidator;

    //  No validation that only enabled enum values get selected and saved.
    //  This behaviour mimics the Fabasoft enum behaviour

    _validators.AddRange (validators);
    return validators;
  }

  /// <summary>
  ///   Populates the <see cref="ListControl"/> from the <see cref="Property"/>'s 
  ///   list of enabled enum values before calling <see cref="RefreshEnumListSelectedValue"/>.
  /// </summary>
  protected virtual void RefreshEnumList()
  {
    if (! IsReadOnly)
    {
      if (Property != null)
      {
        _listControl.Items.Clear();

        if (! IsRequired)
          _listControl.Items.Add (CreateNullItem());

        IEnumerationValueInfo[] valueInfos = Property.GetEnabledValues();

        foreach (IEnumerationValueInfo valueInfo in valueInfos)
        {
          ListItem item = new ListItem (valueInfo.DisplayName, valueInfo.Identifier);
          _listControl.Items.Add (item);
        }
      }
      RefreshEnumListSelectedValue();
    }
  }

  /// <summary> Refreshes the <see cref="ListControl"/> with the new value. </summary>
  protected void RefreshEnumListSelectedValue()
  {
    if (! IsReadOnly)
    {
      bool hasPropertyAfterInitializion = ! _isExecutingLoadViewState && Property != null;

      string itemWithIdentifierToRemove = null;
      if (_oldInternalValue == null && IsRequired)
      {
        itemWithIdentifierToRemove = c_nullIdentifier;
      }
      else if (_oldInternalValue != null && Property != null)
      {
        IEnumerationValueInfo oldEnumerationValueInfo = Property.GetValueInfoByIdentifier (_oldInternalValue);        
        if (oldEnumerationValueInfo != null && ! oldEnumerationValueInfo.IsEnabled)
          itemWithIdentifierToRemove = _oldInternalValue;
        _oldInternalValue = null;
      }

      bool isNullItem =    InternalValue == null
                        || ! hasPropertyAfterInitializion;

      if (   (itemWithIdentifierToRemove == c_nullIdentifier && ! isNullItem)
          || (itemWithIdentifierToRemove != c_nullIdentifier && itemWithIdentifierToRemove != null))
      {
        ListItem itemToRemove = _listControl.Items.FindByValue (itemWithIdentifierToRemove);
        _listControl.Items.Remove (itemToRemove);
      }

      //  Check if null item is to be selected
      if (isNullItem)
      {
        //  No null item in the list
        if (_listControl.Items.FindByValue (c_nullIdentifier) == null)
          _listControl.Items.Insert (0, CreateNullItem());

        _listControl.SelectedValue = c_nullIdentifier;
      }
      else
      {
          //  Item currently not in the list
        if (_listControl.Items.FindByValue (InternalValue) == null)
        {
          ListItem item = new ListItem (EnumerationValueInfo.DisplayName, EnumerationValueInfo.Identifier);
          _listControl.Items.Add (item);
        }

        _listControl.SelectedValue = InternalValue;
      }
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.PreRenderChildControls"/> method. </summary>
  protected override void PreRenderChildControls()
  {
    RefreshEnumListSelectedValue();

    if (IsReadOnly)
    {
      string text = null;
      if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else if (! IsDesignMode && EnumerationValueInfo != null)
      {
        text = EnumerationValueInfo.DisplayName;
      }

      if (StringUtility.IsNullOrEmpty (text))
        _label.Text = "&nbsp;";
      else
        _label.Text = text;

      _label.Width = Unit.Empty;
      _label.Height = Unit.Empty;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);
    }
    else
    {
      _listControl.Enabled = Enabled;

      _listControl.Width = Unit.Empty;
      _listControl.Height = Unit.Empty;
      _listControl.ApplyStyle (_commonStyle);
      _listControlStyle.ApplyStyle (_listControl);
    }
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    RefreshEnumList();
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    string nullDisplayName = string.Empty;
    if (! (_listControl is DropDownList))
    {
      if (IsDesignMode)
        nullDisplayName = "undefined";
      else
        nullDisplayName = GetResourceManager().GetString (ResourceIdentifier.NullDisplayName);
    }

    ListItem emptyItem = new ListItem (nullDisplayName, c_nullIdentifier);
    return emptyItem;
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectEnumerationProperty"/> object this control is bound to. </summary>
  /// <value> An <see cref="IBusinessObjectEnumerationProperty"/> object. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectEnumerationProperty Property
  {
    get { return (IBusinessObjectEnumerationProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectEnumerationProperty));

      base.Property = (IBusinessObjectEnumerationProperty) value; 
    }
  }
  
  /// <summary> Gets or sets the current value. </summary>
  /// <include file='doc\include\Controls\BocEnumValue.xml' path='BocEnumValue/Value/*' />
  [Browsable(false)]
  public new object Value
  {
    get 
    {
      EnsureValue();
      return _value; 
    }
    set
    {
      _value = value;

      if (Property != null && _value != null)
        _enumerationValueInfo = Property.GetValueInfoByValue (_value);
      else
        _enumerationValueInfo = null;

      if (_enumerationValueInfo != null)
        InternalValue = _enumerationValueInfo.Identifier;
      else
        InternalValue = null;
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.ValueImplementation"/> property. </summary>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = value; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <remarks> Only used to simplify access to the <see cref="IEnumerationValueInfo"/>. </remarks>
  /// <value> 
  ///   The <see cref="EnumerationValueInfo"/> object
  ///   or <see langword="null"/> if no item / the null item is selected 
  ///   or the <see cref="Property"/> is <see langword="null"/>.
  /// </value>
  protected IEnumerationValueInfo EnumerationValueInfo
  {
    get 
    {
      if (_enumerationValueInfo == null && Property != null && _value != null)
        _enumerationValueInfo = Property.GetValueInfoByValue (_value);

      return _enumerationValueInfo; 
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <remarks> Used to identify the currently selected item. </remarks>
  /// <value> 
  ///   The <see cref="IEnumerationValueInfo.Identifier"/> object
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  protected virtual string InternalValue
  {
    get 
    {
      if (_internalValue == null && EnumerationValueInfo != null)
        _internalValue = EnumerationValueInfo.Identifier;

      return _internalValue; 
    }
    set 
    {
      if (_internalValue == value)
        return;

      _oldInternalValue = _internalValue;
      _internalValue = value;
      
      EnsureValue();
      RefreshEnumListSelectedValue();
    }
  }

  /// <summary> Ensures that the <see cref="Value"/> is set to the enum-value of the <see cref="InternalValue"/>. </summary>
  protected void EnsureValue()
  {
    if (   _enumerationValueInfo != null 
        && _enumerationValueInfo.Identifier == _internalValue)
    {
      //  Still chached in _enumerationValueInfo
      _value = _enumerationValueInfo.Value;
    }
    else if (_internalValue != null && Property != null)
    {
      //  Can get a new EnumerationValueInfo
      _enumerationValueInfo = Property.GetValueInfoByIdentifier (_internalValue);
      _value = _enumerationValueInfo.Value;
    }
    else if (_internalValue == null)
    {
      _value = null;
      _enumerationValueInfo = null;
    }
  }

  /// <summary> Overrides the <see cref="BusinessObjectBoundWebControl.TargetControl"/> property. </summary>
  /// <remarks> Returns the <see cref="ListControl"/> if the control is in edit mode, otherwise the control itself. </remarks>
  public override Control TargetControl 
  {
    get { return (_listControl == null) ? (Control) this : _listControl; }
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
    get 
    {
      bool isDropDownList = _listControlStyle.ControlType == ListControlType.DropDownList;
      bool isListBox = _listControlStyle.ControlType == ListControlType.ListBox;
    
      return ! (isDropDownList || isListBox);
    }
  }

  /// <summary> This event is fired when the selection is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="ListControl"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
  ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
  ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
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

  /// <summary> Gets the style that you want to apply to the <see cref="ListBox"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category("Style")]
  [Description("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public ListControlStyle ListControlStyle
  {
    get { return _listControlStyle; }
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

  /// <summary> Gets the <see cref="ListControl"/> used in edit mode. </summary>
  [Browsable (false)]
  public ListControl ListControl
  {
    get
    {
      EnsureChildControls();
      return _listControl; 
    }
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
    get { return _errorMessage; }
    set 
    {
      _errorMessage = value; 
      foreach (BaseValidator validator in _validators)
        validator.ErrorMessage = _errorMessage;
    }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocEnumValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocEnumValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocEnumValue"; } }
  #endregion
}

}
