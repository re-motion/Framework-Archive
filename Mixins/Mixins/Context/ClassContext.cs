using System;
using System.Collections.Generic;
using Mixins.Utilities;
using Rubicon.Utilities;

namespace Mixins.Context
{
  [Serializable]
  public class ClassContext
  {
    private Type _type;
    private List<Type> _mixins = new List<Type> ();
    private Set<Type> _mixinsForFastLookup = new Set<Type> ();

    public ClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IEnumerable<Type> Mixins
    {
      get { return _mixins; }
    }

    public int MixinCount
    {
      get { return _mixins.Count; }
    }

    public bool ContainsMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      return _mixinsForFastLookup.Contains (mixinType);
    }

    public void AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      if (!ContainsMixin (mixinType))
      {
        _mixins.Add (mixinType);
        _mixinsForFastLookup.Add (mixinType);
      }
    }
  }
}