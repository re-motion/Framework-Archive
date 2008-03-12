using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  public class MixinRelationshipAttribute : Attribute
  {
    private Type[] _additionalDependencies = Type.EmptyTypes;
    private Type[] _suppressedMixins = Type.EmptyTypes;

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

    /// <summary>
    /// Gets or sets the mixins suppressed by the applied mixin.
    /// </summary>
    /// <value>The mixins suppressed by the applied mixins.</value>
    /// <remarks>Use this attribute to actively remove a mixin from the attribute's target type. The list of suppressed mixins cannot contain 
    /// the applied mixin itself, but it can contain mixins which themselves suppress this mixin. Such circular suppressions result in both mixins
    /// being removed from the configuration.</remarks>
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