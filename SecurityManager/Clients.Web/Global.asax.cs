// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.Security.UI;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web
{
  public class Global : SecurityManagerHttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), new WebSecurityAdapter());
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter());

      //Windsor dependency does not come into play unless application_start is invoked. This only happens in stand-alone applications.
      IWindsorContainer container = new WindsorContainer ();
      container.Register (
          AllTypes.Pick ()
              .FromAssembly (typeof (RendererBase<>).Assembly)
              .If (t => t.Namespace.EndsWith (".QuirksMode.Factories"))
              .WithService.Select ((t, b) => t.GetInterfaces ())
              .Configure (c => c.Named ("default." + c.ServiceType.Name)));

      Application.Set (typeof (IServiceLocator).AssemblyQualifiedName, new WindsorServiceLocator (container));
      ServiceLocator.SetLocatorProvider (() => (IServiceLocator) Application.Get (typeof (IServiceLocator).AssemblyQualifiedName));
    }

    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}
