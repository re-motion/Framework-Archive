using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:WebTabStrip runat=server></{0}:WebTabStrip>")]
public class WebTabStrip : WebControl, IPostBackEventHandler
{
  private WebTab _selectedTab;

  [Description("TabStripSelectedIndexChange")]
  public event EventHandler SelectedIndexChange;

  private int _CachedSelectedIndex;
  private WebTabCollection _Items;
  private int _OldMultiPageIndex;
  private Style _SepDefaultStyle;
  private Style _SepHoverStyle;
  private Style _SepSelectedStyle;
  private Style _TabDefaultStyle;
  private Style _TabHoverStyle;
  private Style _TabSelectedStyle;
  private const int NoSelection = -1;
  private const int NotSet = -2;
  public const string TabSeparatorTagName = "TabSeparator";
  public const string TabStripTagName = "TabStrip";
  public const string TabTagName = "Tab";
  public const string TagNamespace = "TSNS";


  /// <summary> Initalizes a new instance. </summary>
  public WebTabStrip (Control ownerControl)
  {
    _Items = new WebTabCollection (ownerControl);
    _Items.SetParent (this);
    this._CachedSelectedIndex = -2;
    this._OldMultiPageIndex = -1;
    this._TabDefaultStyle = new Style();
    this._TabHoverStyle = new Style();
    this._TabSelectedStyle = new Style();
    this._SepDefaultStyle = new Style();
    this._SepHoverStyle = new Style();
    this._SepSelectedStyle = new Style();
  }

  /// <summary> Initalizes a new instance. </summary>
  public WebTabStrip()
    :this (null)
  {
  }

  protected override ControlCollection CreateControlCollection()
  {
    return new EmptyControlCollection(this);
  }

  protected override void LoadViewState(object savedState)
  {
    if (savedState != null)
    {
      object[] objArray1 = (object[]) savedState;
      base.LoadViewState(objArray1[0]);
      //this.Items.LoadViewState(objArray1[1]);
      //            ((IStateManager) this.TabDefaultStyle).LoadViewState(objArray1[2]);
      //            ((IStateManager) this.TabHoverStyle).LoadViewState(objArray1[3]);
      //            ((IStateManager) this.TabSelectedStyle).LoadViewState(objArray1[4]);
      //            ((IStateManager) this.SepDefaultStyle).LoadViewState(objArray1[5]);
      //            ((IStateManager) this.SepHoverStyle).LoadViewState(objArray1[6]);
      //            ((IStateManager) this.SepSelectedStyle).LoadViewState(objArray1[7]);
    }
  }
  protected override void OnInit(EventArgs e)
  {
    if (this._CachedSelectedIndex != -2)
    {
      this.SelectedIndex = this._CachedSelectedIndex;
      this._CachedSelectedIndex = -2;
    }
    base.OnInit(e);
  }

  protected override void OnLoad(EventArgs e)
  {
    if ((this.Page != null) && !this.Page.IsPostBack)
    {
//      if ((this.TargetID != string.Empty) && (this.Target == null))
//      {
//        throw new Exception(BaseRichControl.GetStringResource("TabStripInvalidTargetID"));
//      }
//      foreach (Tab1 in this.Items)
//      {
//        if (item1 is Tab)
//        {
//          Tab tab1 = (Tab)1;
//          PageView view1 = tab1.Target;
//          if ((tab1.TargetID != string.Empty) && (view1 == null))
//          {
//            throw new Exception(BaseRichControl.GetStringResource("TabInvalidTargetID"));
//          }
//        }
//      }
//      this.SetTargetSelectedIndex();
    }
    base.OnLoad(e);
  }

  protected override void OnPreRender(EventArgs e)
  {
//    this.HelperData = this.SelectedIndex.ToString();
    base.OnPreRender(e);
  }

  protected virtual void OnSelectedIndexChange(EventArgs e)
  {
    if (this.SelectedIndexChange != null)
    {
      this.SelectedIndexChange(this, e);
    }
  }

//  protected override bool ProcessData(string szData)
//  {
//    try
//    {
//      int num1 = Convert.ToInt32(szData);
//      if (this.SelectedIndex != num1)
//      {
//        this.SelectedIndex = num1;
//        return true;
//      }
//    }
//    catch
//    {
//    }
//    return false;
//  }

