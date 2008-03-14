using System;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Defines that a specific mixin is not applied to a class, even when it is explicitly or implicitly configured for that class via
  /// the declarative configuration attributes <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, and <see cref="MixAttribute"/>.
  /// </summary>
  /// <remarks>
  /// Use this attribute to exclude a mixin that is configured to be applied to a base class. This attribute is not inherited, so the mixin
  /// exclusion will only work for the exact mixin to which the attribute is applied.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class IgnoresMixinAttribute : Attribute
  {
    private readonly Type _mixinToIgnore;

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoresClassAttribute"/> class, specifying the mixin to be ignored by this class.
    /// </summary>
    /// <param name="mixinToIgnore">The mixin to be ignored in declarative configuration. Subclasses of this class will not inherit the mixin either.</param>
    public IgnoresMixinAttribute (Type mixinToIgnore)
    {
      _mixinToIgnore = mixinToIgnore;
    }

    /// <summary>
    /// Gets the mixin to be ignored by this class.
    /// </summary>
    /// <value>The mixin to be ignored.</value>
    public Type MixinToIgnore
    {
      get { return _mixinToIgnore; }
    }
  }
}