using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web;
using System.Configuration;
using System.Diagnostics;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.Utilities
{
/// <summary>
/// Utility class for pages.
/// </summary>
public class PageUtility
{
  #region Obsolete UrlUtility

  [Obsolete ("Legacy implementation moved to StandardPageUtility in Rubicon.Web.Legacy.dll. New implementation moved to UrlUtility.AddParameter")]
  public static string AddUrlParameter (string url, string name, string value)
  {
    return UrlUtility.AddParameter (url, name, value);
  }

  [Obsolete ("Legacy implementation moved to StandardPageUtility in Rubicon.Web.Legacy.dll. New implementation moved to UrlUtility.DeleteParameter.")]
  public static string DeleteUrlParameter (string url, string name)
  {
    return UrlUtility.DeleteParameter (url, name);
  }

  [Obsolete ("Legacy implementation moved to StandardPageUtility in Rubicon.Web.Legacy.dll. New implementation moved to UrlUtility.GetParameter.")]
  public static string GetUrlParameter (string url, string name)
  {
    return UrlUtility.GetParameter (url, name);
  }

  #endregion

  #region Obsolete ScriptUtility

  [Obsolete ("Moved to ScriptUtility.")]
  public static string EscapeClientScript (string input)
  {
    return ScriptUtility.EscapeClientScript (input);
  }
 
  [Obsolete ("Moved to ScriptUtility.")]
  public static void RegisterClientScriptBlock (Page page, string key, string javascript)
  {
    ScriptUtility.RegisterClientScriptBlock (page, key, javascript);
  }

  [Obsolete ("Moved to ScriptUtility.")]
  public static void RegisterStartupScriptBlock (Page page, string key, string javascript)
  {
    ScriptUtility.RegisterStartupScriptBlock (page, key, javascript);
  }

  #endregion

  #region Obsolete StandardPageUtility

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public const string c_supressNavParam = "SupressNav";
  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public const string c_supressNavValue = "true";

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public enum NavigationBar 
  {
    Show, 
    Hide, 
    UseDefault
  };

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void RegisterOpenReportScript (Page page, string reportUrl)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void RegisterOpenReportScript (Page page, string reportUrl, string scriptKey)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static IDictionary GetCallParameters (Page page, bool requireParameters)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void DeleteOutdatedSessions (Page page)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }  

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static object GetSessionValue (Page page, string key, bool required)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void SetSessionValue (Page page, string key, object sessionValue)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void ClearSessionValue (Page page, string key)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static object GetSessionValue (Page page, string token, string key, bool required)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void SetSessionValue (Page page, string token, string key, object sessionValue)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void ClearSessionValue (Page page, string token, string key)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void ClearSession (Page page, string token)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetUniqueKey (Page page, string key)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetUniqueKey (string token, string key)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetUniqueToken ()
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string AddCleanupToken (Page page, string url)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string AddCleanupToken (object navigablePage, string url)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string AddParentToken (Page page, string url)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string AddActiveTabParameters (Page sourcePage, string url)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }
  
  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string AddPageToken (string url)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetToken (Page page)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetParentToken (Page page)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      NavigationBar showNavBar)
  {    
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      NavigationBar showNavBar,
      string referrerUrl)
  {    
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void NavigateTo (Page sourcePage, string url, bool returnToThisPage)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void RegisterWindowOpenJavascript (Page page)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetWindowOpenJavascript (string url, bool useScrollbars)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static void CloseBrowserWindow (Page page, bool refreshParent)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  #endregion

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
    return RemoveHttps (page, GetPhysicalPageUrl (page, relativeUrl));
  }

  public static string RemoveHttps (Page page, string url)
  {
    if (url.ToLower().StartsWith ("https://"))
    {
      return "http://" + url.Substring ("https://".Length);
    }
    else if (url.ToLower().StartsWith ("http://"))
    { 
      return url;
    }
    else
    {
      return "http://" + page.Request.Url.Host + url;
    }
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
  /// Gets the form's postback data in a fashion that works for WxePages too. Otherwise simialar to <c>Page.Request.Form</c>.
  /// </summary>
  public static NameValueCollection GetRequestCollection (Page page)
  {
    IWxePage wxePage = page as IWxePage;
    if (wxePage != null)
      return wxePage.GetPostBackCollection ();
    else
      return page.Request.Form;
  }

  public static string GetRequestCollectionItem (Page page, string name)
  {
    NameValueCollection collection = GetRequestCollection (page);
    if (collection == null)
      return null;
    return collection[name];
  }


  [Obsolete ("Use Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl) instead.")]
  public static void RegisterClientScriptInclude (Page page, string key, string scriptUrl)
  {
    Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
  }

  private PageUtility()
	{
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
