using System;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins
{
  /// <summary>
  /// Indicates that an assembly contains mixin configuration information and should be scanned when the default mixin configuration is
  /// initialized.
  /// </summary>
  /// <remarks>
  /// When the default mixin configuration is created, all assemblies currently loaded into the application domain are scanned for mixin
  /// configuration information if (and only if) they have this attribute applied to them. See also <see cref="ApplicationContextBuilder.BuildDefaultContext"/>.
  /// </remarks>
  /// <seealso cref="ApplicationContextBuilder.BuildDefaultContext"/>
  [AttributeUsage (AttributeTargets.Assembly)]
  public class ContainsMixinInfoAttribute : Attribute
  {
  }
}
