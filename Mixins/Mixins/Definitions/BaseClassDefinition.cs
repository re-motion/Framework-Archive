using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
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

    [NonSerialized]
    private ApplicationDefinition _application;

    public BaseClassDefinition (ApplicationDefinition application, Type type)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("application", application);
      ArgumentUtility.CheckNotNull ("type", type);

      _application = application;
    }

    // Will be null if the class definition is deserialized.
    public ApplicationDefinition Application
    {
      get { return _application; }
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

      Members.Accept (visitor);
      Mixins.Accept (visitor);
      RequiredFaceTypes.Accept (visitor);
      RequiredBaseCallTypes.Accept (visitor);
    }
  }
}
