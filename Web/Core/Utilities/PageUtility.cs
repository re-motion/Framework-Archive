using System;
using System.Collections;
using System.Web.UI;

using System.Diagnostics;

namespace Rubicon.Findit.Client.Controls
{

/// <summary>
/// Utility class for pages.
/// </summary>
public class PageUtility
{
	public PageUtility()
	{
	}

  public static object GetSessionValue (Page page, string key, bool required)
  {
    object o = page.Session[GetUniqueKey (page, key)];
    
    if (required && o == null)
       throw new SessionTimeoutException ();

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

  public static bool IsSessionValueSet (Page page, string key)
  {
    return page.Session[GetUniqueKey (page, key)] != null;
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

  /// <summary>
  /// returns a formatted GUID string
  /// </summary>
  /// <returns></returns>
  public static string GetUniqueToken ()
  {
    return Guid.NewGuid ().ToString ("N") ;
  }

  /// <summary>
  /// Returns the correct Url for a specified page with a relative Url 
  /// even if cookieless mode is activated.
  /// </summary>  
  public static string GetPageUrl (Page page, string relativeUrl)
  { 
    string returnUrl;

    Uri pageUrl = new Uri (page.Request.Url, relativeUrl);
    
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
  /// Returns the correct page's URL even if cookieless mode is activated.
  /// </summary>
  public static string GetPageUrl (Page page)
  {
    return GetPageUrl (page, string.Empty);
  }

  public static void CallPage (Page sourcePage, string destinationUrl, IDictionary parameters)
  {
    PageUtility.CallPage (sourcePage, destinationUrl, parameters, true);
  }

  /// <summary>
  /// redirects to the page identified by the given URL 
  /// </summary>
  /// <param name="destinationUrl">URL redirecting to</param>
  /// <param name="parameters">parameters for the page redirected to</param>
  /// <param name="viewNavigationBar" >if false, navigationbar is supressed</param>
  public static void CallPage (Page sourcePage, string destinationUrl, IDictionary parameters, bool viewNavigationBar)
  {    
    // Add referrer information for all pages
    string referrerUrl = GetPageUrl (sourcePage);
    parameters.Add ("Referrer", referrerUrl); 

    string supressNavigation = string.Empty ;
    if (!viewNavigationBar)
      supressNavigation = "&SupressNav=true";

    string token = GetUniqueToken ();
    if (parameters != null)
      sourcePage.Session[token] = parameters;
    
    sourcePage.Response.Redirect (
        sourcePage.Request.ApplicationPath + "/" + destinationUrl +"?pageToken=" + token + supressNavigation);
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
public class SessionTimeoutException : ApplicationException
{
  public override string Message 
  {
    get { return "Session expired"; }
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
}
