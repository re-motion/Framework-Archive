using System.ComponentModel;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  public class MainMenuTab : MenuTab
  {
    private SubMenuTabCollection _subMenuTabs;
    private MenuTab _activeTab;

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
    public MainMenuTab ()
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

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged ();
      _subMenuTabs.OwnerControl = OwnerControl;
    }

    protected override void OnSelectionChanged ()
    {
      base.OnSelectionChanged ();
      TabbedMenu.RefreshSubMenuTabStrip ();
    }

    protected override MenuTab GetActiveTab ()
    {
      if (_activeTab != null)
        return _activeTab;

      _activeTab = this;
      if (Command.Type == CommandType.None)
      {
        foreach (SubMenuTab subMenuTab in _subMenuTabs)
        {
          bool isTabActive = subMenuTab.EvaluateVisible () && subMenuTab.EvaluateEnabled ();
          bool isCommandActive = subMenuTab.Command != null && subMenuTab.Command.Type != CommandType.None;
          if (isTabActive && isCommandActive)
          {
            _activeTab = subMenuTab;
            break;
          }
        }
      }

      return _activeTab;
    }
  }
}