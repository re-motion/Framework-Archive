using System;

namespace Rubicon.Data.DomainObjects
{
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
  public class MappingAssemblyAttribute: Attribute
  {
    public MappingAssemblyAttribute()
    {
    }
  }
}