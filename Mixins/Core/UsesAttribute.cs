using System;
using Rubicon.Mixins.Context;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Indicates that a class integrates a mixin to implement some part of its functionality or public interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration built via <see cref="ApplicationContextBuilder.BuildDefaultContext"/>,
  /// which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// Although the attribute itself is not inherited, its semantics in mixin configuration are: If a base class is configured to be mixed with a
  /// mixin type M by means of the <see cref="UsesAttribute"/>, this configuration setting is inherited by each of its (direct and indirect) subclasses.
  /// The subclasses will therefore also be mixed with the same mixin type M unless a second mixin M2 derived from M is applied to the subclass, thus
  /// overriding the inherited configuration. If M is configured for both base class and subclass, the base class configuration is ignored.
  /// </para>
  /// <para>
  /// This attribute can be applied to the same target class multiple times if a class depends on multiple mixins, but it should not be used to
  /// apply the same mixin multiple times to the same target class.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class UsesAttribute : Attribute
  {
    private Type _mixinType;
    private Type[] _additionalDependencies = Type.EmptyTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsesAttribute"/> class.
    /// </summary>
    /// <param name="mixinType">The mixin type the class depends on.</param>
    public UsesAttribute (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      _mixinType = mixinType;
    }

    /// <summary>
    /// Gets the mixin type the class depends on.
    /// </summary>
    /// <value>The mixin type the class depends on.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    /// <summary>
    /// Gets or sets additional explicit base call dependencies for the applied mixin type. This can be used to establish an ordering when
    /// combining unrelated mixins on a class which override the same methods.
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
  }
}
