using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.Design;
using System.Web.UI.HtmlControls;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> This control can be used to display or edit reference values. </summary>
/// <include file='doc\include\Controls\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("Value")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
public class BocReferenceValue: BusinessObjectBoundModifiableWebControl
{
  // constants
	
  private const string c_nullIdentifier = "--null--";

  /// <summary> 
  ///   Text displayed when control is displayed in desinger and is read-only has no contents.
  /// </summary>
  private const string c_designModeEmptyLabelContents = "#";
  private const string c_defaultControlWidth = "150pt";

  private const string c_bocReferenceValueScriptUrl = "BocReferenceValue.js";

  // types

  /// <summary> A list of control wide resources. </summary>
  /// <remarks> Resources will be accessed using IResourceManager.GetString (Enum). </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.ObjectBinding.Web.Globalization.BocReferenceValue")]
  protected enum ResourceIdentifier
  {
    OptionsTitle,
    NullDisplayName,
    NullItemValidationMessage
  }

  // static members

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };

  private static readonly object s_selectionChangedEvent = new object();
  private static readonly object s_menuItemClickEvent = new object();

	// member fields

  /// <summary>
  ///   <see langword="true"/> if <see cref="BocReferenceValue"/> has been changed since last call to
  ///   <see cref="SaveValue"/>.
  /// </summary>
  private bool _isDirty = true;

  /// <summary> The <see cref="DropDownList"/> used in edit mode. </summary>
  private DropDownList _dropDownList;
  /// <summary> The <see cref="Label"/> used in read-only mode. </summary>
  private Label _label;
  /// <summary> The <see cref="Image"/> optionally displayed in front of the value. </summary>
  private Image _icon ;
  /// <summary> The <see cref="BocDateTimeValueValidator"/> returned by <see cref="CreateValidators"/>. </summary>
  private CompareValidator _notNullItemValidator;

  /// <summary> The <see cref="Style"/> applied to the <see cref="DropDownList"/> and the <see cref="Label"/>. </summary>
  private Style _commonStyle;
  /// <summary> The <see cref="Style"/> applied to the <see cref="DropDownList"/>. </summary>
  private DropDownListStyle _dropDownListStyle;
  /// <summary> The <see cref="Style"/> applied to the <see cref="Label"/>. </summary>
  private Style _labelStyle;

  /// <summary> The object returned by <see cref="BocReferenceValue"/>. </summary>
  /// <remarks> Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. </remarks>
  private IBusinessObjectWithIdentity _value = null;

  /// <summary> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object.
  /// </summary>
  private string _internalValue = null;

  /// <summary> The <c>SelectedValue</c> of <see cref="DropDownList"/>. </summary>
  private string _newInternalValue = null;

  /// <summary> <see langword="true"/> to show the value's icon. </summary>
  private bool _enableIcon = true;

  /// <summary> 
  ///   The <see cref="string"/> with the search expression for populating the <see cref="DropDownList"/>.
  /// </summary>
  private string _select = String.Empty;

  private DropDownMenu _optionsMenu;
  private string _optionsTitle;
  /// <summary> Determines whether the options menu is shown. </summary>
  private bool _showOptionsMenu = true;
  /// <summary> The width applied to the <c>menu block</c>. </summary>
  private Unit _optionsMenuWidth = Unit.Empty;
  private BocMenuItemCollection _optionsMenuItems;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the handling of the post back events. </summary>
  private BocMenuItem[] _optionsMenuItemsPostBackEventHandlingPhase;
  /// <summary> Contains the <see cref="BocMenuItem"/> objects during the rendering phase. </summary>
  private BocMenuItem[] _optionsMenuItemsRenderPhase;

  // construction and disposing

  /// <summary> Simple constructor. </summary>
	public BocReferenceValue()
	{
    _optionsMenuItems = new BocMenuItemCollection (this);
    _commonStyle = new Style();
    _dropDownListStyle = new DropDownListStyle();
    _labelStyle = new Style();
    _icon = new Image();
    _dropDownList = new DropDownList();
    _label = new Label();
    _optionsMenu = new DropDownMenu (this);
    _notNullItemValidator = new CompareValidator();
  }

	// methods and properties

  protected override void CreateChildControls()
  {
    _icon.ID = ID + "_Boc_Icon";
    _icon.EnableViewState = false;
    Controls.Add (_icon);

    _dropDownList.ID = ID + "_Boc_DropDownList";
    _dropDownList.EnableViewState = true;
    Controls.Add (_dropDownList);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
    
    _optionsMenu.ID = ID + "_Boc_OptionsMenu";
    Controls.Add (_optionsMenu);
  }

  /// <summary>
  ///   Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    Binding.BindingChanged += new EventHandler (Binding_BindingChanged);
    _dropDownList.SelectedIndexChanged += new EventHandler(DropDownList_SelectedIndexChanged);
    _optionsMenu.EventCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_EventCommandClick);
    _optionsMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_WxeFunctionCommandClick);
  }

  /// <summary>
  ///   Calls the parent's <c>OnLoad</c> method and prepares the binding information.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    if (! IsDesignMode)
    {
      string newInternalValue = PageUtility.GetRequestCollectionItem (Page, _dropDownList.UniqueID);

      if (newInternalValue == c_nullIdentifier)
        _newInternalValue = null;
      else if (newInternalValue != null)
        _newInternalValue = newInternalValue;
      else
        _newInternalValue = null;

      if (! Page.IsPostBack && ! StringUtility.IsNullOrEmpty (_select))
        RefreshBusinessObjectList();

      if (newInternalValue != null && _newInternalValue != _internalValue)
        _isDirty = true;
    }

    _optionsMenu.MenuItems.Clear();
    _optionsMenu.MenuItems.AddRange (EnsureOptionsMenuItemsForPreviousLifeCycleGot());
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  /// <param name="e"> <see cref="EventArgs.Empty"/>. </param>
  protected virtual void OnSelectionChanged (EventArgs e)
  {
    EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
    if (eventHandler != null)
      eventHandler (this, e);
  }

  /// <summary>
  ///   Calls the parent's <c>OnPreRender</c> method and ensures that the sub-controls are properly initialized.
  /// </summary>
  /// <param name="e">An <see cref="EventArgs"/> object that contains the event data. </param>
  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    //  First call
    EnsureChildControlsPreRendered();

    string key = typeof (BocReferenceValue).FullName + "_Script";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocReferenceValue), ResourceType.Html, c_bocReferenceValueScriptUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
    }

    key = typeof (BocReferenceValue).FullName+ "_Startup";
    if (! Page.IsStartupScriptRegistered (key))
    {
      string script = string.Format ("BocReferenceValue_InitializeGlobals ('{0}');", c_nullIdentifier);
      PageUtility.RegisterStartupScriptBlock (Page, key, script);
    }

    string getSelectionCount;
    if (IsReadOnly)
    {
      if (InternalValue != null)
        getSelectionCount = "function() { return 1; }";
      else 
        getSelectionCount = "function() { return 0; }";
    }
    else
      getSelectionCount = "function() { return BocReferenceValue_GetSelectionCount ('" + _dropDownList.ClientID + "'); }";
    _optionsMenu.GetSelectionCount = getSelectionCount;
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
    EnsureChildControlsPreRendered();

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
    if (values[1] != null)    
      InternalValue = (string) values[1];  
    _isDirty = (bool) values[2];
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
    values[1] = InternalValue;
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
    if (! IsRequired)
      return new BaseValidator[]{};

    BaseValidator[] validators = new BaseValidator[1];
    
    _notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    _notNullItemValidator.ControlToValidate = TargetControl.ID;
    _notNullItemValidator.ValueToCompare = c_nullIdentifier;
    _notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    if (StringUtility.IsNullOrEmpty (_notNullItemValidator.ErrorMessage))
    {
      _notNullItemValidator.ErrorMessage = 
          GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
    }

    validators[0] = _notNullItemValidator;

    return validators;
  }

  /// <summary>
  ///   Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode.
  /// </summary>
  /// <remarks>
  ///   Use this method when manually setting the listed items, e.g. from the parent control.
  /// </remarks>
  /// <param name="businessObjects">Must not be <see langword="null"/>.</param>
  public void SetBusinessObjectReferenceList (IBusinessObjectWithIdentity[] businessObjects)
  {
    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);

    RefreshBusinessObjectList (businessObjects);
  }

  /// <summary>
  ///   Queries <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> for the
  ///   <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode.
  /// </summary>
  public void RefreshBusinessObjectList()
  {
    if (Property == null)
      return;

    IBusinessObjectWithIdentity[] businessObjects = null;

    //  Get all matching business objects
    if (DataSource != null && DataSource.BusinessObject != null && ! StringUtility.IsNullOrEmpty (_select))
      businessObjects = Property.SearchAvailableObjects (DataSource.BusinessObject, _select);

    RefreshBusinessObjectList (businessObjects);
  }

  /// <summary>
  ///   Populates the <see cref="DropDownList"/> with the items passed in <paramref name="businessObjects"/>.
  /// </summary>
  /// <remarks>
  ///   This method controls the actual refilling of the <see cref="DropDownList"/>.
  /// </remarks>
  /// <param name="businessObjects">
  ///   The <see cref="IBusinessObjectWithIdentity"/> objects to place in the 
  ///   <see cref="DropDownList"/>.
  /// </param>
  public virtual void RefreshBusinessObjectList (IBusinessObjectWithIdentity[] businessObjects)
  {
    if (! IsReadOnly)
    {
      if (businessObjects != null)
      {
        _dropDownList.Items.Clear();
      
        //  Add Undefined item
        if (! IsRequired)
          _dropDownList.Items.Add (CreateNullItem());

        if (businessObjects != null)
        {
          //  Populate _dropDownList
          foreach (IBusinessObjectWithIdentity businessObject in businessObjects)
          {
            ListItem item = new ListItem (
              businessObject.DisplayName,
              businessObject.UniqueIdentifier);

            _dropDownList.Items.Add (item);
          }
        }
      }
    }
  }

  /// <summary>
  ///   Prerenders the child controls.
  /// </summary>
  protected override void PreRenderChildControls()
  {
    PreRenderIcon();

    if (HasOptionsMenu)
      PreRenderOptionsMenu();

    if (IsReadOnly)
      PreRenderReadOnlyValue();
    else
      PreRenderEditModeValue();
  }

  /// <summary>
  ///   Prerenders the <see cref="Label"/>.
  /// </summary>
  private void PreRenderReadOnlyValue()
  {
    if (Value != null)
      _label.Text = Value.DisplayName;
    else
      _label.Text = String.Empty;

    _label.Width = Unit.Percentage (100);
    _label.Height = Height;
    _label.ApplyStyle (_commonStyle);
    _label.ApplyStyle (_labelStyle);

    if (IsDesignMode && StringUtility.IsNullOrEmpty (_label.Text))
    {
      _label.Text = c_designModeEmptyLabelContents;
      //  Too long, can't resize in designer to less than the content's width
      //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
    }
  }

  /// <summary>
  ///   Prerenders the <see cref="DropDownList"/>.
  /// </summary>
  private void PreRenderEditModeValue()
  {
    bool isNullItem = InternalValue == null;

    //  Check if null item is to be selected
    if (isNullItem)
    {
      //  No null item in the list
      if (_dropDownList.Items.FindByValue (c_nullIdentifier) == null)
        _dropDownList.Items.Insert (0, CreateNullItem());
      _dropDownList.SelectedValue = c_nullIdentifier;
    }
    else
    {
      if (_dropDownList.Items.FindByValue (InternalValue) != null)
      {
        _dropDownList.SelectedValue = InternalValue;
      }
        //  Item not yet in the list but is a valid item.
      else if (Value != null)
      {
        IBusinessObjectWithIdentity businessObject = Value;

        ListItem item = new ListItem (businessObject.DisplayName, businessObject.UniqueIdentifier);
        _dropDownList.Items.Add (item);

        _dropDownList.SelectedValue = InternalValue;
      }
    }

    _dropDownList.ApplyStyle (_commonStyle);
    _dropDownList.Width = Unit.Percentage (100);
    _dropDownList.Height = Height;
    _dropDownListStyle.ApplyStyle (_dropDownList);
  }

  /// <summary>
  ///   Prerenders the <see cref="Icon"/>.
  /// </summary>
  private void PreRenderIcon()
  {
    //  Get icon
    if (_enableIcon && Property != null)
    {
      IconInfo iconInfo = BusinessObjectBoundWebControl.GetIcon (Value, Property.ReferenceClass.BusinessObjectProvider);

      if (iconInfo != null)
      {
        _icon.ImageUrl = iconInfo.Url;
        _icon.Width = iconInfo.Width;
        _icon.Height = iconInfo.Height;

        _icon.Visible = _enableIcon;
      }
      else
      {
        _icon.Visible = false;
      }
    }
    else
    {
      _icon.Visible = false;
    }
  }

  /// <summary>
  ///   Prerenders the <see cref="_optionsMenu"/>.
  /// </summary>
  private void PreRenderOptionsMenu()
  {
    _optionsMenu.MenuItems.Clear();
    _optionsMenu.MenuItems.AddRange (EnsureOptionsMenuItemsGot());
    if (StringUtility.IsNullOrEmpty (_optionsTitle))
      _optionsMenu.TitleText = GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
    else
      _optionsMenu.TitleText = _optionsTitle;
  }

  protected override void RenderChildren(HtmlTextWriter writer)
  {
    if (Width.IsEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
    else
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Width.ToString());

    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);
    writer.RenderBeginTag (HtmlTextWriterTag.Tr);

    if (_icon.Visible)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("padding-right", "3pt");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      _icon.RenderControl (writer);
      writer.RenderEndTag();
    }   

    if (IsReadOnly)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      _label.RenderControl (writer);
      writer.RenderEndTag();
    }
    else
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      _dropDownList.RenderControl (writer);
      writer.RenderEndTag();

    }

    if (HasOptionsMenu)
    {
      writer.AddStyleAttribute ("padding-left", "3pt");
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);
      _optionsMenu.Width = _optionsMenuWidth;
      _optionsMenu.RenderControl (writer);
      writer.RenderEndTag();
    }

    writer.RenderEndTag();
    writer.RenderEndTag();
  }


  private bool HasOptionsMenu
  {
    get { return _showOptionsMenu && EnsureOptionsMenuItemsGot().Length > 0; }
  }


  /// <summary>
  ///   Raises this control's <see cref="SelectionChanged"/> event if the value was changed 
  ///   through the <see cref="DropDownList"/>.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void DropDownList_SelectedIndexChanged (object sender, EventArgs e)
  {
    if (_newInternalValue != _internalValue)
    {
      InternalValue = _newInternalValue;
      OnSelectionChanged (EventArgs.Empty);
    }
  }

  /// <summary> Handles refreshing the bound control. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
  private void Binding_BindingChanged (object sender, EventArgs e)
  {
    // Nothing to do.
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    ListItem emptyItem = new ListItem (string.Empty, c_nullIdentifier);
    return emptyItem;
  }

    private BocMenuItem[] EnsureOptionsMenuItemsForPreviousLifeCycleGot()
  {
    if (_optionsMenuItemsPostBackEventHandlingPhase == null)
    {
      _optionsMenuItemsPostBackEventHandlingPhase = 
          GetOptionsMenuItemsForPreviousLifeCycle (_optionsMenuItems.ToArray());
    }
    return _optionsMenuItemsPostBackEventHandlingPhase;
  }

  private BocMenuItem[] EnsureOptionsMenuItemsGot()
  {
    if (_optionsMenuItemsRenderPhase == null)
      _optionsMenuItemsRenderPhase = GetOptionsMenuItems (_optionsMenuItems.ToArray());
    return _optionsMenuItemsRenderPhase;
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocReferenceValue"/>'s options menu
  ///   during the previous page life cycle.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="BocColumnDefinition"/> instances displayed during the last page life cycle are required 
  ///     to correctly handle the events raised on the BocList, such as an <see cref="Command"/> event 
  ///     or a data changed event.
  ///   </para><para>
  ///     Make the method <c>protected virtual</c> should this feature be ever required and change the 
  ///     method's body to return the passed <c>menuItems</c>.
  ///   </para>
  /// </remarks>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  private BocMenuItem[] GetOptionsMenuItemsForPreviousLifeCycle (BocMenuItem[] menuItems)
  {
    //  return menuItems;
    return EnsureOptionsMenuItemsGot();
  }

  /// <summary>
  ///   Override this method to modify the menu items displayed in the <see cref="BocReferenceValue"/>'s options menu
  ///   in the current page life cycle.
  /// </summary>
  /// <remarks>
  ///   This call can happen more than once in the control's life cycle, passing different 
  ///   arrays in <paramref name="menuItems" />. It is therefor important to not cache the return value
  ///   in the override of <see cref="GetOptionsMenuItems"/>.
  /// </remarks>
  /// <param name="menuItems"> 
  ///   The <see cref="BocMenuItem"/> array containing the menu item available in the options menu. 
  /// </param>
  /// <returns> The <see cref="BocMenuItem"/> array. </returns>
  protected virtual BocMenuItem[] GetOptionsMenuItems (BocMenuItem[] menuItems)
  {
    return menuItems;
  }

  private void OptionsMenu_EventCommandClick(object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick ((BocMenuItem) e.Item);
  }

  protected virtual void OnMenuItemEventCommandClick (BocMenuItem menuItem)
  {
    WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[s_menuItemClickEvent];
    if (menuItem != null && menuItem.Command != null)
      ((BocMenuItemCommand) menuItem.Command).OnClick (menuItem);
    if (menuItemClickHandler != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
      menuItemClickHandler (this, e);
    }
  }

  /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Is raised when a menu item with a command of type Event is clicked.")]
  public event WebMenuItemClickEventHandler MenuItemClick
  {
    add { Events.AddHandler (s_menuItemClickEvent, value); }
    remove { Events.RemoveHandler (s_menuItemClickEvent, value); }
  }

  private void OptionsMenu_WxeFunctionCommandClick(object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemWxeFunctionCommandClick ((BocMenuItem) e.Item);
  }

  protected virtual void OnMenuItemWxeFunctionCommandClick (BocMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      int[] indices = new int[0];
      IBusinessObject[] businessObjects;
      if (Value != null)
        businessObjects = new IBusinessObject[] { Value };
      else
        businessObjects = new IBusinessObject[0];
 
      BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
      command.ExecuteWxeFunction ((IWxePage) Page, indices, businessObjects);
    }
  }

  /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
  /// <remarks> Explicit setting of <see cref="Property"/> is not offically supported. </remarks>
  /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty Property
  {
    get { return (IBusinessObjectReferenceProperty) base.Property; }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (IBusinessObjectReferenceProperty));

      base.Property = (IBusinessObjectReferenceProperty) value; 
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity"/> currently displayed 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  [Browsable (false)]
  public new IBusinessObjectWithIdentity Value
  {
    get 
    {
      if (InternalValue == null)
      {
        _value = null;
      }
      //  Only reload if value is outdated
      else if (   Property != null
               && (   _value == null
                   || _value.UniqueIdentifier != InternalValue))
      {
        _value = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (InternalValue);
      }

      return _value;
    }
    set 
    { 
      IBusinessObjectWithIdentity businessObjectWithIdentity = value;
      _value = businessObjectWithIdentity; 
      
      if (businessObjectWithIdentity != null)
        InternalValue = businessObjectWithIdentity.UniqueIdentifier;
      else
        InternalValue = null;
    }
  }

  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IBusinessObjectWithIdentity) value; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> for the current 
  ///   <see cref="IBusinessObjectWithIdentity"/> object 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  protected string InternalValue
  {
    get { return _internalValue; }
    set 
    {
      if (_internalValue == value)
        return;

      bool isOldInternalValueNull = _internalValue == null;

      if (StringUtility.IsNullOrEmpty (value))
        _internalValue = null;
      else
        _internalValue = value;

      bool removeNullItem =    IsRequired 
                            && isOldInternalValueNull
                            && _internalValue != null;
      
      if (removeNullItem)
      {
        ListItem itemToRemove = _dropDownList.Items.FindByValue (c_nullIdentifier);
        _dropDownList.Items.Remove (itemToRemove);
      }
    }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using it's ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (! IsReadOnly) ? _dropDownList : (Control) this; }
  }

  /// <summary>
  ///   Specifies whether the object reference within the control has been changed 
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
    get { return false; }
  }

  /// <summary> This event is fired when the selection is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the selection changes.")]
  public event EventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary> The style that you want to apply to the TextBox (edit mode) and the Label (read-only mode). </summary>
  /// <remarks>
  ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <c>Font</c> 
  ///   attributes (Bold, Italic etc.) to <c>true</c>, this cannot be overridden using 
  ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
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

  /// <summary> The style that you want to apply to the TextBox (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the TextBox (edit mode) only.")]
  [NotifyParentProperty( true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public DropDownListStyle DropDownListStyle
  {
    get { return _dropDownListStyle; }
  }

  /// <summary> The style that you want to apply to the Label (read-only mode) only. </summary>
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
    set { _enableIcon = value; }
  }

  /// <summary> The search expression used to populate the selection list in edit mode. </summary>
  /// <value> A <see cref="String"/> with a valid search expression. </value>
  [Category ("Data")]
  [Description ("Set the search expression for populating the selection list.")]
  [DefaultValue ("")]
  public string Select
  {
    get { return _select; }
    set { _select = value; }
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <c>options menu</c>. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed by options menu.")]
  [DefaultValue ((string) null)]
  public BocMenuItemCollection OptionsMenuItems
  {
    get { return _optionsMenuItems; }
  }

  /// <summary> Gets or sets the text that is rendered as a label for the <c>options menu</c>. </summary>
  [Category ("Menu")]
  [Description ("The text that is rendered as a label for the options menu.")]
  [DefaultValue ("")]
  public string OptionsTitle
  {
    get { return _optionsTitle; }
    set { _optionsTitle = value; }
  }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display the options menu.
  /// </summary>
  /// <value> <see langword="true"/> to show the options menu. </value>
  [Category ("Menu")]
  [Description ("Enables the options menu.")]
  [DefaultValue (true)]
  public bool ShowOptionsMenu
  {
    get { return _showOptionsMenu; }
    set { _showOptionsMenu = value; }
  }

  /// <summary> Gets or sets the width of the options menu. </summary>
  [Category ("Menu")]
  [Description ("The width of the options menu.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit OptionsMenuWidth
  {
    get { return _optionsMenuWidth; }
    set { _optionsMenuWidth = value; }
  }

  /// <summary> Gets or sets the validation error message. </summary>
  [Description("Validation error message if the selection is invalid.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string ErrorMessage
  {
    get { return _notNullItemValidator.ErrorMessage; }
    set { _notNullItemValidator.ErrorMessage = value; }
  }
}

}
