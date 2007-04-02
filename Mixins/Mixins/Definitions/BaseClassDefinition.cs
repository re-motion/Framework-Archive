using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Mixins.Definitions
{
  public class BaseClassDefinition : ClassDefinition
  {
    private Dictionary<Type, MixinDefinition> _mixins = new Dictionary<Type, MixinDefinition> ();
    private Dictionary<Type, InterfaceIntroductionDefinition> _introducedInterfaces = new Dictionary<Type, InterfaceIntroductionDefinition> ();
    private Dictionary<Type, Type> _requiredFaceInterfaces = new Dictionary<Type, Type> ();

    public BaseClassDefinition (Type type)
        : base (type)
    {
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public IEnumerable<MixinDefinition> Mixins
    {
      get { return _mixins.Values; }
    }

    public bool HasMixin (Type type)
    {
      return _mixins.ContainsKey (type);
    }

    public void AddMixin (MixinDefinition newMixin)
    {
      if (HasMixin (newMixin.Type))
      {
        string message = string.Format ("Base class {0} already has an instance of mixin {1}.", FullName, newMixin.FullName);
        throw new InvalidOperationException (message);
      }
      _mixins.Add (newMixin.Type, newMixin);
    }

    public MixinDefinition GetMixin (Type type)
    {
      return HasMixin (type) ? _mixins[type] : null;
    }

    public IEnumerable<InterfaceIntroductionDefinition> IntroducedInterfaces
    {
      get { return _introducedInterfaces.Values; }
    }

    public bool HasIntroducedInterface (Type type)
    {
      return _introducedInterfaces.ContainsKey (type);
    }

    public void AddIntroducedInterface (InterfaceIntroductionDefinition introducedInterface)
    {
      Debug.Assert (HasMixin(introducedInterface.Implementer.Type));
      if (HasIntroducedInterface (introducedInterface.Type))
      {
        string message = string.Format ("Duplicate interface {0} introduced on class {1}; implementers: {2} and {3}.", introducedInterface.FullName,
          FullName, introducedInterface.Implementer.FullName, GetIntroducedInterface (introducedInterface.Type).Implementer.FullName);
        throw new InvalidOperationException (message);
      }

      _introducedInterfaces.Add (introducedInterface.Type, introducedInterface);
    }

    public InterfaceIntroductionDefinition GetIntroducedInterface (Type type)
    {
      return HasIntroducedInterface (type) ? _introducedInterfaces[type] : null;
    }

    public IEnumerable<Type> RequiredFaceInterfaces
    {
      get { return _requiredFaceInterfaces.Values; }
    }

    public bool HasRequiredFaceInterface (Type type)
    {
      return _requiredFaceInterfaces.ContainsKey (type);
    }

    public void AddRequiredFaceInterface (Type requiredFaceInterface)
    {
      if (HasRequiredFaceInterface (requiredFaceInterface))
      {
        string message = string.Format ("Face interface {0} already required by class {1}.", requiredFaceInterface.FullName, FullName);
        throw new InvalidOperationException (message);
      }

      _requiredFaceInterfaces.Add (requiredFaceInterface, requiredFaceInterface);
    }

    public Type GetRequiredFaceInterface (Type type)
    {
      return HasRequiredFaceInterface (type) ? _requiredFaceInterfaces[type] : null;
    }
  }
}
