using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Reflection;

using Rubicon;
using Rubicon.Findit.Client.Controls;
using Rubicon.Findit.Globalization.Classes;

namespace Rubicon.Findit.Client.Controls
{

public interface INavigablePage
{
  bool AllowImmediateClose { get; }
  bool CleanupOnImmediateClose { get; }
  bool NavigationRequest (string url);
  bool AutoDeleteSessionVariables { get; }
  void NavigateTo (string url, bool returnToThisPage);
  string Token{ get; }
}

public interface ITabItem
{
  string Href { get; set; }
  bool SupportsPageToken { get; set; }
  ITabItem GetNavigableItem();
}

public class TabControlBuilder: ControlBuilder
{
	public override bool AllowWhitespaceLiterals ()
	{
		return false;
	}
	
	public override Type GetChildControlType (string tag, IDictionary attribs)
	{
		string unqualifiedTag = tag;
		int posColon = tag.IndexOf (':');
		if (posColon >= 0 && posColon < tag.Length + 1)
			unqualifiedTag = tag.Substring (posColon + 1);

		if (unqualifiedTag == "TabMenu")
			return typeof (TabMenu);

    throw new ApplicationException ("Only TabMenu tags are allowed in Tab controls");
	}
}

/// <summary>
/// A tab item
/// </summary>
[ParseChildren (false, "Controls")]
[ControlBuilder (typeof (TabControlBuilder))]
public class Tab: Control, ITabItem
{
	private string _label = string.Empty;
	private string _href = string.Empty;
  private bool _supportsPageToken = false;

	public string Label
	{
		get { return _label; }
		set { _label = value; }
	}
	public string Href
	{
		get { return _href; }
		set { _href = value; }
	}
  public bool SupportsPageToken
  {
    get { return _supportsPageToken; }
    set { _supportsPageToken = value; }
  }
  ITabItem ITabItem.GetNavigableItem ()
  {
    if (Href != string.Empty)
      return this;

    if (this.Controls.Count < 1)
      return null;

    for (int i = 0; i < this.Controls.Count; ++i)
    {
      if (this.Controls[i].Visible)
      {
        return (TabMenu) this.Controls[i];
      }
    }

    return this;
  }
}

/// <summary>
/// A menu item within a tab
/// </summary>
public class TabMenu: Control, ITabItem
{
	private string _label = string.Empty;
	private string _href = string.Empty;
  private bool _supportsPageToken = false;


	public string Label
	{
		get { return _label; }
		set { _label = value; }
	}
	public string Href
	{
		get { return _href; }
		set { _href = value; }
	}

  public bool SupportsPageToken
  {
    get { return _supportsPageToken; }
    set { _supportsPageToken = value; }    
  }
  ITabItem ITabItem.GetNavigableItem()
  {
    return this;
  }
}

[MultiLingualResources ("Rubicon.Findit.Client.Controls.Globalization.TabControl")] 
[ParseChildren (true, "Tabs")]
public class TabControl: Control, IPostBackEventHandler, IResourceDispatchTarget
{
	private TabCollection _tabs = new TabCollection ();

	private string _firstImage = string.Empty;
	private string _secondImage = string.Empty;
	private string _emptyImage = string.Empty;
  private string _activeBackLinkImage = string.Empty;
  private string _inactiveBackLinkImage = string.Empty;
	private string _activeClass = string.Empty;
	private string _inactiveClass = string.Empty;
	
  private Color _backColor;
	private Color _activeColor;
	
  private int _height = -1;
	private int _activeTab = 0;
	private int _activeMenu = 0;
	private string _target = string.Empty;
	private Color _lineColor;
	private bool _seperatorLine = false;
	private bool _vertical = false;
  private bool _hasMenuBar = false;
  private string _statusMessage = string.Empty;
  private HyperLink _helpLink = null;
  private bool _serverSideNavigation = true;
  private string _backLinkUrl = string.Empty;

  // construction and disposal

  // methods and properties

