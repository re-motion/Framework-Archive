using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{
[Designer (typeof (TabStripMenuDesigner))]
public class TabStripMenu: WebControl
{
  // constants
  private const string c_styleFileUrl = "TabStripMenu.css";

  // statics
  //private static readonly object s_clickEvent = new object();
  //private static readonly object s_selectionChangedEvent = new object();
  private static readonly string s_styleFileKey = typeof (TabStripMenu).FullName + "_Style";

  // types

  // fields
  private Style _mainMenuStyle;
  private Style _subMenuStyle;
  private Style _statusStyle;
  private WebTabStrip _mainMenuTabStrip;
  private WebTabStrip _subMenuTabStrip;

  protected internal WebTabStrip MainMenuTabStrip
  {
    get { return _mainMenuTabStrip; }
  }

  protected internal WebTabStrip SubMenuTabStrip
  {
    get { return _subMenuTabStrip; }
  }


  // construction and destruction
  public TabStripMenu()
  {
    _mainMenuTabStrip = new WebTabStrip (this, new Type[] {typeof (TabStripMainMenuItem)});
    _subMenuTabStrip = new WebTabStrip (this);
    _mainMenuStyle = new Style();
    _subMenuStyle = new Style();
    _statusStyle = new Style();
  }

  // methods and properties

  protected override void OnInit(EventArgs e)
  {
    EnsureChildControls();
    base.OnInit (e);
    _mainMenuTabStrip.EnableSelectedTab = true;
    _subMenuTabStrip.EnableSelectedTab = true;
  }

  [Obsolete ("Chagne to ensure pattern.")]
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad (e);
    if (! Page.IsPostBack)
      RefreshSubMenuTabStrip ();
  }

  protected internal void RefreshSubMenuTabStrip ()
  {
    TabStripMainMenuItem selectedMainMenuItem = (TabStripMainMenuItem) _mainMenuTabStrip.SelectedTab;
    _subMenuTabStrip.Tabs.Clear();
    _subMenuTabStrip.Tabs.AddRange (selectedMainMenuItem.SubMenuTabs);
    if (_subMenuTabStrip.SelectedTab == null && _subMenuTabStrip.Tabs.Count > 0)
    {
      _subMenuTabStrip.SetSelectedTab (_subMenuTabStrip.Tabs[0]);
    }
  }

  protected override void CreateChildControls()
  {
    _mainMenuTabStrip.ID = ID + "_MainMenuTabStrip";
    Controls.Add (_mainMenuTabStrip);

    _subMenuTabStrip.ID = ID + "_SubMenuTabStrip";
    Controls.Add (_subMenuTabStrip);
  }

  protected override void OnPreRender(EventArgs e)
  {
    string url = null;
    if (! HtmlHeadAppender.Current.IsRegistered (s_styleFileKey))
    {
      url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (TabStripMenu), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
    }

//    if (Views.Count == 0)
//      Views.Add (_placeHolderTabView);

    base.OnPreRender (e);
  }

  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Table; }
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender (writer);
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EnsureChildControls();
   
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      WcagHelper.Instance.HandleError (1, this);

    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin main menu row

    writer.AddAttribute (HtmlTextWriterAttribute.Colspan, "2");
    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassMainMenuCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin main menu cell
    _mainMenuTabStrip.CssClass = CssClassMainMenu;
    _mainMenuTabStrip.Width = Unit.Percentage (100);
    _mainMenuTabStrip.RenderControl (writer);
    writer.RenderEndTag(); // End main menu cell

    writer.RenderEndTag(); // End main menu row

    writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin sub menu row

    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassSubMenuCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin sub menu cell
    _subMenuTabStrip.Style["width"] = "auto";
    _subMenuTabStrip.CssClass = CssClassSubMenu;
    _subMenuTabStrip.RenderControl (writer);
    writer.RenderEndTag(); // End sub menu cell

    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassStatusCell);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin status cell
    writer.Write ("Tabbed Menu");
    writer.RenderEndTag(); // End status cell

    writer.RenderEndTag(); // End sub menu row
  }

  public override ControlCollection Controls
  {
    get
    {
      EnsureChildControls();
      return base.Controls;
    }
  }


  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("")]
  [DefaultValue ((string) null)]
  [Editor (typeof (TabStripMainMenuItemCollectionEditor), typeof (UITypeEditor))]
  public WebTabCollection Tabs
  {
    get
    {
      return _mainMenuTabStrip.Tabs;
    }
  }

