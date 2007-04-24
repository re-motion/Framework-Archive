using System;

namespace Rubicon.Data.DomainObjects
{
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
  public sealed class MappingAssemblyAttribute : Attribute
  {
    public MappingAssemblyAttribute()
    {
    }
  }
}