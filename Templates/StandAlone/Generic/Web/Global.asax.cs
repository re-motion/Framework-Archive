using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;

using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.Utilities;

namespace Rubicon.Templates.Generic.Web 
{

public class Global : System.Web.HttpApplication, IResourceUrlResolver
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	public Global()
	{
		InitializeComponent();
	}	
	
  string IResourceUrlResolver.GetResourceUrl (
      Control control, 
      Type definingType, 
      ResourceType resourceType, 
      string relativeUrl)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    ArgumentUtility.CheckNotNull ("resourceType", resourceType);
    ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

    bool isDesignMode = ControlHelper.IsDesignMode (control);
    bool isTypeResource = definingType != null;
    if (isDesignMode || isTypeResource)
    {
      return ResourceUrlResolver.GetResourceUrl (control, definingType, resourceType, relativeUrl);
    }
    else
    {
      string root = string.Empty;
      if (HttpRuntime.AppDomainAppVirtualPath != "/")
        root = HttpRuntime.AppDomainAppVirtualPath + root;
      if (! root.EndsWith ("/"))
        root += "/";
      return root + resourceType.Name + "/" + relativeUrl;
    }
  }

  protected void Application_Start(Object sender, EventArgs e)
	{

	}

	protected void Session_Start(Object sender, EventArgs e)
	{

	}

	protected void Application_BeginRequest(Object sender, EventArgs e)
	{

	}

	protected void Application_EndRequest(Object sender, EventArgs e)
	{

	}

	protected void Application_AuthenticateRequest(Object sender, EventArgs e)
	{

	}

	protected void Application_Error(Object sender, EventArgs e)
	{

	}

	protected void Session_End(Object sender, EventArgs e)
	{

	}

	protected void Application_End(Object sender, EventArgs e)
	{

	}
		
	#region Web Form Designer generated code
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
		this.components = new System.ComponentModel.Container();
	}
	#endregion
}

}
