using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
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
        if (member.GetOverridesAsMemberDefinitions ().GetEnumerator ().MoveNext ())
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

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);

      base.AcceptForChildren (visitor);

      InterfaceIntroductions.Accept (visitor);
      InitializationMethods.Accept (visitor);
      ThisDependencies.Accept (visitor);
      BaseDependencies.Accept (visitor);
    }
  }
}
