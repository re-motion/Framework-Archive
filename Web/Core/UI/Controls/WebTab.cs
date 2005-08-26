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
public class WebTab: IControlItem
{
  private const string c_separator = "-";

  public static WebTab GetSeparator()
  {
    return new WebTab (null, c_separator);
  }

  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _itemID = "";
  private string _text = "";
  private IconInfo _icon;
  private WebTabStrip _tabStrip;
  private bool _isSelected = false;
  int _selectDesired = 0;

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text, IconInfo icon)
  {
    ItemID = itemID;
    _text = text;
    _icon = icon;
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text, string iconUrl)
    : this (itemID, text, new IconInfo (iconUrl))
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string itemID, string text)
    : this (itemID, text, string.Empty)
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab()
    : this (null, null, new IconInfo ())
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Sets this tab's <see cref="WebTabStrip"/>. </summary>
  protected internal void SetParent (WebTabStrip tabStrip)
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
  protected internal void SetSelected(bool value)
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
//      //  Could not be added to collection in designer with this check enabled.
//      //ArgumentUtility.CheckNotNullOrEmpty ("value", value);
//      if (! StringUtility.IsNullOrEmpty (value))
//      {
//        WebTreeNodeCollection nodes = null;
//        if (ParentNode != null)
//          nodes = ParentNode.Children;
//        else if (TreeView != null)
//          nodes = TreeView.Nodes;
//        if (nodes != null)
//        {
//          if (nodes.Find (value) != null)
//            throw new ArgumentException ("The collection already contains a node with NodeID '" + value + "'.", "value");
//        }
//      }
      _itemID = value; 
    }
  }

  /// <exclude/>
  [Browsable (false)]
  [Obsolete ("Use ItemID instead.")]
  public string TabID
  {
    get { return ItemID; }
    set { ItemID = value; }
  }

  private bool ShouldSerializeTabID()
  {
    return false;
  }

  /// <summary> Gets or sets the text displayed in this tab. </summary>
  /// <remarks> Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text displayed in this tab. Use '-' for a separator tab.")]
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

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsSeparator
  {
    get { return _text == c_separator; }
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

  public virtual void RenderContents (HtmlTextWriter writer, string postBackEvent)
  {
    if (IsSeparator)
    {
      writer.Write ("&nbsp;");
    }
    else
    {
      bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
      bool hasText = ! StringUtility.IsNullOrEmpty (_text);
      if (! StringUtility.IsNullOrEmpty (postBackEvent))
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      }
      writer.RenderBeginTag (HtmlTextWriterTag.A);
      if (hasIcon)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Src, _icon.Url);
        writer.AddStyleAttribute ("vertical-align", "middle");
        writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag();
      }
      if (hasIcon && hasText)
        writer.Write ("&nbsp;");
      if (hasText)
        writer.Write (_text);
      if (!hasIcon && !hasText)
        writer.Write ("&nbsp;");

      writer.RenderEndTag(); // End achnor
    }
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
}

}
