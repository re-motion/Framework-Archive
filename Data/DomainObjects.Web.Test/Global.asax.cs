using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Web.Test.Domain;

namespace Rubicon.Data.DomainObjects.Web.Test 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
		 // MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeof (ClassWithAllDataTypes).Assembly)));
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
      //MappingLoaderConfiguration mappingloader = (MappingLoaderConfiguration) ConfigurationManager.GetSection ("rubicon.data.domainObjects/mapping");
      //PersistenceConfiguration storage = (PersistenceConfiguration) ConfigurationManager.GetSection ("rubicon.data.domainObjects/storage");
      //IDomainObjectsConfiguration group = DomainObjectsConfiguration.Current;
      //PersistenceConfiguration s2 = group.Storage;
      //MappingLoaderConfiguration m2 = group.MappingLoader;
    }

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

    protected virtual void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
    {
    }

    protected void Application_PostRequestHandlerExecute(Object sender, EventArgs e)
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

