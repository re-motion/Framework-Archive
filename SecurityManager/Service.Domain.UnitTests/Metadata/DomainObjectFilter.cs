using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;

namespace Rubicon.Security.Service.Domain.UnitTests.Metadata
{
  public class DomainObjectFilter
  {
    private DomainObjectCollection _originalCollection;

    public DomainObjectFilter (DomainObjectCollection originalCollection)
    {
      ArgumentUtility.CheckNotNull ("originalCollection", originalCollection);
      _originalCollection = originalCollection;
    }

    public DomainObjectCollection GetObjects (DomainObjectFilterCriteria criteria)
    {
      ArgumentUtility.CheckNotNull ("criteria", criteria);

      DomainObjectCollection filteredCollection = new DomainObjectCollection ();

      foreach (DomainObject domainObject in _originalCollection)
      {
        if (criteria.IsSatisfied (domainObject))
          filteredCollection.Add (domainObject);
      }

      return filteredCollection;
    }

    public DomainObject GetObject (DomainObjectFilterCriteria criteria)
    {
      ArgumentUtility.CheckNotNull ("criteria", criteria);

      DomainObjectCollection result = GetObjects (criteria);
      if (result.Count == 0)
        return null;

      if (result.Count > 1)
        throw new ArgumentException ("Multiple objects satisfy the filter criteria.", "criteria");

      return result[0];
    }

    public T GetObject<T> (DomainObjectFilterCriteria criteria) where T : DomainObject
    {
      return (T) GetObject (criteria);
    }
  }
}
