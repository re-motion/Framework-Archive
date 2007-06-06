using System;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Define the <see cref="ContainsMappingAttribute"/> on assemblies that you whish to be included during the automatic 
  /// loading of the assemblies containing the domain types.
  /// </summary>
  /// <remarks>
  /// The assemblies must be located within the applications base-directory or be referenced by other assemblies that also define the
  /// <see cref="ContainsMappingAttribute"/>.
  /// </remarks>
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
  public sealed class ContainsMappingAttribute : Attribute
  {
    public ContainsMappingAttribute()
    {
    }
  }
}