using System;
using System.Diagnostics;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Rubicon.Findit.Client.Controls
{

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
public class Tab: Control
{
	private string _label = string.Empty;
	private string _href = string.Empty;
  private bool _pageToken = false;

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
  public bool PageToken
  {
    get { return _pageToken; }
    set { _pageToken = value; }
  }
}

/// <summary>
/// A menu item within a tab
/// </summary>
public class TabMenu: Control
{
	private string _label = string.Empty;
	private string _href = string.Empty;
  private bool _pageToken = false;


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

  public bool PageToken
  {
    get { return _pageToken; }
    set { _pageToken = value; }    
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
	private string _target = String.Empty;
	private Color _lineColor;
	private bool _seperatorLine = false;
	private bool _vertical = false;
  private bool _hasMenuBar = false;
  private string _statusMessage = string.Empty;

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
        // TODO: raise events
        case "TabSelected":
        {
          _activeTab = Int32.Parse (argument);
          Page.Session["navTab"]= _activeTab;
          Tab selectedTab = _items[_activeTab];
          string href = selectedTab.Href;
          if (selectedTab.PageToken)
            href= AddPageToken(href);
          
            Page.RegisterStartupScript ("OpenPage", 
            "<script language='JavaScript'>location.replace ('" + href + "' );</script>");

          break;
        }
        case "MenuSelected":
          break;
      }
    }
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
  private string GetHref (string eventName, int itemIndex, Tab tab)
  {
		string resultHref = null;
    string eventArgument = eventName + ":" + itemIndex.ToString();
		string script = Page.GetPostBackClientEvent (this, eventArgument);

    string href = tab.Href;
		/*
    if (href != string.Empty)
		{
      if (tab.PageToken)
        href = AddPageToken (href);

			if (_target != string.Empty)
				script = "window.open('" + href + "', '" + _target + "'); " + script;
			else
				resultHref = "href=\"" + href + "\"";
		}
    */
		if (resultHref == null)
			resultHref = "href=\"javascript:" + script + "\"";

    return resultHref;
  }

  private string GetHref (string eventName, int itemIndex, TabMenu tabMenu)
  {
		string resultHref = null;
    string eventArgument = eventName + ":" + itemIndex.ToString();
		string script = Page.GetPostBackClientEvent (this, eventArgument);

    string href = tabMenu.Href;
		if (href != string.Empty)
		{
      if (tabMenu.PageToken)
        href = AddPageToken (href);

			if (_target != string.Empty)
				script = "window.open('" + href + "', '" + _target + "'); " + script;
			else
				resultHref = "href=\"" + href + "\"";
		}
		if (resultHref == null)
			resultHref = "href=\"javascript:" + script + "\"";

    return resultHref;
  }
  
  
  protected string AddPageToken (string href)
  {
    if (href.IndexOf ("?") == -1)
      return href += "?pageToken=" + PageUtility.GetUniqueToken();
    else
      return href += "&pageToken=" + PageUtility.GetUniqueToken();
  }

	protected override void Render (HtmlTextWriter output)
	{
		object o= Page.Session["navTab"];
    if (o != null)
      _activeTab = (int) o;
    else
      _activeTab = 0;
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

      string href = GetHref ("TabSelected", i, tab);

			// write seperator cell
			if (i > 0)
				output.WriteLine ("<td width=\"3\" bgcolor=\"{0}\"><img src=\"{1}\" width=\"3\"></td>", backColor, _emptyImage);

			// write cell with first image
			output.WriteLine ("<td {0} {1} align=\"left\" valign=\"top\">", classAttrib, heightAttrib);
			output.WriteLine ("<img align=\"top\" width=\"5\" height=\"5\" border=\"0\" src=\"{0}\"/>", _firstImage);
			output.WriteLine ("</td>");

			// write cell with tab text
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

          // string menuHref = string.Format ("href=\"{0}\" target=\"{1}\"", menu.Href, _target);
          string menuHref = GetHref ("MenuSelected", i, menu);
          output.WriteLine ("<a class=\"tabSubLink\" {0}>{1}</a> ", menuHref, menu.Label);
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