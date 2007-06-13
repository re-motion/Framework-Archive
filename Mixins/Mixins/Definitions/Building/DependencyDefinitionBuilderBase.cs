using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public abstract class DependencyDefinitionBuilderBase<TRequirement, TDependency>
      where TDependency : DependencyDefinitionBase<TRequirement, TDependency>
      where TRequirement : RequirementDefinitionBase<TRequirement, TDependency>
  {
    private readonly MixinDefinition _mixin;

    public DependencyDefinitionBuilderBase(MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      _mixin = mixin;
    }

    protected abstract TRequirement GetRequirement (Type type, BaseClassDefinition baseClass);
    protected abstract TRequirement CreateRequirement (Type type, MixinDefinition mixin);
    protected abstract void AddRequirement (TRequirement requirement, BaseClassDefinition baseClass);
    protected abstract TDependency CreateDependency (TRequirement requirement, MixinDefinition mixin, TDependency aggregator);
    protected abstract void AddDependency (MixinDefinition mixin, TDependency dependency);

    public void Apply (IEnumerable<Type> dependencyTypes)
    {
      ArgumentUtility.CheckNotNull ("dependencyTypes", dependencyTypes);

      foreach (Type type in dependencyTypes)
      {
        if (!type.Equals (typeof (object))) // dependencies to System.Object are always fulfilled and not explicitly added to the configuration
        {
          TDependency dependency = BuildDependency (type, null);
          AddDependency (_mixin, dependency);
        }
      }
    }

    private TDependency BuildDependency(Type type, TDependency aggregator)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TRequirement requirement = GetRequirement (type, _mixin.BaseClass);
      if (requirement == null)
      {
        requirement = CreateRequirement (type, _mixin);
        AddRequirement(requirement, _mixin.BaseClass);
      }
      TDependency dependency = CreateDependency (requirement, _mixin, aggregator);
      requirement.RequiringDependencies.Add (dependency);
      CheckForAggregate (dependency);
      return dependency;
    }

    private void CheckForAggregate (TDependency dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (dependency.RequiredType.IsAggregatorInterface)
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
