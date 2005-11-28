using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebTab: IControlItem, IControlStateManager
{
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _itemID = "";
  private string _text = "";
  private IconInfo _icon;
  private WebTabStrip _tabStrip;
  private bool _isSelected = false;
  private int _selectDesired = 0;
  private bool _isControlStateRestored;

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text, IconInfo icon)
  {
    ItemID = itemID;
    Text = text;
    _icon = icon;
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text, string iconUrl)
    : this (itemID, text, new IconInfo (iconUrl))
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text)
    : this (itemID, text, new IconInfo (string.Empty))
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public WebTab()
  {
    _icon = new IconInfo();
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Sets this tab's <see cref="WebTabStrip"/>. </summary>
  protected internal virtual void SetTabStrip (WebTabStrip tabStrip)
  {
    _tabStrip = tabStrip; 
    if (_selectDesired == 1)
    {
      _selectDesired = 0;
      IsSelected = true;
    }
    else if (_selectDesired == -1)
    {
      _selectDesired = 0;
      IsSelected = false;
    }
  }

  /// <summary> Sets the tab's selection state. </summary>
  protected internal void SetSelected (bool value)
  {
    _isSelected = value;
    if (_tabStrip == null)
      _selectDesired = value ? 1 : -1;
  }

  public override string ToString()
  {
    string displayName = ItemID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Text;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the ID of this tab. </summary>
// /// <remarks> Must be unique within the collection of tabs. Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this tab.")]
  //No Default value
  [NotifyParentProperty (true)]
  [ParenthesizePropertyName (true)]
  public virtual string ItemID
  {
    get { return _itemID; }
    set
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      if (! StringUtility.IsNullOrEmpty (value))
      {
        WebTabCollection tabs = null;
        if (_tabStrip != null)
          tabs = _tabStrip.Tabs;
        if (tabs != null)
        {
          if (tabs.Find (value) != null)
            throw new ArgumentException (string.Format ("The collection already contains a tab with ItemID '{0}'.", value), "value");
        }
      }
      _itemID = value; 
    }
  }

  /// <summary> Gets or sets the text displayed in this tab. </summary>
  /// <remarks> Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text displayed in this tab. Use '-' for a separator tab.")]
  //No Default value
  [NotifyParentProperty (true)]
  public virtual string Text
  {
    get { return _text; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      _text = value; 
    }
  }

  /// <summary> Gets or sets the icon displayed in this tab. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The icon displayed in this tab.")]
  [NotifyParentProperty (true)]
  public virtual IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  private bool ShouldSerializeIcon()
  {
    return IconInfo.ShouldSerialize (_icon);
  }

  private void ResetIcon()
  {
    _icon.Reset();
  }

  /// <summary> Gets the <see cref="WebTabStrip"/> to which this tab belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTabStrip TabStrip
  {
    get { return _tabStrip; }
  }

  /// <summary> Gets or sets a flag that determines whether this node is the selected node of the tree view. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsSelected
  {
    get { return _isSelected; }
    set 
    {
      SetSelected (value);
      if (_tabStrip != null)
      {
        if (value)
          _tabStrip.SetSelectedTab (this);
        else if (this == _tabStrip.SelectedTab)
          _tabStrip.SetSelectedTab (null);
      }
    }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "Tab"; }
  }

  /// <summary> Gets or sets the control to which this object belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public Control OwnerControl
  {
    get { return OwnerControlImplementation;  }
    set { OwnerControlImplementation = value; }
  }

  protected virtual Control OwnerControlImplementation
  {
    get { return _ownerControl;  }
    set
    { 
      if (_ownerControl != value)
      {
        _ownerControl = value;
        OnOwnerControlChanged();
      }
    }
  }

  public virtual void RenderContents (HtmlTextWriter writer)
  {
    bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
    bool hasText = ! StringUtility.IsNullOrEmpty (_text);
    if (hasIcon)
      _icon.Render (writer);
    if (hasIcon && hasText)
      writer.Write ("&nbsp;");
    if (hasText)
      writer.Write (_text);
    if (!hasIcon && !hasText)
      writer.Write ("&nbsp;");
  }

  public virtual void OnClick()
  {
  }

  public virtual void OnSelectionChanged()
  {
  }

  public virtual void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
    if (! StringUtility.IsNullOrEmpty (key))
      Text = resourceManager.GetString (key);
    
    if (Icon != null)
      Icon.LoadResources (resourceManager);
  }

  void IControlStateManager.LoadControlState (object state)
  {
    if (_isControlStateRestored)
      return;
    _isControlStateRestored = true;
    LoadControlState (state);
  }

  protected virtual void LoadControlState (object state)
  {
    if (state == null)
      return;

    IsSelected = (bool) state;
  }

  object IControlStateManager.SaveControlState ()
  {
    return SaveControlState();
  }

  protected virtual object SaveControlState()
  {
    if (! IsSelected)
      return null;
    return IsSelected;
  }
}

}