//  [Category ("Style")]
//  [Description ("The style that you want to apply to the main menu.")]
//  [NotifyParentProperty (true)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//  [PersistenceMode (PersistenceMode.InnerProperty)]
//  public Style MainMenuStyle
//  {
//    get { return _mainMenuStyle; }
//  }
//
//  [Category ("Style")]
//  [Description ("The style that you want to apply to the sub menu.")]
//  [NotifyParentProperty (true)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//  [PersistenceMode (PersistenceMode.InnerProperty)]
//  public Style SubMenuStyle
//  {
//    get { return _subMenuStyle; }
//  }
//
//  [Category ("Style")]
//  [Description ("The style that you want to apply to the status area.")]
//  [NotifyParentProperty (true)]
//  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//  [PersistenceMode (PersistenceMode.InnerProperty)]
//  public Style StatusStyle
//  {
//    get { return _statusStyle; }
//  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the main menu's tab strip. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripMenuMain</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="MainMenuStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassMainMenu
  {
    get { return "tabStripMenuMain"; }
  }

  /// <summary> Gets the CSS-Class applied to the sub menu's tab strip. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripMenuSub</c>. </para>
  ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="SubMenuStyle"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassSubMenu
  {
    get { return "tabStripMenuSub"; }
  }

  /// <summary> Gets the CSS-Class applied to the main menu cell. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripMenuMainMenuCell</c>. </para>
  /// </remarks>
  protected virtual string CssClassMainMenuCell
  {
    get { return "tabStripMenuMainMenuCell"; }
  }

  /// <summary> Gets the CSS-Class applied to the sub menu cell. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripMenuSubMenuCell</c>. </para>
  /// </remarks>
  protected virtual string CssClassSubMenuCell
  {
    get { return "tabStripMenuSubMenuCell"; }
  }

  /// <summary> Gets the CSS-Class applied to the status cell. </summary>
  /// <remarks> 
  ///   <para> Class: <c>tabStripMenuStatusCell</c>. </para>
  /// </remarks>
  protected virtual string CssClassStatusCell
  {
    get { return "tabStripMenuStatusCell"; }
  }
  #endregion
}

public class TabStripMainMenuItem: WebTab
{
  private TabStripSubMenuItemCollection _subMenuTabs;

  public TabStripMainMenuItem (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    _subMenuTabs = new TabStripSubMenuItemCollection (null);
    _subMenuTabs.SetParent (this);
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public TabStripMainMenuItem()
  {
    _subMenuTabs = new TabStripSubMenuItemCollection (null);
    _subMenuTabs.SetParent (this);
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("")]
  [DefaultValue ((string) null)]
  [Editor (typeof (TabStripSubMenuItemCollectionEditor), typeof (UITypeEditor))]
  public TabStripSubMenuItemCollection SubMenuTabs
  {
    get { return _subMenuTabs; }
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged ();
    if (! (OwnerControl is TabStripMenu))
      throw new InvalidOperationException ("A TabStripMainMenuItem can only be added to a WebTabStrip that is part of a TabStripMenu.");
    _subMenuTabs.OwnerControl = OwnerControl;
  }

  protected override void LoadControlState (object state)
  {
    base.LoadControlState (state);
    if (IsSelected)
    {
      TabStripMenu tabStripMenu = (TabStripMenu) OwnerControl;
      tabStripMenu.SubMenuTabStrip.Tabs.Clear();
      tabStripMenu.SubMenuTabStrip.Tabs.AddRange (_subMenuTabs);
      if (tabStripMenu.SubMenuTabStrip.SelectedTab == null && tabStripMenu.SubMenuTabStrip.Tabs.Count > 0)
      {
        tabStripMenu.SubMenuTabStrip.SetSelectedTab (tabStripMenu.SubMenuTabStrip.Tabs[0]);
      }
    }
  }

  public override void OnSelectionChanged()
  {
    base.OnSelectionChanged ();
    TabStripMenu tabStripMenu = (TabStripMenu) OwnerControl;
    tabStripMenu.RefreshSubMenuTabStrip ();
  }
}

public class TabStripSubMenuItem: WebTab
{
  private TabStripMainMenuItem _parent;

  public TabStripSubMenuItem (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public TabStripSubMenuItem()
  {
  }
  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged ();
    if (! (OwnerControl is TabStripMenu))
      throw new InvalidOperationException ("A TabStripSubMenuItem can only be added to a WebTabStrip that is part of a TabStripMenu.");
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public TabStripMainMenuItem Parent
  {
    get { return _parent; }
  }

  protected internal void SetParent (TabStripMainMenuItem parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);
    _parent = parent;
  }

  protected override void LoadControlState (object state)
  {
    TabStripMenu tabStripMenu = (TabStripMenu) OwnerControl;
    tabStripMenu.MainMenuTabStrip.EnsureTabsRestored();
    base.LoadControlState (state);
  }

}

public class TabStripSubMenuItemCollection: WebTabCollection
{
  private TabStripMainMenuItem _parent;

  /// <summary> Initializes a new instance. </summary>
  public TabStripSubMenuItemCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public TabStripSubMenuItemCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (TabStripSubMenuItem)})
  {
  }

  protected override void OnInsertComplete (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (TabStripSubMenuItem));

    base.OnInsertComplete (index, value);
    TabStripSubMenuItem tab = (TabStripSubMenuItem) value;
    tab.SetParent (_parent);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (TabStripSubMenuItem));

    base.OnSetComplete (index, oldValue, newValue);
    TabStripSubMenuItem tab = (TabStripSubMenuItem) newValue;
    tab.SetParent (_parent);
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public TabStripMainMenuItem Parent
  {
    get { return _parent; }
  }

  protected internal void SetParent (TabStripMainMenuItem parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);
    _parent = parent;
    for (int i = 0; i < InnerList.Count; i++)
      ((TabStripSubMenuItem) InnerList[i]).SetParent (_parent);
  }
}
}
