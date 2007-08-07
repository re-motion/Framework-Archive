using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{Type}, TargetClass = {TargetClass.Type}")]
  public class MixinDefinition : ClassDefinitionBase, IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    public readonly UniqueDefinitionCollection<Type, SuppressedInterfaceIntroductionDefinition> SuppressedInterfaceIntroductions =
        new UniqueDefinitionCollection<Type, SuppressedInterfaceIntroductionDefinition> (
            delegate (SuppressedInterfaceIntroductionDefinition i) { return i.Type; });
    public readonly UniqueDefinitionCollection<Type, ThisDependencyDefinition> ThisDependencies =
        new UniqueDefinitionCollection<Type, ThisDependencyDefinition> (delegate (ThisDependencyDefinition d) { return d.RequiredType.Type; });
    public readonly UniqueDefinitionCollection<Type, BaseDependencyDefinition> BaseDependencies =
        new UniqueDefinitionCollection<Type, BaseDependencyDefinition> (delegate (BaseDependencyDefinition d) { return d.RequiredType.Type; });

    private readonly TargetClassDefinition _targetClass;
    private int _mixinIndex;


    public MixinDefinition (Type type, TargetClassDefinition targetClass)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      _targetClass = targetClass;
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public IEnumerable<MemberDefinition> GetAllOverrides()
    {
      foreach (MemberDefinition member in GetAllMembers())
      {
        if (member.BaseAsMember != null)
          yield return member;
      }
    }

    public override IVisitableDefinition Parent
    {
      get { return TargetClass; }
    }

    public int MixinIndex
    {
      get { return _mixinIndex; }
      internal set { _mixinIndex = value; }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);

      InterfaceIntroductions.Accept (visitor);
      SuppressedInterfaceIntroductions.Accept (visitor);
      ThisDependencies.Accept (visitor);
      BaseDependencies.Accept (visitor);
    }
  }
}
