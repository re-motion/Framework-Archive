using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
      ICollection types = GetAllDesignerTypes (includeGac);
      MixinConfiguration applicationContext = GetMixinConfiguration (types);

      List<Type> bindableTypes = new List<Type>();
      using (applicationContext.EnterScope ())
      {
        foreach (Type type in types)
        {
          if (!Mixins.TypeUtility.IsGeneratedByMixinEngine (type)
              && Mixins.TypeUtility.HasAscribableMixin (type, typeof (BindableObjectMixinBase<>)))
            bindableTypes.Add (type);
        }
      }
      return bindableTypes;
    }

    public MixinConfiguration GetMixinConfiguration (bool includeGac)
    {
      ICollection typesToBeAnalyzed = GetAllDesignerTypes (includeGac);
      return GetMixinConfiguration(typesToBeAnalyzed);
    }

    private MixinConfiguration GetMixinConfiguration (ICollection typesToBeAnalyzed)
    {
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