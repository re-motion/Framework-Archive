using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Mixins.Context;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design.BindableObject
{
  public class BindableObjectTypeFinder
  {
    private readonly IServiceProvider _serviceProvider;

    public BindableObjectTypeFinder (IServiceProvider serviceProvider)
    {
      ArgumentUtility.CheckNotNull ("serviceProvider", serviceProvider);
      _serviceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider
    {
      get { return _serviceProvider; }
    }

    public List<Type> GetTypes (bool includeGac)
    {
      List<Type> types = new List<Type>();
      ApplicationContext applicationContext = ApplicationContextBuilder.BuildContextFromAssemblies (GetAssemblies (includeGac));
      foreach (ClassContext classContext in applicationContext.ClassContexts)
      {
        if (BindableObjectMixin.HasMixin (classContext.Type, applicationContext))
          types.Add (classContext.Type);
      }
      return types;
    }

    private Assembly[] GetAssemblies (bool includeGac)
    {
      ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService) _serviceProvider.GetService (typeof (ITypeDiscoveryService));
      if (typeDiscoveryService == null)
        return new Assembly[0];

      Set<Assembly> assemblySet = new Set<Assembly>();
      foreach (Type type in typeDiscoveryService.GetTypes (typeof (object), !includeGac))
        assemblySet.Add (type.Assembly);

      return assemblySet.ToArray();
    }
  }
}