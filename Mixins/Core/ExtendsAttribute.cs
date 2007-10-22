using System;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Indicates that a mixin extends a specific class, providing some part of its functionality or public interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration built via <see cref="ApplicationContextBuilder.BuildDefaultContext"/>,
  /// which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// Although the attribute itself is not inherited, its semantics in mixin configuration are: If a base class is configured to be mixed with a
  /// mixin type M by means of the <see cref="ExtendsAttribute"/>, this configuration setting is inherited by each of its (direct and indirect) subclasses.
  /// The subclasses will therefore also be mixed with the same mixin type M unless a second mixin M2 derived from M is applied to the subclass, thus
  /// overriding the inherited configuration. If M is configured for both base class and subclass, the base class configuration is ignored.
  /// </para>
  /// <para>
  /// This attribute can be applied to the same mixin class multiple times if it extends multiple target classes. It should not however be used to
  /// apply the same mixin multiple times to the same target class.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class ExtendsAttribute : Attribute
  {
    private readonly Type _targetType;

    private Type[] _additionalDependencies = Type.EmptyTypes;
    private Type[] _mixinTypeArguments = Type.EmptyTypes;
    private Type[] _suppressedMixins = Type.EmptyTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendsAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type extended by this mixin.</param>
    public ExtendsAttribute (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      _targetType = targetType;
    }

    /// <summary>
    /// Gets the target type the mixin class applies to.
    /// </summary>
    /// <value>The target type the mixin class applies to.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets or sets additional explicit base call dependencies for this mixin type when applied to the given target type. This can be used to
    /// establish an ordering when combining unrelated mixins on a class which override the same methods.
    /// </summary>
    /// <value>The additional dependencies of the mixin. The validity of the dependency types is not checked until the configuration is built.</value>
    /// <exception cref="ArgumentNullException">The <paramref name="value"/> argument is <see langword="null"/>.</exception>
    public Type[] AdditionalDependencies
    {
      get { return _additionalDependencies; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _additionalDependencies = value;
      }
    }

    /// <summary>
    /// Gets or sets the generic type arguments to be used when applying a generic mixin to the given target type. This is useful when the
    /// <see cref="ExtendsAttribute"/> is to be applied to a generic mixin class, but the default generic type specialization algorithm of the
    /// mixin engine does not give the desired results.
    /// </summary>
    /// <value>The generic type arguments to close the generic mixin type with.</value>
    /// <remarks>If this attribute is applied to a non-generic mixin class or if the types supplied don't match the mixin's generic parameters,
    /// a <see cref="ConfigurationException"/> is thrown when the mixin configuration is analyzed.</remarks>
    public Type[] MixinTypeArguments
    {
      get { return _mixinTypeArguments; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _mixinTypeArguments = value;
      }
    }

    /// <summary>
    /// Gets or sets the mixins suppressed by this mixin.
    /// </summary>
    /// <value>The mixins suppressed by this mixin.</value>
    /// <remarks>Use this attribute to actively remove another mixin from this mixin's target type. The list of suppressed mixins cannot contain 
    /// this mixin itself, but it can contain mixins which themselves suppress this mixin. Such circular suppressions result in both mixins being
    /// removed from the configuration.</remarks>
    public Type[] SuppressedMixins
    {
      get { return _suppressedMixins; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _suppressedMixins = value;
      }
    }
  }
}
