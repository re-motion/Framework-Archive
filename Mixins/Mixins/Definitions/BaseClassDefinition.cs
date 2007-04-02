using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Mixins.Definitions
{
  public class BaseClassDefinition : ClassDefinition
  {
    public readonly DefinitionItemCollection<Type, MixinDefinition> Mixins =
        new DefinitionItemCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });
    public readonly DefinitionItemCollection<Type, InterfaceIntroductionDefinition> IntroducedInterfaces =
        new DefinitionItemCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    public readonly DefinitionItemCollection<Type, Type> RequiredFaceInterfaces =
        new DefinitionItemCollection<Type, Type> (delegate (Type t) { return t; });

    public BaseClassDefinition (Type type)
        : base (type)
    {
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }
  }
}
