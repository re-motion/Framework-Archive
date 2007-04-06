using System;

namespace Mixins.Definitions.Building
{
  public class ThisDependencyDefinitionBuilder : DependencyDefinitionBuilderBase<RequiredFaceTypeDefinition, ThisDependencyDefinition>
  {
    public ThisDependencyDefinitionBuilder (MixinDefinition mixin)
        : base (mixin)
    {
    }

    protected override RequiredFaceTypeDefinition GetRequirement (Type type, BaseClassDefinition baseClass)
    {
      return baseClass.RequiredFaceTypes[type];
    }

    protected override RequiredFaceTypeDefinition CreateRequirement (BaseClassDefinition baseClass, Type type)
    {
      return new RequiredFaceTypeDefinition (baseClass, type);
    }

    protected override void AddRequirement (BaseClassDefinition baseClass, RequiredFaceTypeDefinition requirement)
    {
      baseClass.RequiredFaceTypes.Add (requirement);
    }

    protected override ThisDependencyDefinition CreateDependency (RequiredFaceTypeDefinition requirement, MixinDefinition mixin, ThisDependencyDefinition aggregator)
    {
      return new ThisDependencyDefinition (requirement, mixin, aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, ThisDependencyDefinition dependency)
    {
      mixin.ThisDependencies.Add (dependency);
    }
  }
}
