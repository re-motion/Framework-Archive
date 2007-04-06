using System;

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
      return baseClass.RequiredBaseCallTypes[type];
    }

    protected override RequiredBaseCallTypeDefinition CreateRequirement (BaseClassDefinition baseClass, Type type)
    {
      return new RequiredBaseCallTypeDefinition (baseClass, type);
    }

    protected override void AddRequirement (BaseClassDefinition baseClass, RequiredBaseCallTypeDefinition requirement)
    {
      baseClass.RequiredBaseCallTypes.Add (requirement);
    }

    protected override BaseDependencyDefinition CreateDependency (RequiredBaseCallTypeDefinition requirement, MixinDefinition mixin, BaseDependencyDefinition aggregator)
    {
      return new BaseDependencyDefinition (requirement, mixin, aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, BaseDependencyDefinition dependency)
    {
      mixin.BaseDependencies.Add (dependency);
    }
  }
}
