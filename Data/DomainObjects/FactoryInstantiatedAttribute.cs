using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Denotes that a domain object should be instantiated via DomainObjectFactory when loaded.
  /// </summary>
  /// <remarks>
  /// Without this attribute applied to its class, a domain object will be instantiated via ReflectionUtility.CreateObject. With this attribute, it is
  /// instantiated via ObjectFactory.Create.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public class FactoryInstantiatedAttribute : Attribute
  {
  }
}
