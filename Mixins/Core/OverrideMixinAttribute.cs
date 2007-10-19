using System;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Indicates that a target class member overrides a virtual or abstract member of one of the mixins combined with the class.
  /// </summary>
  /// <remarks>
  /// <para>
  /// An overriding member and its base member must both be public or protected, and they must have the same name and signature. If an overriding
  /// member would apply to multiple mixin members, this is regarded as a configuration error.
  /// </para>
  /// <para>
  /// This attribute is inherited (i.e. if the overriding member is replaced in a subclass, the subclass' member is now the overriding member) and
  /// can only be applied once per member.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
  public class OverrideMixinAttribute : Attribute
  {
  }
}
