using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public class ThisDependencyDefinitionBuilder : DependencyDefinitionBuilderBase
  {
    public ThisDependencyDefinitionBuilder (MixinDefinition mixin)
        : base (mixin)
    {
    }

    protected override RequirementDefinitionBase GetRequirement (Type type, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      return baseClass.RequiredFaceTypes[type];
    }

    protected override RequirementDefinitionBase CreateRequirement (Type type, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new RequiredFaceTypeDefinition (mixin.BaseClass, type);
    }

    protected override void AddRequirement (RequirementDefinitionBase requirement, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      baseClass.RequiredFaceTypes.Add ((RequiredFaceTypeDefinition) requirement);
    }

    protected override DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, MixinDefinition mixin, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new ThisDependencyDefinition ((RequiredFaceTypeDefinition) requirement, mixin, (ThisDependencyDefinition) aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (!mixin.ThisDependencies.ContainsKey (dependency.RequiredType.Type))
        mixin.ThisDependencies.Add ((ThisDependencyDefinition) dependency);
    }
  }
}
