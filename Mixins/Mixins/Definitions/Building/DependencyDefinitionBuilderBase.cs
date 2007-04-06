using System;
using System.Collections.Generic;

namespace Mixins.Definitions.Building
{
  public abstract class DependencyDefinitionBuilderBase<TRequirement, TDependency>
      where TDependency : DependencyDefinitionBase<TRequirement, TDependency>
      where TRequirement : RequirementDefinitionBase<TRequirement, TDependency>
  {
    private readonly MixinDefinition _mixin;

    public DependencyDefinitionBuilderBase(MixinDefinition mixin)
    {
      _mixin = mixin;
    }

    protected abstract TRequirement GetRequirement (Type type, BaseClassDefinition baseClass);
    protected abstract TRequirement CreateRequirement (BaseClassDefinition baseClass, Type type);
    protected abstract void AddRequirement (BaseClassDefinition baseClass, TRequirement requirement);
    protected abstract TDependency CreateDependency (TRequirement requirement, MixinDefinition mixin, TDependency aggregator);
    protected abstract void AddDependency (MixinDefinition mixin, TDependency dependency);

    public void Apply (IEnumerable<Type> dependencyTypes)
    {
      foreach (Type type in dependencyTypes)
      {
        TDependency dependency = BuildDependency(type, null);
        AddDependency (_mixin, dependency);
      }
    }

    private TDependency BuildDependency(Type type, TDependency aggregator)
    {
      TRequirement requirement = GetRequirement (type, _mixin.BaseClass);
      if (requirement == null)
      {
        requirement = CreateRequirement (_mixin.BaseClass, type);
        AddRequirement(_mixin.BaseClass, requirement);
      }
      TDependency dependency = CreateDependency (requirement, _mixin, aggregator);
      requirement.RequiringDependencies.Add (dependency);
      CheckForAggregate (dependency);
      return dependency;
    }

    private void CheckForAggregate (TDependency dependency)
    {
      if (dependency.RequiredType.IsEmptyInterface)
      {
        foreach (Type type in dependency.RequiredType.Type.GetInterfaces ())
        {
          TDependency innerDependency = BuildDependency (type, dependency);
          dependency.AggregatedDependencies.Add (innerDependency);
        }
      }
    }
  }
}
