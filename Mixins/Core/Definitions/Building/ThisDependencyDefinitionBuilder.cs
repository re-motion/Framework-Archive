using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public class ThisDependencyDefinitionBuilder : DependencyDefinitionBuilderBase<RequiredFaceTypeDefinition, ThisDependencyDefinition>
  {
    public ThisDependencyDefinitionBuilder (MixinDefinition mixin)
        : base (mixin)
    {
    }

    protected override RequiredFaceTypeDefinition GetRequirement (Type type, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      return baseClass.RequiredFaceTypes[type];
    }

    protected override RequiredFaceTypeDefinition CreateRequirement (Type type, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new RequiredFaceTypeDefinition (mixin.BaseClass, type);
    }

    protected override void AddRequirement (RequiredFaceTypeDefinition requirement, BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);

      baseClass.RequiredFaceTypes.Add (requirement);
    }

    protected override ThisDependencyDefinition CreateDependency (RequiredFaceTypeDefinition requirement, MixinDefinition mixin, ThisDependencyDefinition aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      return new ThisDependencyDefinition (requirement, mixin, aggregator);
    }

    protected override void AddDependency (MixinDefinition mixin, ThisDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (!mixin.ThisDependencies.HasItem (dependency.RequiredType.Type))
        mixin.ThisDependencies.Add (dependency);
    }
  }
}
