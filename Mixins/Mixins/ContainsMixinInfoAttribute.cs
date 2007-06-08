using System;
using Mixins.Context;

namespace Mixins
{
  /// <summary>
  /// Indicates that an assembly contains mixin configuration information and should thus be scanned when the default mixin configuration is
  /// initialized.
  /// </summary>
  /// <remarks>
  /// When the default mixin configuration is created, all assemblies currently loaded into the application domain are scanned for mixin
  /// configuration information if (and only if) they have this attribute applied to them. See also <see cref="ApplicationContextBuilder.BuildDefault"/>.
  /// </remarks>
  /// <seealso cref="ApplicationContextBuilder.BuildDefault"/>
  [AttributeUsage (AttributeTargets.Assembly)]
  public class ContainsMixinInfoAttribute : Attribute
  {
  }
}
