using System;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Defines that a mixin is not applied to a specific class, even when it is explicitly or implicitly configured for that class via
  /// the declarative configuration attributes <see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, and <see cref="MixAttribute"/>.
  /// </summary>
  /// <remarks>
  /// Use this attribute to exclude a target class inherited from a mixin's base class. This attribute is not inherited, so the target class
  /// exclusion will only work for the exact mixin to which the attribute is applied.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class IgnoresClassAttribute : Attribute
  {
    private readonly Type _classToIgnore;

    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoresClassAttribute"/> class, specifying the class to be ignored by this mixin.
    /// </summary>
    /// <param name="classToIgnore">The class to be ignored in declarative configuration. Subclasses of this class
    /// will not inherit the mixin either.</param>
    public IgnoresClassAttribute (Type classToIgnore)
    {
      _classToIgnore = classToIgnore;
    }

    /// <summary>
    /// Gets the class to be ignored by this mixin.
    /// </summary>
    /// <value>The class to be ignored.</value>
    public Type ClassToIgnore
    {
      get { return _classToIgnore; }
    }
  }
}