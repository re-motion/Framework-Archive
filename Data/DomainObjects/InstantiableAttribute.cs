using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// When the <see cref="InstantiableAttribute"/> is defined on a type, it signals that this type can be instantiated by the 
  /// <see cref="DomainObjectFactory"/> even though it declared as <see langword="abstract"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class InstantiableAttribute: Attribute
  {
    public InstantiableAttribute()
    {
    }
  }
}