	public string FirstImage
	{ 
		get { return _firstImage; }
		set { _firstImage = value; }
	}
	public string SecondImage
	{ 
		get { return _secondImage; }
		set { _secondImage = value; }
	}
	public string EmptyImage
	{ 
		get { return _emptyImage; }
		set { _emptyImage = value; }
	}
  public string ActiveBackLinkImage
  {
		get { return _activeBackLinkImage; }
		set { _activeBackLinkImage = value; }
  }
  public string InactiveBackLinkImage
  {
		get { return _inactiveBackLinkImage; }
		set { _inactiveBackLinkImage = value; }
  }
	public string ActiveClass
	{ 
		get { return _activeClass; }
		set { _activeClass = value; }
	}
	public string InactiveClass
	{ 
		get { return _inactiveClass; }
		set { _inactiveClass = value; }
	}
	public Color BackColor
	{ 
		get { return _backColor; }
		set { _backColor = value; }
	}
	public Color ActiveColor
	{
		get { return _activeColor; }
		set { _activeColor = value; }
	}
	public int Height
	{ 
		get { return _height; }
		set { _height = value; }
	}
	public int ActiveTab
	{
		get { return _activeTab; }
		set { _activeTab = value; }
	}
  public int ActiveMenu
  {
    get { return _activeMenu; }
    set { _activeMenu = value; }
  }
	public string Target
	{
		get { return _target; }
		set { _target = value; }
	}
	public Color LineColor
	{
		get { return _lineColor; }
		set { _lineColor = value; }
	}
	public bool SeperatorLine
	{
		get { return _seperatorLine; }
		set { _seperatorLine = value; }
	}
	public bool Vertical
	{
		get { return _vertical; }
		set { _vertical = value; }
	}
  public bool HasMenuBar
  {
    get { return _hasMenuBar; }
    set { _hasMenuBar = value; }
  }
  public string StatusMessage
  {
    get { return _statusMessage; }
    set { _statusMessage = value; }
  }

  public HyperLink HelpLink
  {
    get { return _helpLink; }
    set { _helpLink = value; }
  }

  /// <remarks> currently not tested with ServerSideNavigation="false" </remarks>
  public bool ServerSideNavigation
  {
    get { return _serverSideNavigation; }
    set { _serverSideNavigation = value; }
  }
  public string BackLinkUrl
  {
    get { return _backLinkUrl; }
    set { _backLinkUrl = value; }
  }

	public TabCollection Tabs
	{
		get { return _tabs; }
	}

	void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
	{
    SetSelectedItems (string.Empty);
    int colonPos = eventArgument.IndexOf (":");
    if (colonPos >= 0)
    {
      string eventName = eventArgument.Substring (0, colonPos);
      string argument = eventArgument.Substring (colonPos + 1);
      switch (eventName)
      {
        case "TabSelected":
          MoveToTab (int.Parse (argument));
          break;

        case "MenuSelected":
          MoveToMenu (int.Parse (argument));
          break;
      }
    }
	}

  private bool AllowNavigation (string url)
  {
    INavigablePage navigablePage = this.Page as INavigablePage;
    if (navigablePage == null)
      return true;

    return navigablePage.NavigationRequest (url);
  }

  public void MoveToTab (int tab)
  {
    if (tab >= _tabs.Count) throw new ArgumentOutOfRangeException ("tab");

    Tab selectedTab = _tabs[tab];

    string url = GetCompleteUrl (selectedTab, tab, 0);
    if (AllowNavigation (url))
    {
      _activeTab = tab;      
      PageUtility.Redirect (Page.Response, url);
    }
  }

  public void MoveToMenu (int menu)
  {
    Tab activeTab = (Tab) Tabs[_activeTab];
    if (menu >= activeTab.Controls.Count) throw new ArgumentOutOfRangeException ("menu");

    TabMenu selectedMenu = (TabMenu) activeTab.Controls[menu];

    string url = GetCompleteUrl (selectedMenu, _activeTab, menu);
    if (AllowNavigation (url))
    {
      _activeMenu = menu;
      PageUtility.Redirect (Page.Response, url);
    }
  }

