using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public abstract class DependencyDefinitionBuilderBase
  {
    private readonly MixinDefinition _mixin;

    public DependencyDefinitionBuilderBase(MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      _mixin = mixin;
    }

    protected abstract RequirementDefinitionBase GetRequirement (Type type, BaseClassDefinition baseClass);
    protected abstract RequirementDefinitionBase CreateRequirement (Type type, MixinDefinition mixin);
    protected abstract void AddRequirement (RequirementDefinitionBase requirement, BaseClassDefinition baseClass);
    protected abstract DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, MixinDefinition mixin, DependencyDefinitionBase aggregator);
    protected abstract void AddDependency (MixinDefinition mixin, DependencyDefinitionBase dependency);

    public void Apply (IEnumerable<Type> dependencyTypes)
    {
      ArgumentUtility.CheckNotNull ("dependencyTypes", dependencyTypes);

      foreach (Type type in dependencyTypes)
      {
        if (!type.Equals (typeof (object))) // dependencies to System.Object are always fulfilled and not explicitly added to the configuration
        {
          DependencyDefinitionBase dependency = BuildDependency (type, null);
          AddDependency (_mixin, dependency);
        }
      }
    }

    private DependencyDefinitionBase BuildDependency(Type type, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      RequirementDefinitionBase requirement = GetRequirement (type, _mixin.BaseClass);
      if (requirement == null)
      {
        requirement = CreateRequirement (type, _mixin);

        RequiredMethodDefinitionBuilder requiredMethodBuilder = new RequiredMethodDefinitionBuilder (requirement);
        requiredMethodBuilder.Apply ();

        AddRequirement(requirement, _mixin.BaseClass);
      }
      DependencyDefinitionBase dependency = CreateDependency (requirement, _mixin, aggregator);
      requirement.RequiringDependencies.Add (dependency);
      CheckForAggregate (dependency);
      return dependency;
    }

    private void CheckForAggregate (DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (dependency.RequiredType.IsAggregatorInterface)
      {
        foreach (Type type in dependency.RequiredType.Type.GetInterfaces ())
        {
          DependencyDefinitionBase innerDependency = BuildDependency (type, dependency);
          dependency.AggregatedDependencies.Add (innerDependency);
        }
      }
    }
  }
}
