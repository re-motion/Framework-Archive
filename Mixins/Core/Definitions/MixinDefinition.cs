using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{Type}, BaseClass = {BaseClass.Type}")]
  public class MixinDefinition : ClassDefinitionBase, IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (delegate (InterfaceIntroductionDefinition i) { return i.Type; });
    public readonly UniqueDefinitionCollection<Type, ThisDependencyDefinition> ThisDependencies =
        new UniqueDefinitionCollection<Type, ThisDependencyDefinition> (delegate (ThisDependencyDefinition d) { return d.RequiredType.Type; });
    public readonly UniqueDefinitionCollection<Type, BaseDependencyDefinition> BaseDependencies =
        new UniqueDefinitionCollection<Type, BaseDependencyDefinition> (delegate (BaseDependencyDefinition d) { return d.RequiredType.Type; });

    private readonly BaseClassDefinition _baseClass;
    private int _mixinIndex;


    public MixinDefinition (Type type, BaseClassDefinition baseClass)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      _baseClass = baseClass;
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public IEnumerable<MemberDefinition> GetAllOverrides()
    {
      foreach (MemberDefinition member in GetAllMembers())
      {
        if (member.BaseAsMember != null)
          yield return member;
      }
    }

    public bool HasOverriddenMembers ()
    {
      foreach (MemberDefinition member in GetAllMembers ())
      {
        if (member.Overrides.Count > 0)
          return true;
      }
      return false;
    }

    public override IVisitableDefinition Parent
    {
      get { return BaseClass; }
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
      ThisDependencies.Accept (visitor);
      BaseDependencies.Accept (visitor);
    }
  }
}
