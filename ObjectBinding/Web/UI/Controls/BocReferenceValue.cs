using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit reference values. </summary>
/// <include file='doc\include\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocReferenceValue: BusinessObjectBoundModifiableWebControl
{
	// constants
	
  private const string c_nullIdentifier = "--null--";
  private const string c_nullDisplayName = "Undefined";
  private const string c_nullItemValidationMessage = "Please select an item.";

  // types

  // static members

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };

	// member fields

  /// <summary>
  ///   This event is fired when the selection is changed in the UI.
  /// </summary>
  /// <remarks>
  ///   The event is fired only if the selection change is caused by the user.
  /// </remarks>
  public event EventHandler SelectionChanged;

  /// <summary>
  ///   <see langword="true"/> if <see cref="Value"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used in modifiable mode. </summary>
  private DropDownList _dropDownList = null;

  /// <summary> The <see cref="Label"/> used in read-only mode. </summary>
  private Label _label = null;

  /// <summary> The <see cref="Image"/> optionally displayed in front of the value. </summary>
  private Image _icon = null;

  /// <summary> The object returned by <see cref="Value"/>. </summary>
  /// <remarks> Does not require <see cref="ISerializable"/>. </remarks>
  private object _externalValue = null;

  /// <summary> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object.
  /// </summary>
  private string _internalValue = null;

  /// <summary> The <see cref="DropDownList.SelectedValue"/> of <see cref="_dropDownList"/>. </summary>
  private string _newInternalValue = null;

  /// <summary> The <see cref="Style"/> applied to this controls an all sub-controls. </summary>
  private Style _commonStyle = new Style();

    /// <summary> The <see cref="Style"/> applied to the <see cref="_dropDownList"/>. </summary>
  private Style _dropDownListStyle = new Style();

  /// <summary> The <see cref="Style"/> applied to the <see cref="_label"/>. </summary>
  private Style _labelStyle = new Style();

  /// <summary> The <see cref="Style"/> applied to the <see cref="_icon"/>. </summary>
  private Style _iconStyle = new Style();

  /// <summary> State field for special behaviour during load view state. </summary>
  /// <remarks> Used by <see cref="InternalLoadValue"/>. </remarks>
  private bool _isLoadViewState;

  /// <summary> <see langword="true"/> to show the value's icon. </summary>
  private bool _enableIcon = true;

  /// <summary> 
  ///   The <see cref="string"/> with the search expression for populating the 
  ///   <see cref="_dropDownList"/>.
  /// </summary>
  private string _select = String.Empty;

  // construction and disposing

  /// <summary> Simple constructor. </summary>
	public BocReferenceValue()
	{
    //  empty
  }

	// methods and properties

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);

    //  Prevent a collapsed control
    if (Width == Unit.Empty)
      Width = Unit.Pixel (150);

    _icon = new Image();
    _dropDownList = new DropDownList();
    _label = new Label();

    _icon.ID = this.ID + "_Icon";
    _icon.EnableViewState = false;
    _icon.Visible = EnableIcon;
    Controls.Add (_icon);

    _dropDownList.ID = this.ID + "_DropDownList";
    _dropDownList.EnableViewState = true;
    Controls.Add (_dropDownList);

    _label.ID = this.ID + "_Label";
    _label.EnableViewState = true;
    Controls.Add (_label);

    _dropDownList.SelectedIndexChanged += new EventHandler(DropDownList_SelectedIndexChanged);
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
      string newInternalValue = this.Page.Request.Form[_dropDownList.UniqueID];
      if (newInternalValue == c_nullIdentifier)
        _newInternalValue = null;
      else if (newInternalValue != null)
        _newInternalValue = newInternalValue;
      else
        _newInternalValue = null;

      if (! Page.IsPostBack)
        RefreshBusinessObjectsList();
    }

    if (_newInternalValue != null && _newInternalValue != _internalValue)
      _isDirty = true;
  }

  /// <summary>
  ///   Raises this control's <see cref="SelectionChanged"/> event if the value was changed 
  ///   through the <see cref="_dropDownList"/>.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void DropDownList_SelectedIndexChanged (object sender, EventArgs e)
  {
    if (_newInternalValue != null && _newInternalValue != _internalValue)
    {
      InternalValue = _newInternalValue;
      OnSelectionChanged (EventArgs.Empty);
    }
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnSelectionChanged (EventArgs e)
  {
    if (SelectionChanged != null)
      SelectionChanged (this, e);
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    InternalLoadValue();
  }

  /// <summary> Refreshes the sub-controls for the new value. </summary>
  private void InternalLoadValue()
  {
    InternalLoadValue (false);
  }

  /// <overloads>Overloaded.</overloads>
  /// <summary> Refreshes the sub-controls for the new value. </summary>
  /// <param name="removeUndefined">
  ///   <see langword="true"/> to remove the item specifying a null reference.
  ///   Used for required values once they are set to an item different from the null item.
  /// </param>
  private void InternalLoadValue (bool removeUndefined)
  {
    if (IsReadOnly)
    {
      if (Value != null)
        _label.Text = ((IBusinessObjectWithIdentity)Value).DisplayName;
      else
        _label.Text = String.Empty;
    }
    else // Not Read-Only
    {
      if (removeUndefined)
      {
        ListItem itemToRemove = _dropDownList.Items.FindByValue (c_nullIdentifier);
        if (itemToRemove != null)
          _dropDownList.Items.Remove (itemToRemove);
      }

      //  Check if null item is to be selected
      bool hasProperty = ! _isLoadViewState && Property == null;

      if (InternalValue == null || ! hasProperty)
      {
        //  No
        if (_dropDownList.Items.FindByValue (c_nullIdentifier) == null)
        {
          _dropDownList.Items.Insert (0, CreateEmptyItem());
        }

        _dropDownList.SelectedValue = c_nullIdentifier;
      }
      else
      {
        if (_dropDownList.Items.FindByValue (InternalValue) != null)
        {
          _dropDownList.SelectedValue = InternalValue;
        }
        else if (Value != null)
        {
          IBusinessObjectWithIdentity businessObject = (IBusinessObjectWithIdentity)Value;

          _dropDownList.Items.Add (
            new ListItem (businessObject.DisplayName, businessObject.UniqueIdentifier));

          _dropDownList.SelectedValue = InternalValue;
        }
      }
    }

    if (Property != null)
    {
      IBusinessObjectService service
        = Property.ReferenceClass.BusinessObjectProvider.GetService(
          typeof (IBusinessObjectWebUIService));

      IBusinessObjectWebUIService webUIService = service as IBusinessObjectWebUIService;

      IconPrototype iconPrototype = null;
      if (webUIService != null)
      {
        if (Value != null)
          iconPrototype = webUIService.GetIcon ((IBusinessObjectWithIdentity)Value);
        else
          iconPrototype = webUIService.GetIcon (
            (IBusinessObjectClassWithIdentity) Property.ReferenceClass);
      }
      if (iconPrototype != null)
      {
        _icon.ImageUrl = iconPrototype.Url;
        _icon.Width = iconPrototype.Width;
        _icon.Height = iconPrototype.Height;
      }
      else
      {
        _icon.ImageUrl = "/images/Help.gif";  // HACK: Should be String.Empty;
        _icon.Width = Unit.Pixel(24);         // HACK: Should be Unit.Empty;
        _icon.Height = Unit.Pixel(24);        // HACK: Should be Unit.Empty;
      }
    }

    //  Only show exisiting icons
    if (_icon.ImageUrl != String.Empty)
      _icon.Visible = EnableIcon;
    else
      _icon.Visible = false;
  }

  public override void LoadValue()
  {
    Binding.EvaluateBinding();
    if (Property != null && DataSource != null && DataSource.BusinessObject != null)
    {
      Value = DataSource.BusinessObject.GetProperty (Property);
      _isDirty = false;
    }
  }

  public override void SaveValue()
  {
    Binding.EvaluateBinding();
    if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
      DataSource.BusinessObject.SetProperty (Property, Value);
  }

  protected virtual void RefreshDropDownList(IBusinessObjectWithIdentity[] businessObjects)
  {
    if (    Property != null 
        &&  businessObjects != null)
    {
      _dropDownList.Items.Clear();
    
      //  Add Undefined item
      if (InternalValue == null || !IsRequired)
      {
        _dropDownList.Items.Add (CreateEmptyItem());
      }

      //  Populate _dropDownList
      foreach (IBusinessObjectWithIdentity businessObject in businessObjects)
      {
        ListItem item = new ListItem (businessObject.DisplayName, businessObject.UniqueIdentifier);
        _dropDownList.Items.Add (item);
      }
    }

    InternalLoadValue();
  }

  /// <summary>
  ///   Sets the list of possible items in the list of reference values.
  /// </summary>
  /// <param name="businessObjects">Must not be <see langword="null"/>.</param>
  public void SetBusinessObjectsList (IBusinessObjectWithIdentity[] businessObjects)
  {
    if (IsReadOnly)
      return;

    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

    RefreshDropDownList (businessObjects);
  }

  /// <summary>
  /// 
  /// </summary>
  public void RefreshBusinessObjectsList()
  {
    if (Property == null)
      return;

    if (IsReadOnly)
      return;

    //  Get all matching business objects
    IBusinessObjectWithIdentity[] businessObjects
      = Property.SearchAvailableObjects (DataSource.BusinessObject , _select);

    RefreshDropDownList (businessObjects);
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);
    EnsureChildControlsInitialized ();
  }

  protected override void Render (HtmlTextWriter writer)
  {
    EnsureChildControlsInitialized ();
    base.Render (writer);
  }

  public new IBusinessObjectReferenceProperty Property
  {
    get 
    { 
      return (IBusinessObjectReferenceProperty) base.Property; 
    }
    set
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectReferenceProperty));

      base.Property = (IBusinessObjectReferenceProperty) value; 
    }
  }

  protected override void InitializeChildControls()
  {
    _dropDownList.Visible = ! IsReadOnly;
    _label.Visible = IsReadOnly;

    Unit innerControlWidth = Unit.Empty;
    if (! IsDesignMode)
    {
      if (this.Width.Type == _icon.Width.Type)
      {
        int innerControlWidthValue = (int) (Width.Value - _icon.Width.Value);
        innerControlWidthValue = (innerControlWidthValue > 0) ? innerControlWidthValue : 0;

        innerControlWidth = Unit.Pixel (innerControlWidthValue);
      }
      else
      {
        innerControlWidth = Width;
        Width = Unit.Empty;
      }
    }
    else
    {
      switch (Width.Type)
      {
        case UnitType.Percentage:
        {
          int innerControlWidthValue = (int) (Width.Value - 20);
          innerControlWidthValue = (innerControlWidthValue > 0) ? innerControlWidthValue : 0;

          innerControlWidth = Unit.Percentage (innerControlWidthValue);
          _icon.Width = Unit.Percentage (20);
          break;
        }
        case UnitType.Pixel:
        {
          int innerControlWidthValue = (int) (Width.Value - 24);
          innerControlWidthValue = (innerControlWidthValue > 0) ? innerControlWidthValue : 0;

          innerControlWidth = Unit.Pixel (innerControlWidthValue);
          _icon.Width = Unit.Pixel (24);
          break;
        }
        case UnitType.Point:
        {
          int innerControlWidthValue = (int) (Width.Value - 15);
          innerControlWidthValue = (innerControlWidthValue > 0) ? innerControlWidthValue : 0;

          innerControlWidth = Unit.Point (innerControlWidthValue);
          _icon.Width = Unit.Point (15);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    if (IsReadOnly)
    {
      _label.Style["vertical-align"] = "middle";
      _icon.Style["vertical-align"] = "middle";

      _label.Width = innerControlWidth;
      _label.Height = Height;
      _label.ApplyStyle (_commonStyle);
      _label.ApplyStyle (_labelStyle);

      if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
          _label.Text = c_nullDisplayName;
    }
    else
    {
      _dropDownList.Style["vertical-align"] = "bottom";
      _icon.Style["vertical-align"] = "middle";

      _dropDownList.Width = innerControlWidth;
      _dropDownList.Height = Height;
      _dropDownList.ApplyStyle (_commonStyle);
      _dropDownList.ApplyStyle (_dropDownListStyle);

    }

    _icon.ApplyStyle (_commonStyle);
    _icon.ApplyStyle (_iconStyle);
  }

  protected override void LoadViewState(object savedState)
  {
    _isLoadViewState = true;

    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    InternalValue = (string) values[1];    
    _isDirty = (bool) values[2];

    _isLoadViewState = false;
  }

  protected override object SaveViewState()
  {
    object[] values = new object[4];
    values[0] = base.SaveViewState();
    values[1] = InternalValue;
    values[2] = _isDirty;
    return values;
  }

  private object SaveListItemCollectionViewState (ListItemCollection listItems)
  {
    if (listItems == null || listItems.Count == 0)
      return null;

    ArrayList items = new ArrayList();

    foreach (ListItem listItem in listItems)
    {
      items.Add (new Pair (listItem.Text, listItem.Value));
    }
    
    return items;
  }

  private ListItem[] LoadListItemCollectionViewState(object state)
  {
    if (state == null)
      return new ListItem[]{};

    ArrayList items = (ArrayList)state;

    ListItem[] listItems = new ListItem[items.Count];
    for (int i = 0; i < items.Count; i++)
    {
      Pair item = (Pair)items[i];

      listItems[i] = new ListItem ((string)item.First, (string)item.Second);
    }

    return listItems;
  }

  private ListItem CreateEmptyItem()
  {
    ListItem emptyItem = new ListItem (string.Empty, c_nullIdentifier);
    return emptyItem;
  }

  public override BaseValidator[] CreateValidators()
  {
    if (! IsRequired)
      return new BaseValidator[]{};

    BaseValidator[] validators = new BaseValidator[1];

    CompareValidator notNullItemValidator = new CompareValidator();
    
    notNullItemValidator.ControlToValidate = TargetControl.ID;
    notNullItemValidator.ValueToCompare = c_nullIdentifier;
    notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    //  TODO: Get Message from ResourceProvider
    notNullItemValidator.ErrorMessage = c_nullItemValidationMessage;

    validators[0] = notNullItemValidator;

    return validators;
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual style settings for
  ///   the respective modes. Note that if you set one of the <c>Font</c> attributes (Bold, Italic etc.) to 
  ///   <c>true</c>, this cannot be overridden using <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> 
  ///   properties.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the TextBox (edit mode) only.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty( true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style DropDownListStyle
  {
    get { return _dropDownListStyle; }
  }

  /// <summary>
  ///   The style that you want to apply to the Label (read-only mode) only.
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

  /// <summary>
  ///   The style that you want to apply to the Icon.
  /// </summary>
  /// <remarks>
  ///   These style settings override the styles defined in <see cref="CommonStyle"/>.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the Icon.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style IconStyle
  {
    get { return _iconStyle; }
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  [Description ("Gets or sets the current value.")]
  [Browsable (false)]
  public override object Value
  {
    get 
    {
      //  Only reload if value is outdated
      if (    Property != null
          &&  (   _externalValue == null
               || ((IBusinessObjectWithIdentity)_externalValue).UniqueIdentifier != InternalValue))
      {
        _externalValue = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (
          InternalValue);
      }

      return _externalValue;
    }
    set 
    { 
      IBusinessObjectWithIdentity businessObjectWithIdentity = (IBusinessObjectWithIdentity) value;
      _externalValue = businessObjectWithIdentity; 
      
      if (businessObjectWithIdentity != null)
        InternalValue = businessObjectWithIdentity.UniqueIdentifier;
      else
        InternalValue = null;
    }
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  [Description ("Gets or sets the current value.")]
  [Browsable (false)]
  protected string InternalValue
  {
    get { return _internalValue; }
    set 
    {
      if (_internalValue == value)
        return;

      bool removeUndefined =    IsRequired 
                            &&  (     _internalValue == c_nullIdentifier
                                  ||  _internalValue == null);

      if (StringUtility.IsNullOrEmpty (value))
        _internalValue = null;
      else
        _internalValue = value;
      
      InternalLoadValue (removeUndefined);
    }
  }

  public override Control TargetControl 
  {
    get { return (! IsReadOnly) ? _dropDownList : (Control) this; }
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

  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  public override bool UseLabel
  {
    get { return false; }
  }

  [Browsable (false)]
  public DropDownList DropDown
  {
    get { return _dropDownList; }
  }

  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  [Category ("Appearance")]
  [Description ("Set true to enable the icon in front of the list")]
  [DefaultValue (true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set
    {
      if (_icon != null)
        _icon.Visible = _enableIcon;
    
      _enableIcon = value; 
    }
  }

  [Category ("Data")]
  [Description ("Set the search expression for populating the selection list.")]
  [DefaultValue ("")]
  public string Select
  {
    get { return _select; }
    set 
    {
      if (_select != value)
      {
        _select = value; 
        RefreshBusinessObjectsList();
      }
    }
  }
}

}
