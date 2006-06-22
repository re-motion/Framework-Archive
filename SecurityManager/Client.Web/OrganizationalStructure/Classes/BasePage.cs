using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Globalization;
using System.Threading;
using System.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.Utilities;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes
{
  [WebMultiLingualResources (typeof (GlobalResources))]
  public abstract class BasePage : WxePage, IObjectWithResources 
  {
    // types

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

      if (Request.UserLanguages.Length > 0)
      {
        try
        {
          string[] cultureInfo = Request.UserLanguages[0].Split (';');

          if (cultureInfo.Length > 0)
          {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture (cultureInfo[0]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo (cultureInfo[0]);
          }
        }
        catch (ArgumentException)
        {
          // if cultureInfo contains a invalid value we just ignore it
        }
      }

      this.DataBind ();
    }

    protected override void OnPreRender (EventArgs e)
    {
      RegisterStyleSheets ();

      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      if (!IsPostBack && InitialFocusControl != null)
        SetFocus (InitialFocusControl);

      base.OnPreRender (e);
    }

    private void RegisterStyleSheets ()
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, typeof (ResourceUrlResolver), ResourceType.Html, "style.css");

      HtmlHeadAppender.Current.RegisterStylesheetLink (this.GetType () + "style", url);

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
