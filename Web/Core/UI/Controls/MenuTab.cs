using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

public abstract class MenuTab: WebTab
{
  private SingleControlItemCollection _command = null;
  /// <summary> The command being rendered by this menu item. </summary>
  private NavigationCommand _renderingCommand = null;

  protected MenuTab (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    Initialize();
  }

  protected MenuTab()
  {
    Initialize();
  }

  private void Initialize()
  {
    _command = new SingleControlItemCollection (new NavigationCommand (), new Type[] {typeof (NavigationCommand)});
  }

  protected TabbedMenu TabbedMenu
  {
    get { return (TabbedMenu) OwnerControl; }
  }

  /// <summary> Gets or sets the <see cref="NavigationCommand"/> rendered for this menu item. </summary>
  /// <value> A <see cref="NavigationCommand"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Behavior")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public virtual NavigationCommand Command
  {
    get
    {
      return (NavigationCommand) _command.ControlItem; 
    }
    set 
    {
      _command.ControlItem = value; 
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
      Command = (NavigationCommand) Activator.CreateInstance (Command.GetType());
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

    if (OwnerControl != null && ! (OwnerControl is TabbedMenu))
      throw new InvalidOperationException ("A SubMenuTab can only be added to a WebTabStrip that is part of a TabbedMenu.");

    if (Command != null)
      Command.OwnerControl = OwnerControl;
  }

  public override void LoadResources (IResourceManager resourceManager)
  {
    base.LoadResources (resourceManager);
    if (Command != null)
      Command.LoadResources (resourceManager);
  }

  public override void RenderBeginTagForCommand (HtmlTextWriter writer, bool isEnabled, WebTabStyle style)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("style", style);

    _renderingCommand = GetCommandToRender();
    if (isEnabled && _renderingCommand != null && ! IsDisabled)
    {
      NameValueCollection additionalUrlParameters = TabbedMenu.GetUrlParameters (this);
      _renderingCommand.RenderBegin (
          writer, GetPostBackClientEvent(), new string[0], string.Empty, additionalUrlParameters, false, style);
    }
    else
    {
      style.AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }
  }

  public override void RenderEndTagForCommand (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    if (_renderingCommand != null)
      _renderingCommand.RenderEnd (writer);
    else
      writer.RenderEndTag();
    _renderingCommand = null;
  }

  protected virtual NavigationCommand GetCommandToRender()
  {
    return Command;
  }

  public override void OnClick()
  {
    base.OnClick ();
    if (! IsSelected)
      IsSelected = true;
  }
  
  public override bool EvaluateVisibile()
  {
    if (Command != null)
    {
      if (   WcagHelper.Instance.IsWaiConformanceLevelARequired()
          && Command.Type == CommandType.Event)
      {
        return false;
      }
    }
    
    return base.EvaluateVisibile();
  }

}

public class MainMenuTab: MenuTab
{
  private SubMenuTabCollection _subMenuTabs;

  public MainMenuTab (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    _subMenuTabs = new SubMenuTabCollection (OwnerControl);
    _subMenuTabs.SetParent (this);
  }

  public MainMenuTab (string itemID, string text)
    : this (itemID, text, null)
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public MainMenuTab()
  {
    _subMenuTabs = new SubMenuTabCollection (OwnerControl);
    _subMenuTabs.SetParent (this);
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("")]
  [DefaultValue ((string) null)]
  public SubMenuTabCollection SubMenuTabs
  {
    get { return _subMenuTabs; }
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged ();
    _subMenuTabs.OwnerControl = OwnerControl;
  }

  protected override void OnSelectionChanged()
  {
    base.OnSelectionChanged ();
    TabbedMenu.RefreshSubMenuTabStrip ();
  }

  protected override NavigationCommand GetCommandToRender()
  {
    if (Command == null)
      return null;

    if (Command.Type == CommandType.None)
    {
      foreach (SubMenuTab subMenuTab in _subMenuTabs)
      {
        bool isTabActive = subMenuTab.IsVisible && ! subMenuTab.IsDisabled;
        bool isCommandActive = subMenuTab.Command != null && subMenuTab.Command.Type != CommandType.None;
        if (isTabActive && isCommandActive)
          return subMenuTab.Command;
      }
    }

    return Command;
  }
}

public class SubMenuTab: MenuTab
{
  private MainMenuTab _parent;

  public SubMenuTab (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
  }

  public SubMenuTab (string itemID, string text)
    : this (itemID, text, null)
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public SubMenuTab()
  {
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public MainMenuTab Parent
  {
    get { return _parent; }
  }

  protected internal void SetParent (MainMenuTab parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);
    _parent = parent;
  }
}

}
