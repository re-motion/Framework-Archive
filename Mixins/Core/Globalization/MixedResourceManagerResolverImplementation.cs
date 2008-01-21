using System;
using System.Collections;
using Rubicon.Globalization;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Mixins.Globalization
{
  public class MixedResourceManagerResolverImplementation<TAttribute> : ResourceManagerResolverImplementation<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    protected override object GetResourceManagerSetCacheKey (Type definingType, bool includeHierarchy)
    {
      return Tuple.NewTuple (
          base.GetResourceManagerSetCacheKey (definingType, includeHierarchy),
          TypeFactory.GetContext (definingType, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured));
    }

    protected override TAttribute[] FindFirstResourceDefinitionsInBaseTypes (Type concreteType, out Type definingType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);

      TargetClassDefinition mixinConfiguration = TypeFactory.GetActiveConfiguration (concreteType);
      if (mixinConfiguration != null)
      {
        foreach (MixinDefinition mixinDefinition in mixinConfiguration.Mixins)
        {
          TAttribute[] attributes;
          FindFirstResourceDefinitions (mixinDefinition.Type, false, out definingType, out attributes);
          if (attributes.Length != 0)
            return attributes;
        }
      }
      return base.FindFirstResourceDefinitionsInBaseTypes (concreteType, out definingType);
    }

    protected override void WalkHierarchyAndPrependResourceManagers (System.Collections.ArrayList resourceManagers, Type definingType)
    {
      ArgumentUtility.CheckNotNull ("resourceManagers", resourceManagers);
      ArgumentUtility.CheckNotNull ("definingType", definingType);

      TargetClassDefinition mixinConfiguration = TypeFactory.GetActiveConfiguration (definingType);
      if (mixinConfiguration != null)
      {
        foreach (MixinDefinition mixinDefinition in mixinConfiguration.Mixins)
          PrependMixinResourceManagers (resourceManagers, mixinDefinition.Type);
      }

      base.WalkHierarchyAndPrependResourceManagers (resourceManagers, definingType);
    }

    private void PrependMixinResourceManagers (ArrayList resourceManagers, Type mixinType)
    {
      Type currentType;
      TAttribute[] resourceAttributes;
      FindFirstResourceDefinitions (mixinType, true, out currentType, out resourceAttributes);
      if (currentType != null)
      {
        resourceManagers.InsertRange (0, GetResourceManagers (currentType.Assembly, resourceAttributes));
        WalkHierarchyAndPrependResourceManagers (resourceManagers, currentType);
      }
    }
  }
}