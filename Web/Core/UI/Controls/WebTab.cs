using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebTab: IControlItem
{
  private const string c_separator = "-";

  public static WebTab GetSeparator()
  {
    return new WebTab (null, "-");
  }

  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;
  private string _tabID = "";
  private string _text = "";
  private IconInfo _icon;
  private WebTabStrip _tabStrip;
  private bool _isSelected = false;
  int _selectDesired = 0;

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string tabID, string text, IconInfo icon)
  {
    TabID = tabID;
    _text = text;
    _icon = icon;
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string tabID, string text, string iconUrl)
    : this (tabID, text, new IconInfo (iconUrl))
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTab (string tabID, string text)
    : this (tabID, text, string.Empty)
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
    string displayName = TabID;
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
  [Description ("The ID of this node.")]
  //No Default value
  [NotifyParentProperty (true)]
  [ParenthesizePropertyName (true)]
  public virtual string TabID
  {
    get { return _tabID; }
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
      _tabID = value; 
    }
  }

  /// <summary> Gets or sets the text displayed in this tab. </summary>
  /// <remarks> Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text displayed in this tab. Use '-' for a separator menu.")]
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
  [Description ("The icon displayed in this tree node.")]
  [NotifyParentProperty (true)]
  public virtual IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  private bool ShouldSerializeIcon()
  {
    if (_icon == null)
    {
      return false;
    }
    else if (   StringUtility.IsNullOrEmpty (_icon.Url)
             && _icon.Height.IsEmpty
             && _icon.Width.IsEmpty)
    {
      return false;
    }
    else
    {
      return true;
    }
  }

  private void ResetIcon()
  {
    _icon = new IconInfo (string.Empty, Unit.Empty, Unit.Empty);
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

  public virtual void RenderContents (HtmlTextWriter writer, string postBackLink)
  {
    if (IsSeparator)
    {
        writer.Write ("&nbsp;");
    }
    else
    {
      bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
      bool hasText = ! StringUtility.IsNullOrEmpty (_text);
      bool hasPostBackLink = ! StringUtility.IsNullOrEmpty (postBackLink);
      if (hasPostBackLink)
        writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
      if (hasIcon)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Src, _icon.Url);
        writer.AddStyleAttribute ("vertical-align", "middle");
        writer.AddStyleAttribute ("border", "0");
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag();
      }
      if (hasIcon && hasText)
        writer.Write ("&nbsp;");
      if (hasText)
        writer.Write (_text);
      writer.RenderEndTag();
    }
  }

  public virtual void OnClick()
  {
  }

  public virtual void OnSelectionChanged()
  {
  }
}

public class MultiPageTab: WebTab
{
  string _target;

  /// <summary> Initalizes a new instance. </summary>
  public MultiPageTab (string tabID, string text, IconInfo icon)
    : base (tabID, text, icon)
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public MultiPageTab (string tabID, string text, string iconUrl)
    : this (tabID, text, new IconInfo (iconUrl))
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public MultiPageTab (string tabID, string text)
    : this (tabID, text, string.Empty)
  {
  }

  /// <summary> Initalizes a new instance. </summary>
  public MultiPageTab()
    : this (null, null, new IconInfo ())
  {
  }

  public string Target
  {
    get { return _target; }
    set { _target = value; }
  }

  public override void OnSelectionChanged()
  {
    PageView pageView = (PageView) TabStrip.FindControl (_target);
    MultiPage multiPage = (MultiPage) pageView.Parent;
    int selectedPageView = multiPage.Controls.IndexOf (pageView);
    multiPage.SelectedIndex = selectedPageView;
//    if (multiPage != null)
//    {
//      if (_OldMultiPageIndex < 0)
//      {
//        this.SetTargetSelectedIndex();
//      }
//      if ((_OldMultiPageIndex >= 0) && (multiPage.SelectedIndex != _OldMultiPageIndex))
//      {
//        multiPage.FireSelectedIndexChangeEvent();
//      }
//    }
  }

//  private void SetTargetSelectedIndex()
//  {
//    int num1 = this.Tabs.ToArrayIndex(this.SelectedIndex);
//    if (num1 >= 0)
//    {
//      Tab tab1 = (Tab) this.Tabs[num1];
//      MultiPage page1 = this.Target;
//      if (page1 != null)
//      {
//        PageView view1 = (tab1 == null) ? null : tab1.Target;
//        if ((view1 != null) && !view1.Selected)
//        {
//          if (this._OldMultiPageIndex < 0)
//          {
//            this._OldMultiPageIndex = page1.SelectedIndex;
//          }
//          view1.Activate();
//        }
//      }
//    }
//  }

}
}
