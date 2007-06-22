using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexer to access a specific property of a domain object. Instances of this value type are returned by
  /// <see cref="DomainObject.Properties"/>.
  /// </summary>
  public struct PropertyIndexer : IEnumerable<PropertyAccessor>
  {
    private DomainObject _domainObject;
    
    /// <summary>
    /// Initializes a new <see cref="PropertyIndexer"/> instance. This is usually not called from the outside; instead, <see cref="PropertyIndexer"/>
    /// instances are returned by <see cref="DomainObject.Properties"/>.
    /// </summary>
    /// <param name="domainObject">The domain object whose properties should be accessed with this <see cref="PropertyIndexer"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
    public PropertyIndexer (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _domainObject = domainObject;
    }

    /// <summary>
    /// Selects the property of the domain object with the given name.
    /// </summary>
    /// <param name="propertyName">The name of the property to be accessed.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="propertyName"/> parameter does not denote a valid mapping property of the domain object.
    /// </exception>
    public PropertyAccessor this[string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("propertyName", propertyName);
        try
        {
          return new PropertyAccessor (_domainObject, propertyName);
        }
        catch (ArgumentException ex)
        {
          throw new ArgumentException (string.Format ("The domain object type {0} does not have a mapping property named '{1}'.",
            _domainObject.DataContainer.ClassDefinition.ClassType.FullName, propertyName), "propertyName", ex);
        }
      }
    }

		/// <summary>
		/// Gets the number of properties defined by the domain object. This corresponds to the number of <see cref="PropertyAccessor"/> objects
		/// indexable by this structure and enumerated by <see cref="GetEnumerator"/>.
		/// </summary>
		/// <value>The number of properties defined by the domain object.</value>
  	public int Count
  	{
			get
			{
				ClassDefinition classDefinition = _domainObject.ID.ClassDefinition;
				IRelationEndPointDefinition[] endPointDefinitions = classDefinition.GetRelationEndPointDefinitions ();
				int count = classDefinition.GetPropertyDefinitions ().Count;
				foreach (IRelationEndPointDefinition endPointDefinition in endPointDefinitions)
				{
					if (endPointDefinition.IsVirtual)
						++count;
				}
				return count;
			}
  	}

  	public IEnumerator<PropertyAccessor> GetEnumerator ()
  	{
			ClassDefinition classDefinition = _domainObject.ID.ClassDefinition;

  		foreach (PropertyDefinition propertyDefinition in classDefinition.GetPropertyDefinitions())
  			yield return this[propertyDefinition.PropertyName];

			foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetRelationEndPointDefinitions ())
			{
				if (endPointDefinition.IsVirtual)
					yield return this[endPointDefinition.PropertyName];
			}
  	}

  	IEnumerator IEnumerable.GetEnumerator ()
  	{
			return GetEnumerator ();
  	}
  }
}
