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
  ///   <see langword="true"/> if <see cref="ReferenceValue"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used in edit mode. </summary>
  private DropDownList _dropDownList = null;

  /// <summary> The <see cref="Label"/> used in read-only mode. </summary>
  private Label _label = null;

  /// <summary> The <see cref="Image"/> optionally displayed in front of the value. </summary>
  private Image _icon = null;

  /// <summary> The object returned by <see cref="ReferenceValue"/>. </summary>
  /// <remarks> Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. </remarks>
  private IBusinessObjectWithIdentity _referenceValue = null;

  /// <summary> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object.
  /// </summary>
  private string _internalValue = null;

  /// <summary> The <c>SelectedValue</c> of <see cref="DropDownList"/>. </summary>
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
    //  Set the display value
    if (IsReadOnly)
    {
      if (ReferenceValue != null)
        _label.Text = ReferenceValue.DisplayName;
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

      bool hasPropertyAfterInitializion = _isLoadViewState || Property != null;

      //  Check if null item is to be selected
      if (InternalValue == null || ! hasPropertyAfterInitializion)
      {
        //  No null item in the list
        if (_dropDownList.Items.FindByValue (c_nullIdentifier) == null)
        {
          _dropDownList.Items.Insert (0, CreateNullItem());
        }

        _dropDownList.SelectedValue = c_nullIdentifier;
      }
      else
      {
        if (_dropDownList.Items.FindByValue (InternalValue) != null)
        {
          _dropDownList.SelectedValue = InternalValue;
        }
          //  Item not yet in the list but is a valid item.
        else if (ReferenceValue != null)
        {
          IBusinessObjectWithIdentity businessObject = ReferenceValue;

          _dropDownList.Items.Add (
            new ListItem (businessObject.DisplayName, businessObject.UniqueIdentifier));

          _dropDownList.SelectedValue = InternalValue;
        }
      }
    }

    //  Get icon
    if (Property != null)
    {
      IBusinessObjectService service
        = Property.ReferenceClass.BusinessObjectProvider.GetService(
          typeof (IBusinessObjectWebUIService));

      IBusinessObjectWebUIService webUIService = service as IBusinessObjectWebUIService;

      IconPrototype iconPrototype = null;
      if (webUIService != null)
      {
        if (ReferenceValue != null)
          iconPrototype = webUIService.GetIcon (ReferenceValue);
        else
          iconPrototype = webUIService.GetIcon (
            (IBusinessObjectClassWithIdentity) Property.ReferenceClass);
      }

      if (iconPrototype != null)
      {
        _icon.ImageUrl = iconPrototype.Url;
        _icon.Width = iconPrototype.Width;
        _icon.Height = iconPrototype.Height;

        _icon.Visible = EnableIcon;
      }
      else
      {
        _icon.Visible = false;

        //  For debugging
        //  _icon.ImageUrl = "/images/Help.gif";  // HACK: Should be String.Empty;
        //  _icon.Width = Unit.Pixel(24);         // HACK: Should be Unit.Empty;
        //  _icon.Height = Unit.Pixel(24);        // HACK: Should be Unit.Empty;
        //  _icon.Visible = true;                 // HACK: Should be false
      }
    }
    else
    {
      _icon.Visible = false;
    }
  }

  /// <summary>
  ///   Loads the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> or uses the cached
  ///   information if <paramref name="interim"/> is <see langword="false"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="true"/> to load the <see cref="Value"/> from the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      Binding.EvaluateBinding();
      
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        ReferenceValue 
          = (IBusinessObjectWithIdentity) DataSource.BusinessObject.GetProperty (Property);
        
        _isDirty = false;
      }
    }
  }

  /// <summary>
  ///   Writes the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/> if <paramref name="interim"/> 
  ///   is <see langword="true"/>.
  /// </summary>
  /// <param name="interim">
  ///   <see langword="false"/> to write the <see cref="Value"/> into the 
  ///   <see cref="BusinessObjectBoundWebControl.DataSource"/>.
  /// </param>
  public override void SaveValue (bool interim)
  {
    if (! interim)
    {
      Binding.EvaluateBinding();

      if (Property != null && DataSource != null &&  DataSource.BusinessObject != null && ! IsReadOnly)
        DataSource.BusinessObject.SetProperty (Property, ReferenceValue);
    }
  }

  /// <summary>
  ///   Populates the <see cref="DropDownList"/> with the items passes in 
  ///   <paramref name="businessObjects"/>.
  /// </summary>
  /// <remarks>
  ///   This method controls the actual refilling of the <see cref="_dropDownList"/>.
  /// </remarks>
  /// <param name="businessObjects">
  ///   The <see cref="IBusinessObjectWithIdentity"/> objects to place in the 
  ///   <see cref="_dropDownList"/>. Must not be <see langword="null"/>.
  /// </param>
  protected virtual void RefreshDropDownList(IBusinessObjectWithIdentity[] businessObjects)
  {
    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

    if (    Property != null 
        &&  businessObjects != null)
    {
      _dropDownList.Items.Clear();
    
      //  Add Undefined item
      if (InternalValue == null || !IsRequired)
      {
        _dropDownList.Items.Add (CreateNullItem());
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
  ///   Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in 
  ///   edit mode.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The control has to be in edit mode 
  ///     (<see cref="BusinessObjectBoundModifiableWebControl.ReadOnly"/> set to 
  ///     <see langword="false"/>) to refresh the list.
  ///   </para><para>
  ///     Use this method when manually setting the listed items, i.e. from the parent control.
  ///   </para>
  /// </remarks>
  /// <param name="businessObjects">Must not be <see langword="null"/>.</param>
  public void SetBusinessObjectsList (IBusinessObjectWithIdentity[] businessObjects)
  {
    if (IsReadOnly)
      return;

    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

    RefreshDropDownList (businessObjects);
  }

  /// <summary>
  ///   Queries <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> for the
  ///   <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The control has to be in edit mode
  ///     (<see cref="BusinessObjectBoundModifiableWebControl.ReadOnly"/> set to 
  ///     <see langword="false"/>) to refresh the list.
  ///   </para>
  /// </remarks>
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

  /// <summary>
  ///   Calls the parent's <c>OnPreRender</c> method and ensures that the sub-controls are 
  ///   properly initialized.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    //  First call
    EnsureChildControlsInitialized();
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
    EnsureChildControlsInitialized();

    base.Render (writer);
  }

  /// <summary>
  ///   Initializes the child controls.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <c>Width</c> of the control and the <c>Width</c> property of <see cref="Icon"/> must
  ///     be of the same <see cref="UnitType"/>. If they are not, the width of the 
  ///     icon is added to the specified total width of the control. It is recommended to always 
  ///     specify the <c>Width</c> in pixels.
  ///   </para>
  ///   <para>
  ///     During design-time, the width of the icon versus the width of the list or label
  ///     is only approximated.
  ///   </para>
  /// </remarks>
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
      //  Handle icon width approximation
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

  /// <summary>
  ///   Calls the parents <c>LoadViewState</c> method and restores this control's specific data.
  /// </summary>
  /// <param name="savedState">
  ///   An <see cref="Object"/> that represents the control state to be restored.
  /// </param>
  protected override void LoadViewState(object savedState)
  {
    _isLoadViewState = true;

    object[] values = (object[]) savedState;
    base.LoadViewState (values[0]);
    InternalValue = (string) values[1];    
    _isDirty = (bool) values[2];

    _isLoadViewState = false;
  }

  /// <summary>
  ///   Calls the parents <c>SaveViewState</c> method and saves this control's specific data.
  /// </summary>
  /// <returns>
  ///   Returns the server control's current view state.
  /// </returns>
  protected override object SaveViewState()
  {
    object[] values = new object[4];
    values[0] = base.SaveViewState();
    values[1] = InternalValue;
    values[2] = _isDirty;
    return values;
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    ListItem emptyItem = new ListItem (string.Empty, c_nullIdentifier);
    return emptyItem;
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
  ///   The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to.
  /// </summary>
  /// <remarks>
  ///   Explicit setting of <see cref="Property"/> is not offically supported.
  /// </remarks>
  /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
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
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity"/> currently displayed 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  [Description ("Gets or sets the current value.")]
  [Browsable (false)]
  public virtual IBusinessObjectWithIdentity ReferenceValue
  {
    get 
    {
      //  Only reload if value is outdated
      if (    Property != null
          &&  (   _referenceValue == null
               || _referenceValue.UniqueIdentifier != InternalValue))
      {
        _referenceValue = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (
          InternalValue);
      }

      return _referenceValue;
    }
    set 
    { 
      IBusinessObjectWithIdentity businessObjectWithIdentity = value;
      _referenceValue = businessObjectWithIdentity; 
      
      if (businessObjectWithIdentity != null)
        InternalValue = businessObjectWithIdentity.UniqueIdentifier;
      else
        InternalValue = null;
    }
  }
  
  /// <summary>
  ///   Gets or sets the current value. Must be of type <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity"/> currently displayed 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  [Description ("Gets or sets the current value.")]
  [Browsable (false)]
  public override object Value
  {
    get { return ReferenceValue;  }
    set
    {
      if (value != null)
        ReferenceValue = (IBusinessObjectWithIdentity) value;
      else
        ReferenceValue = null;
    }
  }

  /// <summary>
  ///   Gets or sets the current value.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> for the current 
  ///   <see cref="IBusinessObjectWithIdentity"/> object
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
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

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using it's ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (! IsReadOnly) ? _dropDownList : (Control) this; }
  }

  /// <summary>
  ///   Specifies whether the text within the control has been changed since the last load/save 
  ///   operation.
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

  /// <summary> Overrides <see cref="Rubicon.Web.UI.Controls.ISmartControl.UseLabel"/>. </summary>
  public override bool UseLabel
  {
    get { return false; }
  }

  /// <summary> Gets the <see cref="DropDownList"/> used in edit mode. </summary>
  [Browsable (false)]
  public DropDownList DropDownList
  {
    get { return _dropDownList; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="Image"/> used as an icon. </summary>
  [Browsable (false)]
  public Image Icon
  {
    get { return _icon; }
  }

  /// <summary>
  ///   Set <see langword="true"/> to display an icon for the currently selected 
  ///   <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
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

  /// <summary> The search expression used to populate the selection list in edit mode. </summary>
  /// <value> A <see cref="String"/> with a valid search expression. </value>
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
