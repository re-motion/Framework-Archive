using System;

namespace Rubicon.Data.DomainObjects
{
  //TODO: Find abetter name
  //TODO: Doc
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class IgnoreForMappingAttribute: Attribute
  {
    public IgnoreForMappingAttribute()
    {
    }
  }
}