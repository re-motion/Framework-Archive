using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Mixins.Definitions
{
  public class BaseClassDefinition : ClassDefinition, IVisitableDefinition
  {
    public readonly DefinitionItemCollection<Type, MixinDefinition> Mixins =
        new DefinitionItemCollection<Type, MixinDefinition> (delegate (MixinDefinition m) { return m.Type; });
    /*public readonly DefinitionItemCollection<Type, InterfaceIntroductionDefinition> IntroducedInterfaces =
        new DefinitionItemCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });*/
    public readonly DefinitionItemCollection<Type, RequiredFaceTypeDefinition> RequiredFaceTypes =
        new DefinitionItemCollection<Type, RequiredFaceTypeDefinition> (delegate (RequiredFaceTypeDefinition t) { return t.Type; });
    public readonly DefinitionItemCollection<Type, RequiredBaseCallTypeDefinition> RequiredBaseCallTypes =
        new DefinitionItemCollection<Type, RequiredBaseCallTypeDefinition> (delegate (RequiredBaseCallTypeDefinition t) { return t.Type; });

    public BaseClassDefinition (Type type)
        : base (type)
    {
    }

    public bool IsInterface
    {
      get { return Type.IsInterface; }
    }

    public IEnumerable<InterfaceIntroductionDefinition> IntroducedInterfaces
    {
      get
      {
        foreach (MixinDefinition mixin in Mixins)
        {
          foreach (InterfaceIntroductionDefinition interfaceIntroduction in mixin.InterfaceIntroductions)
          {
            yield return interfaceIntroduction;
          }
        }
      }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);

      Members.Accept (visitor);
      Mixins.Accept (visitor);
      RequiredFaceTypes.Accept (visitor);
      RequiredBaseCallTypes.Accept (visitor);
    }
  }
}
