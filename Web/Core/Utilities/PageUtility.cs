using System;
using System.Collections;
using System.Web.UI;
using System.Web;

using System.Diagnostics;

namespace Rubicon.Findit.Client.Controls
{
/// <summary>
/// Utility class for pages.
/// </summary>
public class PageUtility
{
  public const string c_supressNavParam = "SupressNav";
  public const string c_supressNavValue = "true";

  /// <summary>
  /// Used by method "CallPage" to indicate whether the navigation bar shall be shown.
  /// If "UseDefault" the display status of the navigation bar is retrieved from the calling page's URL.
  /// </summary>
  public enum ShowNavigationBar 
  {
    Show, 
    Hide, 
    UseDefault
  };

	public PageUtility()
	{
	}

  public static object GetSessionValue (Page page, string key, bool required)
  {
    object o = page.Session[GetUniqueKey (page, key)];
    
    if (required && o == null)
    {
      if (page.Session.IsNewSession)
        throw new SessionTimeoutException ();
      else
        throw new SessionVariableNotFoundException (key);
    }

    return o;
  }

  public static void SetSessionValue (Page page, string key, object sessionValue)
  {
    page.Session[GetUniqueKey (page, key)] = sessionValue;
  }

  public static void ClearSessionValue (Page page, string key)
  {
    page.Session[GetUniqueKey (page, key)] = null;
  }

  public static object GetSessionValue (Page page, string token, string key, bool required)
  {
    object o = page.Session[GetUniqueKey (token, key)];
    
    if (required && o == null)
    {
      if (page.Session.IsNewSession)
        throw new SessionTimeoutException ();
      else
        throw new SessionVariableNotFoundException (key);
    }

    return o;
  }

  public static void SetSessionValue (Page page, string token, string key, object sessionValue)
  {
    page.Session[GetUniqueKey (token, key)] = sessionValue;
  }

  public static void ClearSessionValue (Page page, string token, string key)
  {
    page.Session[GetUniqueKey (token, key)] = null;
  }

  /// <summary>
  /// converts a given key value to a page specific value
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public static string GetUniqueKey (Page page, string key)
  {
    string pageToken = (string) page.Request.Params["pageToken"];
    
    if (pageToken == null)
      throw new NoPageTokenException (page);

    return pageToken + ":" + key;
  }

  public static string GetUniqueKey (string token, string key)
  {
    return token + ":" + key;
  }

  /// <summary>
  /// returns a formatted GUID string
  /// </summary>
  /// <returns></returns>
  public static string GetUniqueToken ()
  {
    return Guid.NewGuid ().ToString ("N") ;
  }

  /// <summary>
  /// Returns the physical URL for the current page.
  /// </summary>  
  /// <remarks>
  /// For cookieless sessions, the session ID is inserted into the URL.
  /// </remarks>
  public static string GetPhysicalPageUrl (Page page)
  {
    string returnUrl;

    Uri pageUrl = page.Request.Url;

    // WORKAROUND: With cookieless navigation activated the ASP.NET engine 
    // removes cookie information from the URL => manually add cookie information to URL
    if (page.Session.IsCookieless)
    { 
      string tempUrl = pageUrl.PathAndQuery;
      string appPath = page.Request.ApplicationPath;
      int appPathPositionEnd = appPath.Length;

      returnUrl = 
            tempUrl.Substring (0, appPathPositionEnd) 
          + "/(" + page.Session.SessionID + ")" 
          + tempUrl.Substring (appPathPositionEnd);
    }
    else
    {
      returnUrl = pageUrl.PathAndQuery;
    }
    
    return returnUrl;  
  }

  /// <summary>
  /// Returns the physical URL for a specified page.
  /// </summary>  
  /// <remarks>
  /// For cookieless sessions, the session ID is inserted into the URL.
  /// </remarks>
  public static string GetPhysicalPageUrl (Page page, string relativeUrl)
  {
    if (relativeUrl == null || relativeUrl == string.Empty)
      throw new ArgumentException ("Argument must contain a valid relative URL", "relativeURL");

    string appPath = page.Request.ApplicationPath;

    if (!appPath.EndsWith ("/"))
      appPath += "/";

    if (page.Session.IsCookieless)
      return appPath + "(" + page.Session.SessionID + ")/" + relativeUrl;
    else
      return appPath + relativeUrl;
  }

