using System;
using Rubicon.Utilities;

namespace Mixins
{
  /// <summary>
  /// Indicates that a class extends a specific class, extending some part of its functionality or public interface as a mixin.
  /// </summary>
  /// <remarks>
  /// This attribute is not inherited (i.e. if a mixin extends a base class, subclasses of the mixin do not automatically extend the same base class)
  /// and can be applied multiple times if a mixin extends on multiple classes.
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)] // ExtendsAttribute is not inherited!
  public class ExtendsAttribute : Attribute
  {
    private Type _targetType;

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
  }
}
