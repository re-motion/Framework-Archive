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
  private WcagHelper _wcagHelper;

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

    TabStripMainMenuItem selectedMainMenuItem = (TabStripMainMenuItem) _mainMenuTabStrip.SelectedTab;
    _subMenuTabStrip.Tabs.Clear();
    _subMenuTabStrip.Tabs.AddRange (selectedMainMenuItem.SubMenuTabs);
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
  private WebTabCollection _subMenu;

  public TabStripMainMenuItem (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    _subMenu = new WebTabCollection (null, new Type[] {typeof (TabStripSubMenuItem)});
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public TabStripMainMenuItem()
  {
    _subMenu = new WebTabCollection (null, new Type[] {typeof (TabStripSubMenuItem)});
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("")]
  [DefaultValue ((string) null)]
  [Editor (typeof (TabStripSubMenuItemCollectionEditor), typeof (UITypeEditor))]
  public WebTabCollection SubMenuTabs
  {
    get { return _subMenu; }
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged ();
    _subMenu.OwnerControl = OwnerControl;
  }
}

public class TabStripSubMenuItem: WebTab
{
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
}
}