  public static string GetPhysicalHttpPageUrl (Page page, string relativeUrl)
  {
    return RemoveHttps (GetPhysicalPageUrl (page, relativeUrl));
  }

  public static string RemoveHttps (string url)
  {
    if (url.ToLower().StartsWith ("https://"))
      return "http://" + url.Substring ("https://".Length);
    else
      return url;
  }

/*  private static string InternalGetPhysicalPageUrl (Page page, string relativeUrl)
  { 
    if (page.Session.IsCookieless)
      return page.Request.ApplicationPath + "/(" + page.Session.SessionID + ")/" + relativeUrl;
    else
      return page.Request.ApplicationPath + "/" + relativeUrl;

  
    string returnUrl;

    //Uri pageUrl = new Uri (page.Request.Url, relativeUrl);
    Uri pageUrl = new Uri (page.Request.ApplicationPath + relativeUrl);

    // WORKAROUND: With cookieless navigation activated the ASP.NET engine 
    // removes cookie information from the URL => manually add cookie information to URL
    if (page.Session.IsCookieless)
    { 
      string tempUrl = pageUrl.PathAndQuery;
      string appPath = page.Request.ApplicationPath;
      int appPathPositionEnd = appPath.Length;

      returnUrl = 
            tempUrl.Substring (0, appPathPositionEnd) 
          + "/(" + page.Session.SessionID + ")" 
          + tempUrl.Substring (appPathPositionEnd);
    }
    else
    {
      returnUrl = pageUrl.PathAndQuery;
    }
    
    return returnUrl;
  }
    */

  public static string AddCleanupToken (Page page, string url)
  {
    return AddUrlParameter (url, "cleanupToken", GetToken(page));
  }

  public static string AddParentToken (Page page, string url)
  {
    return AddUrlParameter (url, "parentToken", GetToken(page));
  }

  public static string AddActiveTabParameters (Page sourcePage, string url)
  {
		string selectedTab = sourcePage.Request.QueryString["navSelectedTab"];
    if (selectedTab != null)
      url = AddUrlParameter (url, "navSelectedTab", selectedTab);

		string selectedMenu = sourcePage.Request.QueryString["navSelectedMenu"];
    if (selectedMenu != null)
      url = AddUrlParameter (url, "navSelectedMenu", selectedMenu);
    return url;
  }
  
  public static string AddPageToken (string url)
  {
    return AddUrlParameter (url, "pageToken", GetUniqueToken());
  }

  public static string AddUrlParameter (string url, string name, string value)
  {
    string parameterSeperator = (url.IndexOf ("?") == -1) ? "?" : "&";
    return url + parameterSeperator + name + "=" 
        + HttpUtility.UrlEncode(value, HttpContext.Current.Response.ContentEncoding);

    //System.Text.Encoding encoding = System.Text.Encoding.GetEncoding ("ISO-8859-1");
    //return url + parameterSeperator + name + "=" + value;
  }

  public static string GetToken (Page page)
  {
    return page.Request.QueryString["pageToken"];
  }

  public static string GetParentToken (Page page)
  {
    return page.Request.QueryString["parentToken"];
  }

  public static void Redirect (HttpResponse response, string destinationUrl)
  {
	  destinationUrl = response.ApplyAppPathModifier (destinationUrl);
  	response.Clear();

		response.StatusCode = 302;
    response.AppendHeader ("location", destinationUrl);
		response.Write("<html><head><title>Object moved</title></head><body>\r\n");
		response.Write("<h2>Object moved to <a href='" + destinationUrl + "'>here</a>.</h2>\r\n");
		response.Write("</body></html>\r\n");

    response.End ();
	}

