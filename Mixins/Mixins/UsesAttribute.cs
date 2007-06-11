using System;
using Rubicon.Utilities;

namespace Mixins
{
  /// <summary>
  /// Indicates that a class employs a mixin to implement some part of its functionality or public interface.
  /// </summary>
  /// <remarks>
  /// This attribute is inherited (i.e. if a base class depends on a mixin, its subclasses also depend on the same mixin) and can be applied
  /// multiple times if a class depends on multiple mixins.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)] // UsesMixinAttribute is inherited
  public class UsesAttribute : Attribute
  {
    private Type _mixinType;

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
  }
}
