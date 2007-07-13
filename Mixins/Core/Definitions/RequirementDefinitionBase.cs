using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rubicon.Mixins.Utilities;
using Rubicon.Collections;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{Type}, BaseClass = {BaseClass.Type}")]
  public abstract class RequirementDefinitionBase : IVisitableDefinition
  {
    public readonly UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> RequiringDependencies =
        new UniqueDefinitionCollection<DependencyDefinitionBase, DependencyDefinitionBase> (delegate (DependencyDefinitionBase d) { return d; });

    public readonly UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> Methods =
        new UniqueDefinitionCollection<MethodInfo, RequiredMethodDefinition> (delegate (RequiredMethodDefinition m) { return m.InterfaceMethod; });

    private BaseClassDefinition _baseClass;
    private Type _type;

    public RequirementDefinitionBase(BaseClassDefinition baseClass, Type type)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      ArgumentUtility.CheckNotNull ("type", type);

      _baseClass = baseClass;
      _type = type;
    }

    public BaseClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return BaseClass; }
    }

    public bool IsEmptyInterface
    {
      get { return Type.IsInterface && Type.GetMethods().Length == 0; }
    }

    public bool IsAggregatorInterface
    {
      get { return IsEmptyInterface && Type.GetInterfaces().Length != 0; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ConcreteAccept (visitor);
      Methods.Accept (visitor);
    }

    protected abstract void ConcreteAccept (IDefinitionVisitor visitor);

    public IEnumerable<MixinDefinition> FindRequiringMixins()
    {
      Set<MixinDefinition> mixins = new Set<MixinDefinition>();
      foreach (DependencyDefinitionBase dependency in RequiringDependencies)
        mixins.Add (dependency.Depender);
      return mixins;
    }
  }
}