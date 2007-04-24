using System;

namespace Rubicon.Data.DomainObjects
{
  //TODO: Doc
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class NotAbstractAttribute: Attribute
  {
    public NotAbstractAttribute()
    {
    }
  }
}