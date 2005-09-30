using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Globalization;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Web;
namespace OBWTest 
{
public class Global : System.Web.HttpApplication // , IResourceUrlResolver
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	public Global()
	{
    //  Initialize Logger
	  log4net.LogManager.GetLogger (typeof (Global));
		InitializeComponent();
	}	
	
//  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
//  {
//    return this.Request.ApplicationPath + "/" + resourceType.Name + "/" + relativeUrl;
//  }

	protected void Application_Start(Object sender, EventArgs e)
	{
    log4net.Config.DOMConfigurator.Configure();

    string objectPath = Server.MapPath ("objects");
    if (! Directory.Exists (objectPath))
      Directory.CreateDirectory (objectPath);

    ReflectionBusinessObjectStorage.RootPath = objectPath;
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

