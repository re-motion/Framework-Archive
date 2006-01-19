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

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.TestBasePage")]
public class TestWxeBasePage:
    WxePage, 
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
    // IResourceUrlResolver //  Provides the URLs for this WebForm (e.g. to the FormGridManager)
{  
  private Button _nextButton = new Button();

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

      _nextButton.ID = "NextButton";
      _nextButton.Text = "Next";
      WxeControls.AddAt (0, _nextButton);
    }

    ShowAbortConfirmation = Rubicon.Web.UI.ShowAbortConfirmation.Always;
    EnableAbort = Rubicon.NullableValueTypes.NaBooleanEnum.False;
    base.OnInit (e);
    RegisterEventHandlers();
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    string key = GetType().FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (ResourceUrlResolver), ResourceType.Html, "Style.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    key = GetType().FullName + "_Global";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, "Html/global.css");
    }

    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

    LiteralControl stack = new LiteralControl();

    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    sb.Append ("<br><div>");
    sb.Append ("<b>Stack:</b><br>");
    for (WxeStep step = CurrentStep; step != null; step = step.ParentStep)
      sb.AppendFormat ("{0}<br>", step.ToString());      
    sb.Append ("</div>");
    stack.Text = sb.ToString();
    
    WxeControls.Add (stack);
  }

  protected virtual void RegisterEventHandlers()
  {
    _nextButton.Click += new EventHandler(NextButton_Click);
  }

  protected virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    if (MultiLingualResourcesAttribute.ExistsResource (type))
      return MultiLingualResourcesAttribute.GetResourceManager (type, true);
    else
      return null;
  }

  IResourceManager IObjectWithResources.GetResourceManager()
  {
    return GetResourceManager();
  }

//  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
//  {
//    if (ControlHelper.IsDesignMode (this, this.Context))
//      return resourceType.Name + "/" + relativeUrl;
//    else
//      return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
//  }

  private void NextButton_Click(object sender, System.EventArgs e)
  {
    ExecuteNextStep();
  }

  protected virtual ControlCollection WxeControls
  {
    get { return WxeForm.Controls; }
  }
}

}
