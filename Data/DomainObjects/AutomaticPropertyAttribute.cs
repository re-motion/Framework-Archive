using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Designates an abstract property of a domain object to be automatically implemented when the object is instantiated.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute causes abstract properties to be automatically implemented as follows:
  /// <code>
  /// get
  /// {
  ///   return GetPropertyValue<...> ();
  /// }
  /// set
  /// {
  ///   SetPropertyValue (value);
  /// }
  /// </code>
  /// </para>
  /// <para>
  /// Use this whenever you need to implement trivial properties on your domain objects. Note that the automatic implementation will only
  /// work for domain objects instantiated with the <see cref="DomainObjectFactory"/>.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class AutomaticPropertyAttribute : Attribute
  {

  }
}
