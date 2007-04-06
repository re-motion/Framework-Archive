using System;
using System.Collections.Generic;

namespace Mixins.Definitions
{
  public abstract class RequirementDefinitionBase<TSelf, TDependency>: IVisitableDefinition
      where TDependency : DependencyDefinitionBase<TSelf, TDependency>
      where TSelf : RequirementDefinitionBase<TSelf, TDependency>
  {
    public readonly DefinitionItemCollection<TDependency, TDependency> RequiringDependencies =
        new DefinitionItemCollection<TDependency,TDependency> (delegate (TDependency d) { return d; });

    private BaseClassDefinition _baseClass;
    private Type _type;

    public RequirementDefinitionBase(BaseClassDefinition baseClass, Type type)
    {
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

    public abstract void Accept (IDefinitionVisitor visitor);

    public IEnumerable<MixinDefinition> FindRequiringMixins()
    {
      return FindRequiringMixins (RequiringDependencies);
    }

    private IEnumerable<MixinDefinition> FindRequiringMixins (IEnumerable<TDependency> requiringDependencies)
    {
      Dictionary<MixinDefinition, MixinDefinition> mixins = new Dictionary<MixinDefinition, MixinDefinition> (); // used as set
      foreach (TDependency dependency in requiringDependencies)
      {
        if (!mixins.ContainsKey (dependency.Depender))
        {
          mixins.Add (dependency.Depender, dependency.Depender);
        }
        foreach (MixinDefinition mixin in FindRequiringMixins (dependency.AggregatedDependencies))
        {
          if (!mixins.ContainsKey (mixin))
          {
            mixins.Add (mixin, mixin);
          }
        }
      }
      return mixins.Keys;
    }
  }
}