  public string GetCurrentUrl (string defaultPage)
  {
    SetSelectedItems (defaultPage);
    TabMenu menu = (TabMenu) Tabs[_activeTab].Controls[_activeMenu];
    return GetCompleteUrl (menu, _activeTab, _activeMenu);
  }

  private string GetCompleteUrl (ITabItem tabItem, int newSelectedTabIndex, int newSelectedMenuIndex)
  {
    ITabItem navigableItem = tabItem.GetNavigableItem();
    /*if (tabItem.Href == string.Empty)
      return string.Empty;*/

    string url = PageUtility.GetPhysicalPageUrl (this.Page, navigableItem.Href);
    if (navigableItem.SupportsPageToken)
      url = PageUtility.AddPageToken (url);

    url = PageUtility.AddUrlParameter (url, "navSelectedTab", newSelectedTabIndex.ToString());
    url = PageUtility.AddUrlParameter (url, "navSelectedMenu", newSelectedMenuIndex.ToString());

    return url;
  }


  /*
    
	protected override object SaveViewState ()
	{
		return new Pair (base.SaveViewState(), _activeTab);
	}

	protected override void LoadViewState (object savedState)
	{
		
    Pair state = (Pair) savedState;
		base.LoadViewState (state.First);
		_activeTab = (int) state.Second;
    
	}

  */
  private string GetHref (int tabIndex, Tab tab)
  {
    return GetHref ("TabSelected", tabIndex, tabIndex, 0, tab);
  }
  private string GetHref (int tabIndex, int menuIndex, TabMenu tabMenu)
  {
    return GetHref ("MenuSelected", menuIndex, tabIndex, menuIndex, tabMenu);
  }

  private string GetHref (string eventName, int itemIndex, int tabIndex, int menuIndex, ITabItem tabItem)
  {
		string resultHref = null;
    string eventArgument = eventName + ":" + itemIndex.ToString();
		string script = Page.GetPostBackClientEvent (this, eventArgument);

    string url = GetCompleteUrl (tabItem, tabIndex, menuIndex);

    if (url != string.Empty)
    {
      INavigablePage navigablePage = this.Page as INavigablePage;
      bool isNavigablePage = navigablePage != null;
      bool allowImmediateClose = isNavigablePage && navigablePage.AllowImmediateClose;
      bool clientSideLink = false;

      if (   ! _serverSideNavigation 
          || ! isNavigablePage 
          || allowImmediateClose) 
		  {
        clientSideLink = true;
        if (isNavigablePage && navigablePage.CleanupOnImmediateClose)
        {
          if (tabItem.SupportsPageToken)
          {
            url = PageUtility.AddUrlParameter (url, "cleanupToken", navigablePage.Token);
          }
          else
          {
            // use server-side link because target cannot clean up session state
            clientSideLink = false;
          }
        }
      }

      if (clientSideLink)
      {
        // create a link for client-side navigation
			  if (_target != string.Empty)
				  script = "window.open('" + url + "', '" + _target + "'); " + script; 
			  else
				  resultHref = "href=\"" + url + "\"";
		  }
    }
		if (resultHref == null)
			resultHref = "href=\"javascript:" + script + "\"";

    return resultHref;
  }

