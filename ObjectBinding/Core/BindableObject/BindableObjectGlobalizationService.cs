using System;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectGlobalizationService : IBindableObjectGlobalizationService
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Rubicon.ObjectBinding.Globalization.BindableObjectGlobalizationService")]
    private enum ResourceIdentifier
    {
      True,
      False
    }

    private readonly InterlockedCache<Type, IResourceManager> _resourceManagerCache = new InterlockedCache<Type, IResourceManager>();

    public BindableObjectGlobalizationService ()
    {
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return EnumDescription.GetDescription (value) ?? value.ToString();
    }

    public string GetBooleanValueDisplayName (bool value)
    {
      IResourceManager resourceManager = GetResourceManagerFromCache (typeof (ResourceIdentifier));
      return resourceManager.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    public string GetPropertyDisplayName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      IResourceManager resourceManager = GetResourceManagerFromCache (propertyInfo.DeclaringType);
      string resourceID = "property:" + propertyInfo.Name;
      if (!resourceManager.ContainsResource (resourceID))
        return propertyInfo.Name;
      return resourceManager.GetString (resourceID);
    }

    private IResourceManager GetResourceManagerFromCache (Type type)
    {
      IResourceManager resourceManager;
      if (_resourceManagerCache.TryGetValue (type, out resourceManager))
        return resourceManager;
      return _resourceManagerCache.GetOrCreateValue (type, GetResourceManager);
    }

    private IResourceManager GetResourceManager (Type type)
    {
      if (!MultiLingualResourcesAttribute.ExistsResource (type))
        return NullResourceManager.Instance;
      return MultiLingualResourcesAttribute.GetResourceManager (type, true);
    }
  }
}