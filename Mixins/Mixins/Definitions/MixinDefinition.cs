using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Definitions
{
  public class MixinDefinition : ClassDefinition, IVisitableDefinition
  {
    public readonly DefinitionItemCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions =
        new DefinitionItemCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    public readonly DefinitionItemCollection<MethodInfo, MethodDefinition> InitializationMethods =
        new DefinitionItemCollection<MethodInfo, MethodDefinition> (delegate (MethodDefinition m) { return m.MethodInfo; });
    public readonly DefinitionItemCollection<Type, ThisDependencyDefinition> ThisDependencies =
        new DefinitionItemCollection<Type, ThisDependencyDefinition> (delegate (ThisDependencyDefinition d) { return d.RequiredType.Type; });
    public readonly DefinitionItemCollection<Type, BaseDependencyDefinition> BaseDependencies =
        new DefinitionItemCollection<Type, BaseDependencyDefinition> (delegate (BaseDependencyDefinition d) { return d.RequiredType.Type; });

    private BaseClassDefinition _baseClass;
    

    public MixinDefinition (Type type, BaseClassDefinition baseClass)
        : base (type)
    {
      _baseClass = baseClass;
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public IEnumerable<MemberDefinition> Overrides
    {
      get
      {
        foreach (MemberDefinition member in Members)
        {
          if (member.Base != null)
          {
            yield return member;
          }
        }
      }
    }

    public override IVisitableDefinition Parent
    {
      get { return BaseClass; }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);

      Members.Accept (visitor);
      InterfaceIntroductions.Accept (visitor);
      InitializationMethods.Accept (visitor);
      ThisDependencies.Accept (visitor);
      BaseDependencies.Accept (visitor);
    }
  }
}
