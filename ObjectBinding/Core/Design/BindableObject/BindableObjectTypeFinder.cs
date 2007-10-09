using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rubicon.Collections;
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
      ApplicationContext applicationContext = GetApplicationContext (includeGac);

      List<Type> bindableTypes = new List<Type> ();
      foreach (ClassContext classContext in applicationContext.ClassContexts)
      {
        if (Mixins.TypeUtility.HasAscribableMixin (classContext.Type, typeof (BindableObjectMixinBase<>)))
          bindableTypes.Add (classContext.Type);
      }
      return bindableTypes;
    }

    public ApplicationContext GetApplicationContext (bool includeGac)
    {
      IEnumerable typesToBeAnalyzed = GetAllDesignerTypes (includeGac);

      ApplicationContextBuilder builder = new ApplicationContextBuilder (null);
      foreach (Type type in typesToBeAnalyzed)
        builder.AddType (type);

      return builder.BuildContext ();
    }

    private IEnumerable GetAllDesignerTypes (bool includeGac)
    {
      ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService) _serviceProvider.GetService (typeof (ITypeDiscoveryService));
      if (typeDiscoveryService == null)
        return Type.EmptyTypes;
      else
        return typeDiscoveryService.GetTypes (typeof (object), !includeGac);
    }
  }
}