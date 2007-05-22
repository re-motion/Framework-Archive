using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Mixins.Utilities;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{Type}")]
  public class BaseClassDefinition : ClassDefinition, IVisitableDefinition
  {
    public readonly DefinitionItemCollection<Type, MixinDefinition> Mixins =
        new DefinitionItemCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });
    public readonly DefinitionItemCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes =
        new DefinitionItemCollection<Type, RequiredFaceTypeDefinition> (delegate (RequiredFaceTypeDefinition t) { return t.Type; });
    public readonly DefinitionItemCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes =
        new DefinitionItemCollection<Type, RequiredBaseCallTypeDefinition> (delegate (RequiredBaseCallTypeDefinition t) { return t.Type; });
    public readonly DefinitionItemCollection<Type, InterfaceIntroductionDefinition> IntroducedInterfaces =
        new DefinitionItemCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });

    private MixinTypeInstantiator _mixinTypeInstantiator;

    [NonSerialized]
    private ApplicationDefinition _application;

    public BaseClassDefinition (ApplicationDefinition application, Type type)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("application", application);
      ArgumentUtility.CheckNotNull ("type", type);

      _application = application;
      _mixinTypeInstantiator = new MixinTypeInstantiator (type);
    }

    // Will be null if the class definition is deserialized.
    public ApplicationDefinition Application
    {
      get { return _application; }
    }

    internal MixinTypeInstantiator MixinTypeInstantiator
    {
      get { return _mixinTypeInstantiator; }
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public override IVisitableDefinition Parent
    {
      get { return Application; }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
      
      base.AcceptForChildren (visitor);
      
      Mixins.Accept (visitor);
      RequiredFaceTypes.Accept (visitor);
      RequiredBaseCallTypes.Accept (visitor);
    }

    public bool HasMixinWithConfiguredType(Type configuredType)
    {
      Type realType = _mixinTypeInstantiator.GetConcreteMixinType (configuredType);
      return Mixins.HasItem (realType);
    }

    public MixinDefinition GetMixinByConfiguredType(Type configuredType)
    {
      Type realType = _mixinTypeInstantiator.GetConcreteMixinType (configuredType);
      return Mixins[realType];
    }

    public IEnumerable<MethodDefinition> GetAllMixinMethods()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (MethodDefinition method in mixin.Methods)
          yield return method;
    }

    public IEnumerable<PropertyDefinition> GetAllMixinProperties ()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (PropertyDefinition property in mixin.Properties)
          yield return property;
    }

    public IEnumerable<EventDefinition> GetAllMixinEvents ()
    {
      foreach (MixinDefinition mixin in Mixins)
        foreach (EventDefinition eventDefinition in mixin.Events)
          yield return eventDefinition;
    }
  }
}
