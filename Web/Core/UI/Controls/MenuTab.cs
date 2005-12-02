using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

public abstract class TabStripMenuItem: WebTab
{
  /// <summary> The command rendered for this menu item. </summary>
  private SingleControlItemCollection _command = null;

  protected TabStripMenuItem (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    Initialize();
  }

  protected TabStripMenuItem()
  {
    Initialize();
  }

  private void Initialize()
  {
    _command = new SingleControlItemCollection (new Command (CommandType.Event), new Type[] {typeof (Command)});
  }

  protected TabStripMenu TabStripMenu
  {
    get { return (TabStripMenu) OwnerControl; }
  }

  /// <summary> Gets or sets the <see cref="Command"/> rendered for this menu item. </summary>
  /// <value> A <see cref="Command"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Behavior")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public virtual Command Command
  {
    get
    {
      return (Command) _command.Item; 
    }
    set 
    {
      _command.Item = value; 
    }
  }

  protected bool ShouldSerializeCommand()
  {
    if (Command == null)
      return false;

    if (Command.IsDefaultType)
      return false;
    else
      return true;
  }

  /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
  /// <remarks> 
  ///   The default value is a <see cref="Command"/> object with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  protected void ResetCommand()
  {
    if (Command != null)
    {
      Command = (Command) Activator.CreateInstance (Command.GetType());
      Command.Type = CommandType.None;
    }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  public SingleControlItemCollection PersistedCommand
  {
    get { return _command; }
  }

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   Does not persist <see cref="Command"/> objects with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  protected bool ShouldSerializePersistedCommand()
  {
    return ShouldSerializeCommand();
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();

    if (OwnerControl != null && ! (OwnerControl is TabStripMenu))
      throw new InvalidOperationException ("A TabStripSubMenuItem can only be added to a WebTabStrip that is part of a TabStripMenu.");

    if (Command != null)
      Command.OwnerControl = OwnerControl;
  }

  public override void LoadResources (IResourceManager resourceManager)
  {
    base.LoadResources (resourceManager);
    if (Command != null)
      Command.LoadResources (resourceManager);
  }

  public override void RenderBeginTagForCommand (HtmlTextWriter writer, bool isEnabled)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    if (isEnabled && Command != null)
    {
      string backedUpHref = Command.HrefCommand.Href;
      if (! ControlHelper.IsDesignMode (TabStripMenu))
        Command.HrefCommand.Href = TabStripMenu.AddTabsToUrl (Command.HrefCommand.Href, this);

      Command.RenderBegin (writer, GetPostBackClientEvent(), string.Empty);

      Command.HrefCommand.Href = backedUpHref;
    }
    else
    {
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }
  }

  public override void RenderEndTagForCommand (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    if (Command != null)
      Command.RenderEnd (writer);
    else
      writer.RenderEndTag();
  }

  public override void OnClick()
  {
    base.OnClick ();
    if (! IsSelected)
      IsSelected = true;

    if (Command != null && Command.Type == CommandType.WxeFunction)
    {
      if (TabStripMenu.Page is IWxePage)
        ExecuteWxeFunction ((IWxePage) TabStripMenu.Page, Command);
      else
        RedirectToWxeFunction (Command);
    }
  }

  protected virtual void ExecuteWxeFunction (IWxePage page, Command command)
  {
    ArgumentUtility.CheckNotNull ("page", page);
    ArgumentUtility.CheckNotNull ("command", command);
    if (command.Type != CommandType.WxeFunction)
      throw new ArgumentException ("Only commands with Type 'WxeFunction' allowed.", "command");

    Command.ExecuteWxeFunction (page, new NameObjectCollection());
  }

  protected virtual void RedirectToWxeFunction (Command command)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    if (command.Type != CommandType.WxeFunction)
      throw new ArgumentException ("Only commands with Type 'WxeFunction' allowed.", "command");

    string url = Command.FormatWxeFunctionUrl ();
    PageUtility.Redirect (TabStripMenu.Page.Response, url);
  }
}

public class TabStripMainMenuItem: TabStripMenuItem
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
    _subMenuTabs.OwnerControl = OwnerControl;
  }

  public override void OnSelectionChanged()
  {
    base.OnSelectionChanged ();
    TabStripMenu.RefreshSubMenuTabStrip (true);
  }
}

public class TabStripSubMenuItem: TabStripMenuItem
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