  private void SetSelectedItems (string defaultPage)
  {
		string selectedTab = Page.Request.QueryString["navSelectedTab"];
		string selectedMenu = Page.Request.QueryString["navSelectedMenu"];

    if (selectedTab == null && selectedMenu == null)
    {
      if (defaultPage == string.Empty)
      {
        for (int i = 0; i < _tabs.Count; ++i)
        {
          if (_tabs[i].Visible)
          {
            _activeTab = i;
            break;
          }
        }
      }
      else
      {
        for (int tabIdx = 0; tabIdx < _tabs.Count; tabIdx++)
        {
          Tab tab = _tabs[tabIdx];

          if (string.Compare (tab.Href, defaultPage, true) == 0)
          {
            _activeTab = tabIdx;
            return;
          }

          for (int tabMenuIdx = 0; tabMenuIdx < tab.Controls.Count; tabMenuIdx++)
          {
            TabMenu menu = (TabMenu) tab.Controls[tabMenuIdx];

            if (string.Compare (menu.Href, defaultPage, true) == 0)
            {
              _activeTab = tabIdx;
              _activeMenu = tabMenuIdx;
              return;
            }
          }
        }
      }
    }
    
    if (selectedTab != null)
    {
      _activeTab = int.Parse (selectedTab);
    }
    else
    {
      for (int i = 0; i < _tabs.Count; ++i)
      {
        if (_tabs[i].Visible)
        {
          _activeTab = i;
          break;
        }
      }
    }

    if (selectedMenu != null)
    {
      _activeMenu = int.Parse (selectedMenu);
    }
    else
    {
      for (int i = 0; i < _tabs[_activeTab].Controls.Count; ++i)
      {
        if (_tabs[_activeTab].Controls[i].Visible)
        {
          _activeMenu = i;
          break;
        }
      }
    }
  }

  
  protected override void Render (HtmlTextWriter output)
	{
    SetSelectedItems (string.Empty);

    if (this.Site != null && this.Site.DesignMode)
		{
			output.WriteLine ("[TabControl - edit in HTML view]");
			return;
		}

		WebColorConverter wcc = new WebColorConverter ();
		string backColor = wcc.ConvertToString (_backColor);
		string activeColor = wcc.ConvertToString (_activeColor);
		string lineColor = wcc.ConvertToString (_lineColor);

		string heightAttrib = string.Empty;
		if (Height >= 0)
			heightAttrib = "height=\"" + _height.ToString() + "\"";
		string activeClassAttrib = string.Empty;
		if (ActiveClass != string.Empty)
			activeClassAttrib = "class=\"" + _activeClass + "\"";
		string inactiveClassAttrib = string.Empty;
		if (InactiveClass != string.Empty)
			inactiveClassAttrib = "class=\"" + _inactiveClass + "\"";
		string targetAttribute = string.Empty;
		if (Target != string.Empty)
			targetAttribute = "target=\"" + _target + "\"";

		output.WriteLine ("<table bgcolor=\"{0}\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr>",
        backColor);

    CheckActiveTab();

    int numVisibleTabs = 0;
    int activeVisibleTab = 0;
		for (int i=0; i<Tabs.Count; ++i)
		{
			Tab tab = Tabs[i];

      if (tab.Visible)
      {
        string classAttrib = inactiveClassAttrib;
		    if (i == _activeTab)
        {
			    classAttrib = activeClassAttrib;
          activeVisibleTab = numVisibleTabs;
        }


		    // write seperator cell
		    if (numVisibleTabs > 0)
			    output.WriteLine ("<td width=\"3\" bgcolor=\"{0}\"><img src=\"{1}\" width=\"3\"></td>", backColor, _emptyImage);

		    // write cell with first image
		    output.WriteLine ("<td {0} {1} align=\"left\" valign=\"top\">", classAttrib, heightAttrib);
		    output.WriteLine ("<img align=\"top\" width=\"5\" height=\"5\" border=\"0\" src=\"{0}\"/>", _firstImage);
		    output.WriteLine ("</td>");

		    // write cell with tab text
        string href = GetHref (i, tab);
		    output.WriteLine ("<td {0}> <a {0} {1}>{2}</a></td>", classAttrib, href, tab.Label);

		    // write cell with second image
		    output.WriteLine ("<td {0} {1} align=\"right\" valign=\"top\">", classAttrib, heightAttrib);
		    output.WriteLine ("<img align=\"top\" width=\"5\" height=\"5\" border=\"0\" src=\"{0}\"/>", _secondImage);
		    output.WriteLine ("</td>");

        ++ numVisibleTabs;
      }
		}


    RenderHelpLink (output);

		output.WriteLine ("</tr>");

    RenderSeperatorLine (output, lineColor, activeClassAttrib, numVisibleTabs, activeVisibleTab);
    
    RenderMenuBar (output);
		
    output.WriteLine ("</table>");
	}

  private void RenderHelpLink (HtmlTextWriter output)
  {
    if (HelpLink != null)
    {
      output.WriteLine ("<td align=\"right\">");
      HelpLink.RenderControl (output);
      output.WriteLine ("</td>");
    }
  }

