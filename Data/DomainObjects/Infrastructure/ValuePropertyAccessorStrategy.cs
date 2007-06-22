using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  internal class ValuePropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly ValuePropertyAccessorStrategy Instance = new ValuePropertyAccessorStrategy();

    private ValuePropertyAccessorStrategy ()
    {
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      return propertyDefinition.PropertyType;
    }

    private PropertyValue GetPropertyValue (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return propertyAccessor.DomainObject.GetDataContainer().PropertyValues[propertyAccessor.PropertyIdentifier];
    }

    public bool HasChanged (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return GetPropertyValue(propertyAccessor).HasChanged;
    }

		public bool IsNull (PropertyAccessor propertyAccessor)
		{
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
			return GetValueWithoutTypeCheck (propertyAccessor) == null;
		}

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return GetPropertyValue (propertyAccessor).Value;
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      GetPropertyValue (propertyAccessor).Value = value;
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return GetPropertyValue (propertyAccessor).OriginalValue;
    }
  }
}
