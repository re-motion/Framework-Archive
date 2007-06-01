using System;
using Mixins.Definitions.Building;
using Mixins.Utilities.Singleton;
using Rubicon.Collections;
using Mixins.Context;

namespace Mixins.Definitions
{
  public class BaseClassDefinitionCache : CallContextSingletonBase<BaseClassDefinitionCache, DefaultInstanceCreator<BaseClassDefinitionCache>>
  {
    private static BaseClassDefinitionBuilder s_definitionBuilder = new BaseClassDefinitionBuilder();

    private object _syncObject = new object();
    private ICache<ClassContext, BaseClassDefinition> _cache = new Cache<ClassContext, BaseClassDefinition>();

    public bool IsCached (ClassContext context)
    {
      lock (_syncObject)
      {
        BaseClassDefinition dummy;
        return _cache.TryGetValue (context, out dummy);
      }
    }

    public BaseClassDefinition GetBaseClassDefinition (ClassContext context)
    {
      context.Freeze();

      BaseClassDefinition definition;
      lock (_syncObject)
        _cache.TryGetValue (context, out definition);
      if (definition == null)
      {
        definition = s_definitionBuilder.Build (context);
        lock (_syncObject)
        {
          definition = _cache.GetOrCreateValue (context, delegate { return definition; });
        }
      }
      return definition;
    }
  }
}