  void IPostBackEventHandler.RaisePostBackEvent (string argument)
  {
//    this.OnSelectedIndexChange(new EventArgs());
//    MultiPage page1 = this.Target;
//    if (page1 != null)
//    {
//      if (this._OldMultiPageIndex < 0)
//      {
//        this.SetTargetSelectedIndex();
//      }
//      if ((this._OldMultiPageIndex >= 0) && (page1.SelectedIndex != this._OldMultiPageIndex))
//      {
//        page1.FireSelectedIndexChangeEvent();
//      }
//    }
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    foreach (WebTab tab in this.Items)
    {
      if (tab.IsSelected)
        writer.AddAttribute (HtmlTextWriterAttribute.Class, null);  
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, null);  
      writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string postBackLink = null;
      if (! tab.IsSeparator)
        postBackLink = Page.GetPostBackClientHyperlink (this, tab.TabID);
      tab.RenderContents(writer, postBackLink);

      writer.RenderEndTag();
    }
  }

//  protected override void RenderDesignerPath(HtmlTextWriter writer)
//  {
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
//    this.AddAttributesToRender(writer);
//    writer.RenderBeginTag(HtmlTextWriterTag.Table);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
//    }
//    base.RenderDesignerPath(writer);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderEndTag();
//    }
//    writer.RenderEndTag();
//  }
//
//  protected override void RenderDownLevelPath(HtmlTextWriter writer)
//  {
//    string[] textArray1 = new string[5] { "<script language=\"javascript\">", this.ClientHelperID, ".value=", this.SelectedIndex.ToString(), ";</script>" } ;
//    writer.WriteLine(string.Concat(textArray1));
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
//    writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
//    this.AddAttributesToRender(writer);
//    writer.RenderBeginTag(HtmlTextWriterTag.Table);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
//    }
//    base.RenderDownLevelPath(writer);
//    if (this.Orientation == Orientation.Horizontal)
//    {
//      writer.RenderEndTag();
//    }
//    writer.RenderEndTag();
//  }

