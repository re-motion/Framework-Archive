﻿using System;
using System.IO;
using System.Web;
using Remotion.Development.Web.ResourceHosting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public class Global : HttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      var objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);

      SetObjectStorageProvider(objectPath);
      RegisterAutoCompleteService();
      RegisterIconService();
      RegisterResourceVirtualPathProvider();
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }
    
    private static void SetObjectStorageProvider (string objectPath)
    {
      var provider = new XmlReflectionBusinessObjectStorageProvider (objectPath);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
    }

    private static void RegisterAutoCompleteService ()
    {
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
          .AddService (typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService());
    }

    private static void RegisterIconService ()
    {
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
          .AddService (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService());
    }

    private static void RegisterResourceVirtualPathProvider ()
    {
      _resourceVirtualPathProvider = new ResourceVirtualPathProvider (
          new[]
          {
              new ResourcePathMapping ("Remotion.Web", @"..\..\Web\Core\res"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web", @"..\..\ObjectBinding\Web\res")
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();
    }
  }
}