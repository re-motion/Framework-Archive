using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Rubicon.Findit.Client.Controls
{

public interface INavigablePage
{
  bool AllowImmediateClose {get; }
  bool NavigationRequest (string url);
}

public interface ITabItem
{
  string Href { get; set; }
  bool RequiresPageToken { get; set; }
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
  private bool _requiresPageToken = false;

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
  public bool RequiresPageToken
  {
    get { return _requiresPageToken; }
    set { _requiresPageToken = value; }
  }
  ITabItem ITabItem.GetNavigableItem ()
  {
    if (Href != string.Empty)
      return this;

    if (this.Controls.Count < 1)
      return null;

    TabMenu firstMenu = this.Controls[0] as TabMenu;
    if (firstMenu != null)
      return firstMenu;
    else 
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
  private bool _requiresPageToken = false;


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

  public bool RequiresPageToken
  {
    get { return _requiresPageToken; }
    set { _requiresPageToken = value; }    
  }
  ITabItem ITabItem.GetNavigableItem()
  {
    return this;
  }
}

[ParseChildren (true, "Items")]
public class TabControl: Control, IPostBackEventHandler
{
	private TabCollection _items = new TabCollection ();

	private string _firstImage = String.Empty;
	private string _secondImage = String.Empty;
	private string _emptyImage = String.Empty;
	private string _activeClass = String.Empty;
	private string _inactiveClass = String.Empty;
	
  private Color _backColor;
	private Color _activeColor;
	
  private int _height = -1;
	private int _activeTab = 0;
	private int _activeMenu = 0;
	private string _target = String.Empty;
	private Color _lineColor;
	private bool _seperatorLine = false;
	private bool _vertical = false;
  private bool _hasMenuBar = false;
  private string _statusMessage = string.Empty;
  private bool _serverSideNavigation = false;

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
  public bool ServerSideNavigation
  {
    get { return _serverSideNavigation; }
    set { _serverSideNavigation = value; }
  }

	public TabCollection Items
	{
		get { return _items; }
	}

	void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
	{
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
    if (tab >= _items.Count) throw new ArgumentOutOfRangeException ("tab");

    _activeTab = tab;
    Tab selectedTab = _items[_activeTab];

    string url = GetCompleteUrl (selectedTab, tab, 0);
    if (AllowNavigation (url))
      Page.Response.Redirect (url);
  }

  public void MoveToMenu (int menu)
  {
    Tab activeTab = (Tab) Items[_activeTab];
    if (menu >= activeTab.Controls.Count) throw new ArgumentOutOfRangeException ("menu");

    _activeMenu = menu;
    TabMenu selectedMenu = (TabMenu) activeTab.Controls[menu];

    string url = GetCompleteUrl (selectedMenu, _activeTab, menu);
    if (AllowNavigation (url))
      Page.Response.Redirect (url);
  }

  private string GetCompleteUrl (ITabItem tabItem, int newSelectedTabIndex, int newSelectedMenuIndex)
  {
    ITabItem navigableItem = tabItem.GetNavigableItem();
    /*if (tabItem.Href == string.Empty)
      return string.Empty;*/

    string url = PageUtility.GetPhysicalPageUrl (this.Page, navigableItem.Href);
    if (navigableItem.RequiresPageToken)
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

    string href = GetCompleteUrl (tabItem, tabIndex, menuIndex);

    if (href != string.Empty)
    {
      INavigablePage navigablePage = this.Page as INavigablePage;
      bool isNavigablePage = navigablePage != null;
      bool allowImmediateClose = isNavigablePage && navigablePage.AllowImmediateClose;

      if (   ! _serverSideNavigation 
          || ! isNavigablePage 
          || allowImmediateClose)
		  {
			  if (_target != string.Empty)
				  script = "window.open('" + href + "', '" + _target + "'); " + script;
			  else
				  resultHref = "href=\"" + href + "\"";
		  }
    }
		if (resultHref == null)
			resultHref = "href=\"javascript:" + script + "\"";

    return resultHref;
  }

  


	protected override void Render (HtmlTextWriter output)
	{
		string selectedTab = Page.Request.QueryString["navSelectedTab"];
    if (selectedTab != null)
      _activeTab = int.Parse (selectedTab);
    else
      _activeTab = 0;

		string selectedMenu = Page.Request.QueryString["navSelectedMenu"];
    if (selectedMenu != null)
      _activeMenu = int.Parse (selectedMenu);
    else
      _activeMenu = 0;

    if (this.Site != null && this.Site.DesignMode)
		{
			output.WriteLine ("[TabControl - edit in HTML view]");
			return;
		}

		WebColorConverter wcc = new WebColorConverter ();
		string backColor = wcc.ConvertToString (_backColor);
		string activeColor = wcc.ConvertToString (_activeColor);
		string lineColor = wcc.ConvertToString (_lineColor);

		string heightAttrib = String.Empty;
		if (Height >= 0)
			heightAttrib = "height=\"" + _height.ToString() + "\"";
		string activeClassAttrib = String.Empty;
		if (ActiveClass != String.Empty)
			activeClassAttrib = "class=\"" + _activeClass + "\"";
		string inactiveClassAttrib = String.Empty;
		if (InactiveClass != String.Empty)
			inactiveClassAttrib = "class=\"" + _inactiveClass + "\"";
		string targetAttribute = String.Empty;
		if (Target != String.Empty)
			targetAttribute = "target=\"" + _target + "\"";

		output.WriteLine ("<table bgcolor=\"{0}\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr>",
        backColor);


		for (int i=0; i<Items.Count; ++i)
		{
			Tab tab = Items[i];

			string classAttrib = inactiveClassAttrib;
			if (i == _activeTab)
				classAttrib = activeClassAttrib;


			// write seperator cell
			if (i > 0)
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
		}
		output.WriteLine ("</tr>");

    RenderSeperatorLine (output, lineColor, activeClassAttrib);
    
    RenderMenuBar (output);
		
    output.WriteLine ("</table>");
	}

  public void RenderSeperatorLine (HtmlTextWriter output, string lineColor, string activeClassAttrib)
  {
		if (SeperatorLine)
		{
      output.WriteLine ("<tr>");
			if (_activeTab != 0)
			{
				output.WriteLine ("<td colspan=\"{0}\" bgcolor=\"{1}\"><img src=\"{2}\" width=\"1\" height=\"1\" /></td>",
						_activeTab * 4, lineColor, _emptyImage);
			}
			if (_activeTab >= 0 && _activeTab < _items.Count)
			{
				output.WriteLine ("<td colspan=\"3\" {0} <img src=\"{1}\" width=\"1\" height=\"1\" /></td>",
						activeClassAttrib, _emptyImage);
			}
      if (_activeTab < (_items.Count - 1))
      {
			  output.WriteLine ("<td colspan=\"{0}\" bgcolor=\"{1}\"><img src=\"{2}\" width=\"1\" height=\"1\" /></td>",
					  (_items.Count - _activeTab - 1) * 4, lineColor, _emptyImage);
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
      int colspan = _items.Count * 4;
			output.WriteLine ("<td colspan=\"{0}\" width=\"100%\" height=\"12em\" valign=\"center\">", colspan);

      output.WriteLine ("<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" >");
      output.WriteLine ("<tr class=\"tabSubLink\"><td nowrap>&nbsp;");
  
      Tab activeTab = Items[_activeTab];
      bool isFirstMenu = true;
      for (int i = 0; i < activeTab.Controls.Count; ++i)
      {
        TabMenu menu = activeTab.Controls[i] as TabMenu;
        if (menu != null)
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
      if (StatusMessage != string.Empty)
      {
        output.WriteLine ("</td><td width=\"100%\" align=\"right\">{0}", StatusMessage);
      }
      output.WriteLine ("</td></tr></table>");
      output.WriteLine ("</td>");
      output.WriteLine ("</tr>");
    }
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
		
		Debug.WriteLine ("IList.Add");
		return _tabs.Add (tab);
	}

	public void Add (Tab tab)
	{
		Debug.WriteLine ("Add");
		_tabs.Add (tab);
	}

	public void Clear ()
	{
		Debug.WriteLine ("Clear");
		_tabs.Clear ();
	}

	bool IList.Contains (object value)
	{
		Debug.WriteLine ("IList.Contains");
		return _tabs.Contains (value);
	}

	int IList.IndexOf (object value)
	{
		Debug.WriteLine ("IList.IndexOf");
		return _tabs.IndexOf (value);
	}

	void IList.Insert (int index, object value)
	{
		Debug.WriteLine ("IListInsert");
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
		Debug.WriteLine ("IList.Remove");
		_tabs.Remove (value);
	}

	public void Remove (Tab tab)
	{
		Debug.WriteLine ("Remove");
		_tabs.Remove (tab);
	}

	public void RemoveAt (int index)
	{
		Debug.WriteLine ("RemoveAt");
		_tabs.RemoveAt (index);
	}

	void ICollection.CopyTo (Array array, int index)
	{
		Debug.WriteLine ("CopyTo");
		_tabs.CopyTo (array, index);
	}

	public int Count
	{
		get 
		{ 
			Debug.WriteLine ("Count");
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
		Debug.WriteLine ("GetEnumerator");
		return _tabs.GetEnumerator ();
	}

	public Tab this [int index]
	{
		get 
		{ 
			Debug.WriteLine ("get []");
			return (Tab) _tabs[index]; 
		}
		set 
		{ 
			Debug.WriteLine ("set []");
			_tabs[index] = value; 
		}
	}

	object IList.this [int index]
	{
		get 
		{ 
			Debug.WriteLine ("IList get []");
			return _tabs[index]; 
		}

		set
		{
			Debug.WriteLine ("IList set []");
			Tab tab = value as Tab;
			if (tab == null)
				throw new ArgumentException ("argument must be instance of class Tab", "value");

			_tabs[index] = value;
		}
	}
}
}