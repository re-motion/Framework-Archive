using System;
using System.Collections;
using System.Collections.Generic;
using Mixins.Utilities;
using Rubicon.Collections;
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

    public override bool Equals (object obj)
    {
      ClassContext other = obj as ClassContext;
      if (other == null || !other.Type.Equals (Type) || other._mixins.Count != _mixins.Count)
        return false;

      for (int i = 0; i < _mixins.Count; ++i)
        if (!_mixins[i].Equals (other._mixins[i]))
          return false;

      return true;
    }

    public override int GetHashCode ()
    {
      return Type.GetHashCode() ^ EqualityUtility.GetRotatedHashCode (_mixins);
    }
  }
}