//  protected override void RenderUpLevelPath(HtmlTextWriter writer)
//  {
//    writer.Write("<?XML:NAMESPACE PREFIX=\"TSNS\" /><?IMPORT NAMESPACE=\"TSNS\" IMPLEMENTATION=\"" + base.AddPathToFilename("tabstrip.htc") + "\" />");
//    writer.WriteLine();
//    this.AddAttributesToRender(writer);
//    writer.AddAttribute("selectedIndex", this.SelectedIndex.ToString());
//    if (this.Orientation == Orientation.Vertical)
//    {
//      writer.AddAttribute("orientation", "vertical");
//    }
//    if (this.TargetID != string.Empty)
//    {
//      writer.AddAttribute("targetID", this.Target.ClientID);
//    }
//    if (this.SepDefaultImageUrl != string.Empty)
//    {
//      writer.AddAttribute("sepDefaultImageUrl", this.SepDefaultImageUrl);
//    }
//    if (this.SepHoverImageUrl != string.Empty)
//    {
//      writer.AddAttribute("sepHoverImageUrl", this.SepHoverImageUrl);
//    }
//    if (this.SepSelectedImageUrl != string.Empty)
//    {
//      writer.AddAttribute("sepSelectedImageUrl", this.SepSelectedImageUrl);
//    }
//    string text1 = this.TabDefaultStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("tabDefaultStyle", text1);
//    }
//    text1 = this.TabHoverStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("tabHoverStyle", text1);
//    }
//    text1 = this.TabSelectedStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("tabSelectedStyle", text1);
//    }
//    text1 = this.SepDefaultStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("sepDefaultStyle", text1);
//    }
//    text1 = this.SepHoverStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("sepHoverStyle", text1);
//    }
//    text1 = this.SepSelectedStyle.CssText;
//    if (text1 != string.Empty)
//    {
//      writer.AddAttribute("sepSelectedStyle", text1);
//    }
//    if (this.Page != null)
//    {
//      string text2 = this.ClientHelperID + ".value=event.index";
//      if (this.AutoPostBack)
//      {
//        text2 = text2 + ";if (getAttribute('_submitting') != 'true'){setAttribute('_submitting','true');try{" + this.Page.GetPostBackEventReference(this, string.Empty) + ";}catch(e){setAttribute('_submitting','false');}}";
//      }
//      writer.AddAttribute("onSelectedIndexChange", "JScript:" + text2);
//      writer.AddAttribute("onwcready", "JScript:try{" + this.ClientHelperID + ".value=selectedIndex}catch(e){}");
//    }
//    writer.RenderBeginTag("TSNS:TabStrip");
//    writer.WriteLine();
//    base.RenderUpLevelPath(writer);
//    writer.RenderEndTag();
//  }
//
  internal void ResetSelectedIndex()
  {
    if (this.ViewState["SelectedIndex"] != null)
    {
      this.ViewState.Remove("SelectedIndex");
    }
  }

  protected override object SaveViewState()
  {
    object[] viewState = new object[1];
    viewState[0] = base.SaveViewState();
//
//    object[] objArray2 = new object[8] { base.SaveViewState(), this.Items.SaveViewState(), ((IStateManager) this.TabDefaultStyle).SaveViewState(), ((IStateManager) this.TabHoverStyle).SaveViewState(), ((IStateManager) this.TabSelectedStyle).SaveViewState(), ((IStateManager) this.SepDefaultStyle).SaveViewState(), ((IStateManager) this.SepHoverStyle).SaveViewState(), ((IStateManager) this.SepSelectedStyle).SaveViewState() } ;
//    object[] objArray1 = objArray2;
//    objArray2 = objArray1;
//    for (int num1 = 0; num1 < objArray2.Length; num1++)
//    {
//      object obj1 = objArray2[num1];
//      if (obj1 != null)
//      {
//        return objArray1;
//      }
//    }
//    return null;
    return viewState;
  }

  private void SetTargetSelectedIndex()
  {
//    int num1 = this.Items.ToArrayIndex(this.SelectedIndex);
//    if (num1 >= 0)
//    {
//      Tab tab1 = (Tab) this.Items[num1];
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
  }

  //protected override void TrackViewState()
  //{
  //      base.TrackViewState();
  //      ((IStateManager) this.TabDefaultStyle).TrackViewState();
  //      ((IStateManager) this.TabHoverStyle).TrackViewState();
  //      ((IStateManager) this.TabSelectedStyle).TrackViewState();
  //      ((IStateManager) this.SepDefaultStyle).TrackViewState();
  //      ((IStateManager) this.SepHoverStyle).TrackViewState();
  //      ((IStateManager) this.SepSelectedStyle).TrackViewState();
  //      this.Items.TrackViewState();
  //}

  [Category("Behavior")]
  [DefaultValue(false)]
  [Description("AutoPostBack")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public bool AutoPostBack
  {
    get
    {
      object obj1 = this.ViewState["AutoPostBack"];
      if (obj1 != null)
      {
        return (bool) obj1;
      }
      return false;
    }
    set
    {
      this.ViewState["AutoPostBack"] = value;
    }
  }

  [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
  [DefaultValue((string) null)]
  [MergableProperty(false)]
  [Category("Data")]
  [Description("TabStripItems")]
  public virtual WebTabCollection Items
  {
    get
    {
      return this._Items;
    }
  }

//  protected override bool NeedHelper
//  {
//    get
//    {
//      return true;
//    }
//  }

//  [PersistenceMode(PersistenceMode.Attribute)]
//  [Category("Appearance")]
//  [Description("TabStripOrientation")]
//  [DefaultValue(0)]
//  public Orientation Orientation
//  {
//    get
//    {
//      object obj1 = this.ViewState["Orientation"];
//      if (obj1 != null)
//      {
//        return (Orientation) obj1;
//      }
//      return Orientation.Horizontal;
//    }
//    set
//    {
//      this.ViewState["Orientation"] = value;
//    }
//  }

  [Description("TabStripSelectedIndex")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Category("Behavior")]
  [DefaultValue(0)]
  public int SelectedIndex
  {
    get
    {
//      int num1 = this.Items.NumTabs;
//      if (num1 != 0)
//      {
//        int num2 = 0;
//        object obj1 = this.ViewState["SelectedIndex"];
//        if (obj1 != null)
//        {
//          num2 = (int) obj1;
//        }
//        else if (this._CachedSelectedIndex == -1)
//        {
//          return -1;
//        }
//        if ((num2 >= 0) && (num2 < num1))
//        {
//          return num2;
//        }
//      }
      return -1;
    }
    set
    {
//      if ((this.Items.NumTabs == 0) && (value > -2))
//      {
//        this._CachedSelectedIndex = value;
//      }
//      else
//      {
//        if ((value <= -2) || (value >= this.Items.NumTabs))
//        {
//          throw new ArgumentOutOfRangeException();
//        }
//        this.ViewState["SelectedIndex"] = value;
//        if (value >= 0)
//        {
//          this._OldMultiPageIndex = -1;
//          this.SetTargetSelectedIndex();
//        }
//      }
    }
  }

  [Category("Separator Defaults")]
  [DefaultValue("")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Description("SepDefaultImageUrl")]
  public string SepDefaultImageUrl
  {
    get
    {
      object obj1 = this.ViewState["SepDefaultImageUrl"];
      if (obj1 != null)
      {
        return (string) obj1;
      }
      return string.Empty;
    }
    set
    {
      this.ViewState["SepDefaultImageUrl"] = value;
    }
  }

  [Description("SepDefaultStyle")]
  [Category("Separator Defaults")]
  [DefaultValue(typeof(Style), "")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public Style SepDefaultStyle
  {
    get
    {
      return this._SepDefaultStyle;
    }
    set
    {
      this._SepDefaultStyle = value;
    }
  }

  [DefaultValue("")]
  [Category("Separator Defaults")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Description("SepHoverImageUrl")]
  public string SepHoverImageUrl
  {
    get
    {
      object obj1 = this.ViewState["SepHoverImageUrl"];
      if (obj1 != null)
      {
        return (string) obj1;
      }
      return string.Empty;
    }
    set
    {
      this.ViewState["SepHoverImageUrl"] = value;
    }
  }

  [DefaultValue(typeof(Style), "")]
  [Category("Separator Defaults")]
  [Description("SepHoverStyle")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public Style SepHoverStyle
  {
    get
    {
      return this._SepHoverStyle;
    }
    set
    {
      this._SepHoverStyle = value;
    }
  }

  [Category("Separator Defaults")]
  [DefaultValue("")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Description("SepSelectedImageUrl")]
  public string SepSelectedImageUrl
  {
    get
    {
      object obj1 = this.ViewState["SepSelectedImageUrl"];
      if (obj1 != null)
      {
        return (string) obj1;
      }
      return string.Empty;
    }
    set
    {
      this.ViewState["SepSelectedImageUrl"] = value;
    }
  }

  [Category("Separator Defaults")]
  [Description("SepSelectedStyle")]
  [DefaultValue(typeof(Style), "")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public Style SepSelectedStyle
  {
    get
    {
      return this._SepSelectedStyle;
    }
    set
    {
      this._SepSelectedStyle = value;
    }
  }

  [Category("Tab Defaults")]
  [Description("TabDefaultStyle")]
  [DefaultValue(typeof(Style), "")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public Style TabDefaultStyle
  {
    get
    {
      return this._TabDefaultStyle;
    }
    set
    {
      this._TabDefaultStyle = value;
    }
  }

  [Description("TabHoverStyle")]
  [Category("Tab Defaults")]
  [DefaultValue(typeof(Style), "")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public Style TabHoverStyle
  {
    get
    {
      return this._TabHoverStyle;
    }
    set
    {
      this._TabHoverStyle = value;
    }
  }

  [Description("TabSelectedStyle")]
  [Category("Tab Defaults")]
  [DefaultValue(typeof(Style), "")]
  [PersistenceMode(PersistenceMode.Attribute)]
  public Style TabSelectedStyle
  {
    get
    {
      return this._TabSelectedStyle;
    }
    set
    {
      this._TabSelectedStyle = value;
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  [CLSCompliant (false)]
  public MultiPage Target
  {
    get
    {
      string text1 = this.TargetID;
      if (text1 != string.Empty)
      {
        Control control1 = null;
        Control control2 = this.NamingContainer;
        Control control3 = this.Page;
        if (control2 != null)
        {
          control1 = control2.FindControl(text1);
        }
        if ((control1 == null) && (control3 != null))
        {
          control1 = control3.FindControl(text1);
        }
        if ((control1 != null) && (control1 is MultiPage))
        {
          return (MultiPage) control1;
        }
      }
      return null;
    }
  }

  [Category("Behavior")]
  [DefaultValue("")]
  [PersistenceMode(PersistenceMode.Attribute)]
  [Description("TabStripTargetID")]
  public string TargetID
  {
    get
    {
      object obj1 = this.ViewState["TargetID"];
      if (obj1 != null)
      {
        return (string) obj1;
      }
      return string.Empty;
    }
    set
    {
      this.ViewState["TargetID"] = value;
    }
  }
  
  /// <summary> Sets the selected tab. </summary>
  internal void SetSelectedTab (WebTab tab)
  {
    if (tab != null && tab.TabStrip != this)
      throw new InvalidOperationException ("Only tabs that are part of this tab strip can be selected.");
    if (_selectedTab != tab)
    {
      if ((_selectedTab != null) && _selectedTab.IsSelected)
        _selectedTab.SetSelected (false);
      _selectedTab = tab;
      if ((_selectedTab != null) && ! _selectedTab.IsSelected)
        _selectedTab.SetSelected (true);
    }
  }

  /// <summary> Gets the currently selected tab. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public WebTab SelectedTab
  {
    get { return _selectedTab; }
  }
}

}
