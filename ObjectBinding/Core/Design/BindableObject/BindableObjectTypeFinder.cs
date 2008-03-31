using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Mixins;
using Rubicon.Mixins.Context;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;
using System.Collections;

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
      MixinConfiguration applicationContext = GetApplicationContext (includeGac);

      List<Type> bindableTypes = new List<Type>();
      //using (applicationContext.EnterScope ())
      //{
      //  ICollection types = GetAllDesignerTypes (includeGac);
      //  foreach (Type type in types)
      //  {
      //    if (!Mixins.TypeUtility.IsGeneratedByMixinEngine (type) 
      //        && Mixins.TypeUtility.HasAscribableMixin (type, typeof (BindableObjectMixinBase<>)))
      //      bindableTypes.Add (type);
      //  }
      //}

      foreach (ClassContext classContext in applicationContext.ClassContexts)
      {
        if (Mixins.TypeUtility.HasAscribableMixin (classContext.Type, typeof (BindableObjectMixinBase<>)))
          bindableTypes.Add (classContext.Type);
      }
      return bindableTypes;
    }

    public MixinConfiguration GetApplicationContext (bool includeGac)
    {
      ICollection typesToBeAnalyzed = GetAllDesignerTypes (includeGac);

      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      foreach (Type type in typesToBeAnalyzed)
        builder.AddType (type);

      return builder.BuildConfiguration ();
    }

    private ICollection GetAllDesignerTypes (bool includeGac)
    {
      ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService) _serviceProvider.GetService (typeof (ITypeDiscoveryService));
      if (typeDiscoveryService == null)
        return Type.EmptyTypes;
      else
        return typeDiscoveryService.GetTypes (typeof (object), !includeGac);
    }
  }
}