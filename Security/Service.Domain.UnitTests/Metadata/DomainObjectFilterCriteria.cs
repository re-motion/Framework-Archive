using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Service.Domain.UnitTests.Metadata
{
  public class DomainObjectFilterCriteria
  {
    private class PropertyExpectation
    {
      private PropertyInfo _propertyInfo;
      private object _expectedValue;

      public PropertyExpectation (PropertyInfo propertyInfo, object expectedValue)
      {
        _propertyInfo = propertyInfo;
        _expectedValue = expectedValue;
      }

      public PropertyInfo PropertyInfo
      {
        get { return _propertyInfo; }
      }

      public object ExpectedValue
      {
        get { return _expectedValue; }
      }

      public bool IsSatisfied (DomainObject domainObject)
      {
        object actualValue = _propertyInfo.GetValue (domainObject, null);
        return _expectedValue.Equals (actualValue);
      }
    }

    public static DomainObjectFilterCriteria ExpectType (Type filterType)
    {
      return new DomainObjectFilterCriteria (filterType);
    }

    private Type _filterType;
    private List<PropertyExpectation> _propertyExpectations;

    public DomainObjectFilterCriteria (Type filterType)
    {
      ArgumentUtility.CheckNotNull ("filterType", filterType);

      _filterType = filterType;
      _propertyExpectations = new List<PropertyExpectation> ();
    }

    public bool IsSatisfied (DomainObject domainObject)
    {
      if (!_filterType.IsAssignableFrom (domainObject.GetType ()))
        return false;

      foreach (PropertyExpectation propertyExpectation in _propertyExpectations)
      {
        if (!propertyExpectation.IsSatisfied (domainObject))
          return false;
      }

      return true;
    }

    public DomainObjectFilterCriteria ExpectPropertyValue (string propertyName, object expectedValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("expectedValue", expectedValue);

      PropertyInfo propertyInfo = _filterType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.Instance);
      if (propertyInfo == null)
        throw new ArgumentException (string.Format ("Property '{0}' not found on type '{1}'.", propertyName, _filterType.FullName), "propertyName");

      _propertyExpectations.Add (new PropertyExpectation (propertyInfo, expectedValue));
      return this;
    }
  }
}
