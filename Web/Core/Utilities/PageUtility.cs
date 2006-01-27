using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Rubicon.Utilities;
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
  public static string RemoveHttps (Page page, string url)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetPhysicalHttpPageUrl (Page page, string relativeUrl)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetPhysicalPageUrl (Page page)
  {
    throw new NotImplementedException ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.");
  }

  [Obsolete ("Moved to StandardPageUtility in Rubicon.Web.Legacy.dll.", true)]
  public static string GetPhysicalPageUrl (Page page, string relativeUrl)
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

  #endregion

  [Obsolete ("Use Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl) instead.")]
  public static void RegisterClientScriptInclude (Page page, string key, string scriptUrl)
  {
    Rubicon.Web.UI.HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, scriptUrl);
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
  ///   Gets the form's postback data in a fashion that works for WxePages too. 
  ///   Otherwise simialar to <b>Page.Request.Form</b>.
  /// </summary>
  /// <param name="page"> The page to query for the request collection. Must not be <see langword="null"/>. </param>
  /// <returns> 
  ///   The <see cref="NameValueCollection"/> returned by 
  ///   <see cref="IWxePage.GetPostBackCollection">IWxePage.GetPostBackCollection</see> or the 
  ///   <see cref="HttpRequest.Form"/> collection of the <see cref="Page.Request"/>, depending on whether or not the
  ///   <paramref name="page"/> implements <see cref="IWxePage"/>.
  /// </returns>
  public static NameValueCollection GetRequestCollection (Page page)
  {
    IWxePage wxePage = page as IWxePage;
    if (wxePage != null)
      return wxePage.GetPostBackCollection ();
    else
      return page.Request.Form;
  }

  /// <summary>
  ///   Gets a single item from the form's postback data in a fashion that works for WxePages too. 
  ///   Otherwise simialar to <b>Page.Request.Form</b>.
  /// </summary>
  /// <param name="page"> The page to query for the request collection. Must not be <see langword="null"/>. </param>
  /// <param name="name"> The name of the item to be returned. Must not be <see langword="null"/> or empty. </param>
  /// <returns> 
  ///   The item identified by <paramref name="name"/> or <see langword="null"/> if the item could not be found. 
  /// </returns>
  public static string GetRequestCollectionItem (Page page, string name)
  {
    ArgumentUtility.CheckNotNull ("page", page);
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);

    NameValueCollection collection = GetRequestCollection (page);
    if (collection == null)
      return null;
    return collection[name];
  }

  private PageUtility()
	{
	}
}

}
