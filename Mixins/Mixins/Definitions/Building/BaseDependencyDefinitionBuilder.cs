using System;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class BaseDependencyDefinitionBuilder : DependencyDefinitionBuilderBase<RequiredBaseCallTypeDefinition, BaseDependencyDefinition>
  {
    public BaseDependencyDefinitionBuilder (MixinDefinition mixin)
        : base (mixin)
    {
    }

    protected override RequiredBaseCallTypeDefinition GetRequirement (Type type, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      return baseClass.RequiredBaseCallTypes[type];
    }

    protected override RequiredBaseCallTypeDefinition CreateRequirement (Type type, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      return new RequiredBaseCallTypeDefinition (baseClass, type);
    }

    protected override void AddRequirement (RequiredBaseCallTypeDefinition requirement, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      baseClass.RequiredBaseCallTypes.Add (requirement);
    }

    protected override BaseDependencyDefinition CreateDependency (RequiredBaseCallTypeDefinition requirement, MixinDefinition mixin,
        BaseDependencyDefinition aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new BaseDependencyDefinition (requirement, mixin, aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, BaseDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      mixin.BaseDependencies.Add (dependency);
    }
  }
}
