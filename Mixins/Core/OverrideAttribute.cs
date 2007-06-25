using System;

namespace Mixins
{
  /// <summary>
  /// When applied to a mixin method, indicates that it overrides a virtual method of the mixin's target class.
  /// When applied to a target class method, indicates that it overrides a virtual or abstract method of one of the mixins combined with the class.
  /// </summary>
  /// <remarks>
  /// <para>
  /// An overriding method and its base method must both be public or protected, and they must have the same name and signature. If an overriding
  /// method would apply to multiple mixin methods, this is regarded as a configuration error.
  /// </para>
  /// <para>
  /// This attribute is inherited (i.e. if the overriding method is replaced in a subclass, the subclass' method is now the overriding method) and
  /// can only be applied once per method.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
  public class OverrideAttribute : Attribute
  {
  }
}
