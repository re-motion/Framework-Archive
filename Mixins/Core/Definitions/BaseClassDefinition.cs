using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rubicon.Mixins.Utilities;
using Rubicon.Utilities;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{Type}")]
  public class BaseClassDefinition : ClassDefinitionBase, IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<Type, MixinDefinition> Mixins =
        new UniqueDefinitionCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes =
        new UniqueDefinitionCollection<Type, RequiredFaceTypeDefinition> (delegate (RequiredFaceTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes =
        new UniqueDefinitionCollection<Type, RequiredBaseCallTypeDefinition> (delegate (RequiredBaseCallTypeDefinition t) { return t.Type; });
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> IntroducedInterfaces =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });

    private MixinTypeInstantiator _mixinTypeInstantiator;
    private ClassContext _configurationContext;

    public BaseClassDefinition (ClassContext configurationContext)
        : base (configurationContext.Type)
    {
      ArgumentUtility.CheckNotNull ("configurationContext", configurationContext);

      _configurationContext = configurationContext;
      _mixinTypeInstantiator = new MixinTypeInstantiator (configurationContext.Type);
    }

    public ClassContext ConfigurationContext
    {
      get { return _configurationContext; }
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
      get { return null; }
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
      return Mixins.ContainsKey (realType);
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
