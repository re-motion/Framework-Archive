using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Collections;

using Rubicon.Web.UI.Utilities;

namespace Rubicon.Web.UI.Controls
{
/// <summary>
/// Provides a common implementation for interface INavigablePage with a TabControl for navigation.
/// </summary>
internal class NavigablePage //: MultiLingualPage, INavigablePage, IPostBackEventHandler
{
  // types

  // static members and constants
/*
  protected static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      bool returnToThisPage, 
      PageUtility.ShowNavigationBar showNavBar)
  {
    CallPage (sourcePage, destinationUrl, null, returnToThisPage, showNavBar);
  }

  /// <summary>
  /// redirects to the page identified by the given URL 
  /// </summary>
  /// <param name="destinationUrl">URL redirecting to</param>
  /// <param name="parameters">parameters for the page redirected to</param>
  protected static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      PageUtility.ShowNavigationBar showNavBar)
  {
    PageUtility.CallPage (sourcePage, destinationUrl, parameters, returnToThisPage, showNavBar);
  }

  /// <summary>
  /// redirects to the page identified by the given URL 
  /// </summary>
  /// <param name="destinationUrl">URL redirecting to</param>
  /// <param name="parameters">parameters for the page redirected to</param>
  protected static void CallPage (
      Page sourcePage, 
      string destinationUrl, 
      IDictionary parameters, 
      bool returnToThisPage,
      PageUtility.ShowNavigationBar showNavBar,
      string referrerUrl)
  {
    PageUtility.CallPage (sourcePage, destinationUrl, parameters, returnToThisPage, showNavBar, referrerUrl);
  }


  // member fields

  // construction and disposing

  protected override void InitializeSecurityService ()
  {
    try
    {
      SetSecurityService (WebGovSecurityService.GetOrCreateCurrent (Context));
    }
    catch (PortalParameterMissingFormsLoginException)
    {
      WebGovSecurityService.DeleteFormsLoginCookie (this.Response);
      this.Response.Redirect (PageUtility.GetPhysicalPageUrl (this, "default.aspx?newWindow=false"));
    }
    catch (MaintenanceModeException)
    {
      this.Response.Redirect (ResourceHelper.Current.GetUrl ("MaintenanceMode.htm"));
    }
    catch (InvalidClientAccessException e)
    {
      DBEventLog.WriteException (e, this.Session);
      this.Response.Redirect (ResourceHelper.Current.GetUrl ("InvalidClientAccess.htm"));
    }
    catch (DisabledLocationException e)
    {
      DBEventLog.WriteException (e, this.Session);
      this.Response.Redirect (ResourceHelper.Current.GetUrl ("DisabledLocation.htm"));      
    }
  }

  protected virtual SecurityService.AccessType GetRequiredAccessType()
  {
    return Rubicon.Findit.Common.Domain.SecurityService.AccessType.NoProtection;
  }

	protected override void OnInit(EventArgs e)
	{
    base.OnInit(e);

    DBEventLog.WriteSessionState (this.Session, "WebGovPage.OnInit");
   
    if (! IsPostBack)
    {
      string cleanupToken = this.Request.QueryString["cleanupToken"];
      CleanupSession (cleanupToken);
      PageUtility.DeleteOutdatedSessions( this );
    }

    Rubicon.Findit.Common.Domain.SecurityService.AccessType requiredAccessType = GetRequiredAccessType();
    if (! this.SecurityService.HasAccess (requiredAccessType) )
      throw new AccessDeniedException (requiredAccessType);

    this.Unload += new EventHandler (this.Page_Unload);
	}

  // methods and properties

  private void Page_Unload (object sender, System.EventArgs e)
  {
    DBEventLog.WriteSessionState (this.Session, "WebGovPage.Unload");
  }

  public new WebGovSecurityService SecurityService
  {
    get {return (WebGovSecurityService) base.SecurityService; }
  }

  public virtual bool AllowImmediateClose
  {
    get { return true; }
  }

  public virtual bool CleanupOnImmediateClose 
  { 
    get { return true; }
  }
  
  public virtual bool NavigationRequest (string url)
  {
    return true;
  }

  public virtual bool AutoDeleteSessionVariables 
  { 
    get { return true; }
  }

  public virtual void NavigateTo (string url, bool returnToThisPage)
  {
    // remove session variables
    if (! returnToThisPage)
    {
      string pageToken = this.Request.QueryString["pageToken"];
      CleanupSession (pageToken);
    }

    // copy certain parameters to new url
    string[] parameters = {"navSelectedTab", "navSelectedMenu"};
    foreach (string parameter in parameters)
    {
      bool urlHasParameter =     url.IndexOf ("?" + parameter + "=") != -1
                              || url.IndexOf ("&" + parameter + "=") != -1;
      if (! urlHasParameter)
      {
        string parameterValue = this.Request.QueryString[parameter];
        if (parameterValue != null && parameterValue != string.Empty)
          url = PageUtility.AddUrlParameter (url, parameter, parameterValue);
      }
    }

    PageUtility.Redirect (this.Response, url);
  }

  private const string c_eventNameShowMessageBoxResult = "ShowMessageBoxResult:";
  private const string c_eventNameNavigationRequest = "WebGovPageNavigationRequest";
  private const string c_viewStateNavigationUrl = "NavigationRequest:NavigateToUrl";

  public void RegisterMessageBox (string message, string eventName)
  {
    if (this.ID == null || this.ID == string.Empty)
      throw new InvalidOperationException ("Page must have ID in order to use ShowMessageBox.");

    string script = string.Format (
        "<script language=\"javascript\"> \n"
            + "if (confirm (\"{0}\")) \n"
            + "  " + this.GetPostBackClientEvent (this, c_eventNameShowMessageBoxResult + eventName + ":OK") + "\n"
            + "else \n"
            + "  " + this.GetPostBackClientEvent (this, c_eventNameShowMessageBoxResult + eventName + ":Cancel") + "\n" 
            + "</script>",
        message);

    this.RegisterStartupScript ("WebGovPage_ShowMessageBoxScript", script);
  }

  public void InitiateConditionalNavigation (string url, string message)
  {
    ViewState[c_viewStateNavigationUrl] = url;
    RegisterMessageBox (message, c_eventNameNavigationRequest);
  }

  public virtual void RaisePostBackEvent (string eventArgument)
  {
    if (eventArgument.StartsWith (c_eventNameShowMessageBoxResult))
    {
      // eventArgument example: "ShowMessageBoxResult:EventName:OK"
      string eventArgPart = eventArgument.Substring (c_eventNameShowMessageBoxResult.Length);
      // eventArgPart example: "EventName:OK"
      int colonPos = eventArgPart.IndexOf (":");
      string eventName = eventArgPart.Substring (0, colonPos);
      string resultString = eventArgPart.Substring (colonPos + 1);
      bool result = resultString == "OK";
      HandleMessageBoxResult (eventName, result);
    }
    else if (eventArgument == "WebGovPageBackLink")
    {
      CallBackLink();
    }
  }

  protected virtual void HandleMessageBoxResult (string eventName, bool result)
  {
    if (eventName == c_eventNameNavigationRequest)
    {
      if (result)
      {
        string url = (string) ViewState[c_viewStateNavigationUrl];
        NavigateTo (url, false);
      }
    }
  }

  /// <summary>
  /// converts a given key value to a page specific value
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  private string GetUniqueKey (string key)
  {
    return PageUtility.GetUniqueKey (this, key);
  }


  protected enum ShowBackLinkType 
  {
    Always,
    Never,
    Default
  }

  protected virtual ShowBackLinkType GetShowBackLinkType ()
  {
    return ShowBackLinkType.Default;
  }

  protected virtual bool IsServerSideBackLink()
  {
    return false;
  }

  public bool ShowBackLink()
  {
    ShowBackLinkType showBackLink = GetShowBackLinkType();
    if (showBackLink == ShowBackLinkType.Always)
      return true;
    else if (showBackLink == ShowBackLinkType.Never)
      return false;

    IDictionary parameters = GetCallParameters (false);
    if (parameters == null)
      return false;
    return parameters ["Referrer"] != null;
  }

  public virtual string GetBackLinkUrl ()
  {
    if (IsServerSideBackLink())
      return "javascript:" + GetPostBackClientEvent (this, "WebGovPageBackLink");

    IDictionary parameters = GetCallParameters (false);
    if (parameters != null)
      return (string) parameters ["Referrer"];

    string backLinkUrl = (string) GetSessionValue ( "referrerUrl", false);
    if (backLinkUrl == null)
    {
      if (!IsPostBack)
      {
        backLinkUrl = Request.UrlReferrer.AbsoluteUri;
        SetSessionValue (this.Token, "referrerUrl", backLinkUrl);
      }
      else
      {
        throw new SessionVariableNotFoundException ("referrerUrl");
      }
    }
    return backLinkUrl;
  }

  protected virtual void CallBackLink ()
  {
  }
  */
}
}
