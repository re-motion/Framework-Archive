using System;

namespace Rubicon.Data.DomainObjects
{
  //TODO: Doc
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class StorageGroupAttribute: Attribute
  {
    protected StorageGroupAttribute()
    {
    }
  }
}