using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
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

    protected override RequiredBaseCallTypeDefinition CreateRequirement (Type type, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("mixin", mixin);

      Assertion.Assert (type != typeof (object), "This method will not be called for typeof (object).");

      if (!type.IsInterface)
      {
        string message = string.Format ("Base call dependencies must be interfaces (or System.Object), but mixin {0} (on class {1} has a dependency "
            + "on a class: {2}.", mixin.FullName, mixin.BaseClass.FullName, type.FullName);
        throw new ConfigurationException (message);
      }

      return new RequiredBaseCallTypeDefinition (mixin.BaseClass, type);
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
      if (!mixin.BaseDependencies.ContainsKey (dependency.RequiredType.Type))
        mixin.BaseDependencies.Add (dependency);
    }
  }
}
