using System;
using System.Globalization;
using System.Threading;
using Rubicon.Globalization;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions;
using Rubicon.Web;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.Classes
{
  [WebMultiLingualResources (typeof (GlobalResources))]
  public abstract class BasePage : WxePage, IObjectWithResources 
  {
    // types
    private const string c_globalStyleFileUrl = "Style.css";
    private const string c_globalStyleFileKey = "SecurityManagerGlobalStyle";

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected new BaseTransactedFunction CurrentFunction
    {
      get { return (BaseTransactedFunction) base.CurrentFunction; }
    }

    protected virtual IFocusableControl InitialFocusControl
    {
      get { return null; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      RegisterStyleSheets ();
    }

    protected override void OnPreRender (EventArgs e)
    {
      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      if (!IsPostBack && InitialFocusControl != null)
        SetFocus (InitialFocusControl);

      base.OnPreRender (e);
    }

    private void RegisterStyleSheets ()
    {
      string url = ResourceUrlResolver.GetResourceUrl (this, typeof (ResourceUrlResolver), ResourceType.Html, "style.css");

      HtmlHeadAppender.Current.RegisterStylesheetLink (this.GetType () + "style", url);

      if (!HtmlHeadAppender.Current.IsRegistered (c_globalStyleFileKey))
      {
        string styleUrl = ResourceUrlResolver.GetResourceUrl (this, typeof (BasePage), ResourceType.Html, c_globalStyleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (c_globalStyleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    IResourceManager IObjectWithResources.GetResourceManager ()
    {
      return this.GetResourceManager ();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType ();

      if (MultiLingualResourcesAttribute.ExistsResource (type))
        return MultiLingualResourcesAttribute.GetResourceManager (type, true);
      else
        return null;
    }
  }
}
