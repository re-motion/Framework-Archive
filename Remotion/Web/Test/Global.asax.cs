// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test
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

    protected void Application_Start (Object sender, EventArgs e)
    {
      XmlConfigurator.Configure ();

      IWindsorContainer container = new WindsorContainer ();

      RegisterRendererFactories (container, ".StandardMode.Factories");
      container.Register (Component.For<IScriptUtility> ().ImplementedBy<ScriptUtility> ().LifeStyle.Singleton);
      container.Register (Component.For<ResourceTheme> ().Instance (Remotion.Web.ResourceTheme.NovaBlue));

      Application.Set (typeof (IServiceLocator).AssemblyQualifiedName, new WindsorServiceLocator (container));
      ServiceLocator.SetLocatorProvider (() => (IServiceLocator) Application.Get (typeof (IServiceLocator).AssemblyQualifiedName));
    }

    private void RegisterRendererFactories (IWindsorContainer container, string namespaceSuffix)
    {
      // Remotion.Web.Core
      container.Register (
          AllTypes.Pick ()
              .FromAssembly (typeof (RendererBase<>).Assembly)
              .If (t => t.Namespace.EndsWith (namespaceSuffix))
              .WithService.Select ((t, b) => t.GetInterfaces ())
              .Configure (c => c.Named (c.ServiceType.Name)));
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
      //      string appPath = Request.ApplicationPath;
      //
      //      if (!appPath.EndsWith ("/"))
      //        appPath += "/";
      //
      //      Server.Transfer (appPath + "Start.aspx");
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