  /// <summary>
  /// Redirects to the page identified by the given URL 
  /// </summary>
  /// <param name="destinationUrl">URL redirecting to</param>
  /// <param name="parameters">parameters for the page redirected to</param>
  /// <param name="viewNav">Specifies the display status of the navigation bar for the page redirected to.</param>
  public static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      ShowNavigationBar showNavBar)
  {    

    // Add referrer information for all pages
    string referrerUrl = GetPhysicalPageUrl (sourcePage);
    parameters.Add ("Referrer", referrerUrl); 

    string supressNavigation = string.Empty ;
    if (showNavBar == ShowNavigationBar.Show)
      supressNavigation = "&" + c_supressNavParam + "=" + c_supressNavValue;
    else if (showNavBar == ShowNavigationBar.UseDefault)
    {
      if (sourcePage.Request.QueryString[c_supressNavParam] == c_supressNavValue)
        supressNavigation = "&" + c_supressNavParam + "=" + c_supressNavValue;
    }

    string token = GetUniqueToken ();
    if (parameters != null)
      sourcePage.Session[token] = parameters;
    
    string urlDelimiter;
    if (destinationUrl.IndexOf ("?") > 0)
      urlDelimiter = "&";
    else
      urlDelimiter = "?";

    string relAppPath = destinationUrl + urlDelimiter + "pageToken=" + token + supressNavigation;

    string url = GetPhysicalPageUrl (sourcePage, relAppPath);

    //string url = sourcePage.Request.ApplicationPath + "/" + destinationUrl 
    //    + urlDelimiter + "pageToken=" + token 
    //    + supressNavigation;

    NavigateTo (sourcePage, url, returnToThisPage);
  }

  public static void NavigateTo (Page sourcePage, string url, bool returnToThisPage)
  {
    INavigablePage navigablePage = sourcePage as INavigablePage;
    if (navigablePage != null)
      navigablePage.NavigateTo (url, returnToThisPage);
    else
      PageUtility.Redirect (sourcePage.Response, url);
  }

  /// <summary>
  /// Returns an IDictionary with the parameters the page was called with
  /// and copies the parameters to the new token
  /// </summary>
  /// <param name="requireParameters">If false, no exception is thrown if parameters are not found.</param>
  /// <exception cref="NoPageParametersException">Thrown when a page is called without parameters.</exception>
  /// <exception cref="SessionTimeout">Thrown when the session is timed out.</exception>
  /// <returns>IDictionary containing parameters</returns>
  public static IDictionary GetCallParameters (Page page, bool requireParameters)
  {
    string token = (string) page.Request.Params["pageToken"];

    IDictionary parameters = null;
    if (token != null)
      parameters = (IDictionary) page.Session[token];

    if (requireParameters && parameters == null)
    {
      if (! page.IsPostBack)
        throw new NoPageParametersException (page);
      else 
        throw new SessionTimeoutException ();
    }

    return parameters;
  }
}

[Serializable]
public class NoPageParametersException : ApplicationException
{
  private Page _page;

  public NoPageParametersException (Page page)
  {
    _page = page;
  }

  public Page Page
  {
    get { return _page; }
  }

  public override string Message 
  {
    get 
    { 
      return string.Format ("Page {0} requires parameters, but no parameters were specified. "
          + "Use CallPage() to invoke this page.", _page.ID); 
    }
  }
}

[Serializable]
public class NoPageTokenException : ApplicationException
{
  private string _url;

  public NoPageTokenException (Page page)
  {
    _url = page.Request.Url.GetLeftPart (UriPartial.Path);
  }

  public string Url
  {
    get { return _url; }
  }

  public override string Message 
  {
    get 
    { 
      return string.Format ("Page {0} requires a page token, but no token was specified. "
          + "Use CallPage() to invoke this page.", _url); 
    }
  }
}

[Serializable]
public class SessionVariableNotFoundException : ApplicationException
{
  private string _key;

  public SessionVariableNotFoundException (string key)
  {
    _key = key;
  }

  public string Key
  {
    get { return _key; }
  }

  public override string Message 
  {
    get 
    { 
      return string.Format ("The current session does not contain a key \"{0}\".", _key);
    }
  }
}

[Serializable]
public class SessionTimeoutException : ApplicationException
{
  public override string Message 
  {
    get { return "Session expired"; }
  }
}

}

