using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections;
using System.Threading;
using Rubicon.Web;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;
using Rubicon.Globalization;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Controls;
using System.Web;

namespace OBWTest.IndividualControlTests
{

[MultiLingualResources ("OBWTest.Globalization.TestBasePage")]
public class TestBasePage :
    WxePage, 
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
{  
  protected new TestFunction CurrentFunction
  {
    get { return (TestFunction) base.CurrentFunction; }
  }

  protected override void OnInit(EventArgs e)
  {
    if (! ControlHelper.IsDesignMode (this, Context))
    {
      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
    }

    RegisterEventHandlers();
    base.OnInit (e);
  }

  protected override void OnPreRender(EventArgs e)
  {
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

    string key = GetType().FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string href = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (ResourceUrlResolver), ResourceType.Html, "Style.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, href);
    }

    key = GetType().FullName + "_Global";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string href = UrlUtility.GetAbsoluteUrl (Page, "~/Html/global.css", false);
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, href);
    }

    base.OnPreRender (e);
  }

  protected virtual void RegisterEventHandlers()
  {
  }

  protected virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    if (MultiLingualResources.ExistsResource (type))
      return MultiLingualResources.GetResourceManager (type, true);
    else
      return null;
  }

  IResourceManager IObjectWithResources.GetResourceManager()
  {
    return GetResourceManager();
  }
}

}
