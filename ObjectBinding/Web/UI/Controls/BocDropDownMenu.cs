using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

[ToolboxData("<{0}:BocDropDownMenu runat=server></{0}:BocDropDownMenu>")]
public class BocDropDownMenu : BusinessObjectBoundWebControl, IBocMenuItemContainer
{
  // constants

  // types
  
  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };
  private static readonly object s_menuItemClickEvent = new object();

  // member fields

  private DropDownMenu _dropDownMenu;
  private IBusinessObjectWithIdentity _value;
  private bool _enableIcon = true;
  private string[] _hiddenMenuItems;

  // contruction and destruction

  public BocDropDownMenu()
  {
    _dropDownMenu = new DropDownMenu (this);
  }

  // methods and properties

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    if (! IsDesignMode)
      InitializeMenusItems();
  }

  protected override void CreateChildControls()
  {
    base.CreateChildControls ();
    _dropDownMenu.ID = ID + "_Boc_DropDownMenu";
    Controls.Add (_dropDownMenu);
    _dropDownMenu.EventCommandClick += new WebMenuItemClickEventHandler (DropDownMenu_EventCommandClick);
    _dropDownMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (DropDownMenu_WxeFunctionCommandClick);
  }
 
  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);
    if (! IsDesignMode)
      PreRenderMenuItems();

    PreRenderDropDownMenu();
  }
  
  protected virtual void InitializeMenusItems()
  {
  }

  protected virtual void PreRenderMenuItems()
  {
    if (_hiddenMenuItems == null)
      return;

    BocDropDownMenu.HideMenuItems (MenuItems, _hiddenMenuItems);
  }

  public static void HideMenuItems (WebMenuItemCollection menuItems, string[] hiddenItems)
  {
    ArgumentUtility.CheckNotNull ("menuItems", menuItems);
    ArgumentUtility.CheckNotNull ("hiddenItems", hiddenItems);

    for (int idxHiddenItems = 0; idxHiddenItems < hiddenItems.Length; idxHiddenItems++)
    {
      string itemID = hiddenItems[idxHiddenItems].Trim();
      if (itemID.Length == 0)
        continue;

      bool isSuffix = itemID.IndexOf (".") == -1;
      string itemIDSuffix = null;
      if (isSuffix)
        itemIDSuffix = "." + itemID;

      for (int idxItems = 0; idxItems < menuItems.Count; idxItems++)
      {
        WebMenuItem menuItem = (WebMenuItem) menuItems[idxItems];
        if (! menuItem.IsVisible)
          continue;
        if (StringUtility.IsNullOrEmpty (menuItem.ItemID))
          continue;
        
        if (isSuffix)
        {
          if (menuItem.ItemID.Length == itemID.Length)
          {
            if (menuItem.ItemID == itemID)
              menuItem.IsVisible = false;
          }
          else
          {
            if (menuItem.ItemID.EndsWith (itemIDSuffix))
              menuItem.IsVisible = false;
          }
        }
        else
        {
          if (menuItem.ItemID == itemID)
            menuItem.IsVisible = false;
        }
      }
    }
  }

  private void PreRenderDropDownMenu()
  {
    _dropDownMenu.IsReadOnly = true;
    _dropDownMenu.Enabled = Enabled;
    _dropDownMenu.Width = Width;
    _dropDownMenu.Height = Height;
    _dropDownMenu.Style.Clear();
    foreach (string key in Style.Keys)
      _dropDownMenu.Style[key] = Style[key];

    if (Value != null)
    {
      _dropDownMenu.GetSelectionCount = "function() { return 1; }";
      _dropDownMenu.TitleText = Value.DisplayName;

     if (_enableIcon)
     {
       _dropDownMenu.TitleIcon = 
         BusinessObjectBoundWebControl.GetIcon (Value, Value.BusinessObjectClass.BusinessObjectProvider);
      }
    }
    else
    {
      _dropDownMenu.GetSelectionCount = "function() { return 0; }";
    }

    if (IsDesignMode)
      _dropDownMenu.TitleText = "##";
  }

  protected override void Render (HtmlTextWriter writer)
  {
    if (IsWaiConformanceLevelARequired && IsWcagDebuggingEnabled)
      throw new Rubicon.Web.UI.WcagException (1, this);
    _dropDownMenu.RenderControl (writer);
  }

  /// <summary> Loads the <see cref="Value"/> from the <see cref="BusinessObjectBoundWebControl.DataSource"/>. </summary>
  /// <remarks> 
  ///   If no <see cref="Property"/> is provided, the datasource's bound business object itself is used as the value.
  /// </remarks>
  /// <param name="interim"> Not used. </param>
  public override void LoadValue (bool interim)
  {
    if (DataSource != null && DataSource.BusinessObject != null)
    {
      if (Property != null)
        ValueImplementation = DataSource.BusinessObject.GetProperty (Property);
      else
        ValueImplementation = DataSource.BusinessObject;
    }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> An object implementing <see cref="IBusinessObjectWithIdentity"/>. </value>
  [Browsable (false)]
  public new IBusinessObjectWithIdentity Value
  {
    get { return _value; }
    set { _value = value; }
  }

  /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
  /// <value> The value must be of type <see cref="IList"/> or <see cref="IBusinessObjectWithIdentity"/>. </value>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IBusinessObjectWithIdentity) value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used for accessing the data to be loaded into 
  ///   <see cref="Value"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the bound 
  ///   <see cref="IBusinessObjectWithIdentity"/>'s <see cref="IBusinessObjectClass"/>. If no property is assigned, 
  ///   the <see cref="DataSource"/>'s <see cref="IBusinessObjectWithIdentity"/> itself will be used as the control's 
  ///   value.
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty Property
  {
    get { return (IBusinessObjectReferenceProperty) base.Property; }
    set { base.Property = (IBusinessObjectReferenceProperty) value; }
  }

  /// <summary> Gets a value that indicates whether properties with the specified multiplicity are supported. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is true. </returns>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
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
  /// <value> Returns always <see langword="true"/>. </value>
  public override bool UseLabel
  {
    get { return true; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; 
  ///   using its ClientID.
  /// </summary>
  public override Control TargetControl 
  {
    get { return (Control) _dropDownMenu; }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed by the menu.")]
  [DefaultValue ((string) null)]
  [Editor (typeof (BocMenuItemCollectionEditor), typeof (System.Drawing.Design.UITypeEditor))]
  public WebMenuItemCollection MenuItems
  {
    get { return _dropDownMenu.MenuItems; }
  }

  [DefaultValue (true)]
  public bool EnableGrouping
  {
    get { return _dropDownMenu.EnableGrouping; }
    set { _dropDownMenu.EnableGrouping = value; }
  }

  /// <summary> 
  ///   Handles the <see cref="DropDownMenu.EventCommandClick"/> event of the <see cref="DropDownMenu"/>.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  private void DropDownMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick (e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.OnClick"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/> and raises <see cref="MenuItemClick"/> event. 
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
  {
    WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[s_menuItemClickEvent];
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
        ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
      else
        menuItem.Command.OnClick();
    }
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

  /// <summary> Handles the <see cref="DropDownMenu.WxeFunctionCommandClick"/> event of the <see cref="DropDownMenu"/>. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  private void DropDownMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemWxeFunctionCommandClick (e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.ExecuteWxeFunction"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/>.
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.WxeFunction"/>. </remarks>
  protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
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
      else
      {
        menuItem.Command.ExecuteWxeFunction ((IWxePage) Page, null);
      }
    }
  }
  /// <summary> 
  ///   Gets or sets a flag that determines whether the <see cref="Icon"/> is shown in front of the <see cref="Value"/>.
  /// </summary>
  /// <value> <see langword="true"/> to show the <see cref="Icon"/>. The default value is <see langword="true"/>. </value>
  /// <remarks> 
  ///   An icon is only shown if the <see cref="Property"/>'s 
  ///   <see cref="IBusinessObjectClass.BusinessObjectProvider">ReferenceClass.BusinessObjectProvider</see>
  ///   provides an instance of type <see cref="IBusinessObjectWebUIService"/> and 
  ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> returns not <see langword="null"/>.
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("Flag that determines whether to show the icon in front of the value.")]
  [DefaultValue (true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set { _enableIcon = value; }
  }

  /// <summary> Gets or sets the list of menu items to be hidden. </summary>
  /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
  [Category ("Menu")]
  [Description ("The list of menu items to be hidden, identified by their ItemIDs.")]
  [DefaultValue ((string) null)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [TypeConverter (typeof (Rubicon.Web.UI.Design.StringArrayConverter))]
  public string[] HiddenMenuItems 
  {
    get 
    {
      if (_hiddenMenuItems == null)
        return new string[0];
      return _hiddenMenuItems;
    }
    set {_hiddenMenuItems = value;}
  }
  /// <summary> Gets the encapsulated <see cref="DropDownMenu"/>. </summary>
  protected DropDownMenu DropDownMenu
  {
    get { return _dropDownMenu; }
  }

  bool IBocMenuItemContainer.IsReadOnly
  {
    get
    {
      if (DataSource == null)
        return false;
      if (Property == null)
        return (DataSource.Mode == DataSourceMode.Read) ? true : false;
      else
        return true;
    }
  }

  bool IBocMenuItemContainer.IsSelectionEnabled
  {
    get { return true; }
  }

  IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects()
  {
    if (Value == null)
      return new IBusinessObject[0];
    else
      return new IBusinessObject[] {Value};
  }

  void IBocMenuItemContainer.RemoveBusinessObjects(IBusinessObject[] businessObjects)
  {
    throw new NotSupportedException ("BocDropDownMenu is a read-only control, even though the bound object might be modifiable.");
  }

  void IBocMenuItemContainer.InsertBusinessObjects(IBusinessObject[] businessObjects)
  {
    throw new NotSupportedException ("BocDropDownMenu is a read-only control, even though the bound object might be modifiable.");
  }
}

}
