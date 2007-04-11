using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Infrastructure;

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
  ///   return GetPropertyValue&lt;...&gt; ();
  /// }
  /// set
  /// {
  ///   SetPropertyValue (value);
  /// }
  /// </code>
  /// </para>
  /// <para>
  /// Use automatic properties whenever a domain object's property implementationw ould be trivial. Make the properties abstract and annotate them
  /// with this attribute. Note that the automatic implementation will only work for domain objects instantiated with the
  /// <see cref="DomainObjectFactory"/>. In order to communicate that the domain object class is only abstract because of the automatic properties,
  /// annotate the class with a <see cref="NotAbstractAttribute"/>.
  /// </para>
  /// </remarks>
  // TODO: Remove
  [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class AutomaticPropertyAttribute : Attribute
  {

  }
}