  /// <summary>
  /// Check if the Active Tab (Default Tab) is Visible, if not, set the first visible Tab active
  /// </summary>
  private void CheckActiveTab()
  {
    if (!Tabs[_activeTab].Visible)
    {
      for (int i=0; i<Tabs.Count; ++i)
      {
        Tab tab = Tabs[i];    

        if (tab.Visible)
        {
          _activeTab = i;
          break;
        }
      }
    }
  }

  public void RenderSeperatorLine (HtmlTextWriter output, string lineColor, string activeClassAttrib, 
      int numVisibleTabs, int activeVisibleTab)
  {
		if (SeperatorLine)
		{
      output.WriteLine ("<tr>");
			if (activeVisibleTab != 0)
			{
				output.WriteLine ("<td colspan=\"{0}\" bgcolor=\"{1}\"><img src=\"{2}\" width=\"1\" height=\"1\" /></td>",
						activeVisibleTab * 4, lineColor, _emptyImage);
			}
			if (activeVisibleTab >= 0 && activeVisibleTab < numVisibleTabs)
			{
				output.WriteLine ("<td colspan=\"3\" {0}><img src=\"{1}\" width=\"1\" height=\"1\" /></td>",
						activeClassAttrib, _emptyImage);
			}
      if (activeVisibleTab < (numVisibleTabs - 1))
      {
			  output.WriteLine ("<td colspan=\"{0}\" bgcolor=\"{1}\"><img src=\"{2}\" width=\"1\" height=\"1\" /></td>",
					  (numVisibleTabs - activeVisibleTab - 1) * 4, lineColor, _emptyImage);
      }
			output.WriteLine ("<td width=\"100%\" bgcolor=\"{0}\"><img src=\"{1}\" width=\"1\" height=\"1\" /></td>",
					lineColor, _emptyImage);
  		output.WriteLine ("</tr>");
		}
  }
  
  
  public void RenderMenuBar (HtmlTextWriter output)
  {
    if (HasMenuBar)
    { 
      output.WriteLine ("<tr>");
      int colspan = _tabs.Count * 4;
			output.WriteLine ("<td colspan=\"{0}\" width=\"100%\" height=\"12em\" valign=\"middle\">", colspan);

      output.WriteLine ("<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" >");
      output.WriteLine ("<tr class=\"tabSubMenuBar\"><td >&nbsp;");

      CheckActiveMenu();

      Tab activeTab = Tabs[_activeTab];
      bool isFirstMenu = true;
      for (int i = 0; i < activeTab.Controls.Count; ++i)
      {
        TabMenu menu = activeTab.Controls[i] as TabMenu;
        if (menu != null)
        {
          if (menu.Visible)
          {
            if (! isFirstMenu)
              output.WriteLine (" | ");

            string menuHref = GetHref (_activeTab, i, menu);
            output.WriteLine ("<a class=\"{0}\" {1}>{2}</a> ", 
                (i==_activeMenu) ? "tabActiveSubLink" : "tabSubLink",
                menuHref, 
                menu.Label);
            isFirstMenu = false;
          }
        }
      }

      if (StatusMessage != string.Empty)
      {
        output.WriteLine ("</td><td align=\"right\" class=\"tabSubLink\">{0}", StatusMessage);
      }
      output.WriteLine ("</td></tr></table>");

      if (BackLinkUrl != string.Empty)
      {
        output.WriteLine ("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
        output.WriteLine ("<tr valign=\"middle\"><td>&nbsp;</td><td>");
        output.WriteLine ("<a class=\"tabActiveBackLink\" href=\"{0}\"><img src=\"{1}\" border=\"0\"></a>",
            BackLinkUrl,
            _activeBackLinkImage);
        output.WriteLine ("</td><td nowrap>");
        output.WriteLine ("&nbsp;<a class=\"tabActiveBackLink\" href=\"{0}\">{1}</a>&nbsp;&nbsp;",
            BackLinkUrl, ResourceManagerPool.GetResourceText (this, "BackLink"));
        output.WriteLine ("</td><td width=\"100%\">");
        output.WriteLine ("</td></tr></table>");
      }

      output.WriteLine ("</td>");
      output.WriteLine ("</tr>");
    }
  }

  private void CheckActiveMenu()
  {
    TabMenu activeMenu = Tabs[ActiveTab].Controls[ActiveMenu] as TabMenu;

    if (!activeMenu.Visible)
    {
      for (int i=0; i<Tabs[ActiveTab].Controls.Count; i++)
      {
        TabMenu menu = Tabs[ActiveTab].Controls[i] as TabMenu;

        if (menu.Visible)
        {
          ActiveMenu = i;
          break;
        }
      }
    }
  }

  public void Dispatch(IDictionary values)
  {
    foreach (DictionaryEntry entry in values)
    {
      string key = entry.Key.ToString ();
      string text = entry.Value.ToString ();        
      string[] ctrlIDs = key.Split( ':' );

      if (ctrlIDs.Length != 1)
      {
        if( ctrlIDs.Length > 2 )
        {
          //  menu handling

          TabMenu menu = GetMenuByName( GetTabByName (ctrlIDs[0]), ctrlIDs[1] );
          if (menu != null)
            menu.Label = text;
        }
        else
        {
          //  tab handling

          Tab tab = GetTabByName ( ctrlIDs[0] );
          if (tab != null)
            tab.Label = text;
        }
      }      
      else
      {
        ResourceDispatcher.SetProperty (this, key, text);
      }
    }
  }

  private TabMenu GetMenuByName (Tab tab, string menuName)
  {
    if (tab != null)
    {
      foreach (TabMenu menu in tab.Controls)
      {
        if (menu.ID == menuName)
          return menu;
      }
    }
    return null;
  }
  

  private Tab GetTabByName (string ctrlName)
  {
    foreach (Tab tab in this.Tabs)
    {
      if (tab.ID == ctrlName)
        return tab;
    }

    return null;
  }
}


public class TabCollection: IList
{
	private ArrayList _tabs = new ArrayList ();

	int IList.Add (object value)
	{
		Tab tab = value as Tab;
		if (tab == null)
			throw new ArgumentException ("argument must be an instance of class Tab", "value");
		
		return _tabs.Add (tab);
	}

	public void Add (Tab tab)
	{
		_tabs.Add (tab);
	}

	public void Clear ()
	{
		_tabs.Clear ();
	}

	bool IList.Contains (object value)
	{
		return _tabs.Contains (value);
	}

	int IList.IndexOf (object value)
	{
		return _tabs.IndexOf (value);
	}

	void IList.Insert (int index, object value)
	{
		Tab tab = value as Tab;
		if (tab == null)
			throw new ArgumentException ("argument must be an instance of class Tab", "value");
		
		_tabs.Insert (index, value);
	}

	bool IList.IsFixedSize 
	{
		get { return false; }
	}

	bool IList.IsReadOnly 
	{
		get { return false; }
	}

	void IList.Remove (object value)
	{
		_tabs.Remove (value);
	}

	public void Remove (Tab tab)
	{
		_tabs.Remove (tab);
	}

	public void RemoveAt (int index)
	{
		_tabs.RemoveAt (index);
	}

	void ICollection.CopyTo (Array array, int index)
	{
		_tabs.CopyTo (array, index);
	}

	public int Count
	{
		get 
		{ 
			return _tabs.Count; 
		}
	}

	bool ICollection.IsSynchronized
	{
		get { return _tabs.IsSynchronized; }
	}

	object ICollection.SyncRoot
	{
		get { return this; }
	}

	IEnumerator IEnumerable.GetEnumerator ()
	{
		return _tabs.GetEnumerator ();
	}

	public Tab this [int index]
	{
		get { return (Tab) _tabs[index]; }
		set { _tabs[index] = value; }
	}

	object IList.this [int index]
	{
		get { return _tabs[index]; }
		set
		{
			Tab tab = value as Tab;
			if (tab == null)
				throw new ArgumentException ("argument must be instance of class Tab", "value");

			_tabs[index] = value;
		}
	}
}
}