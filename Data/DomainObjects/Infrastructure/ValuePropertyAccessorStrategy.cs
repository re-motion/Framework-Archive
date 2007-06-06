using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;

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
      return propertyAccessor.DomainObject.DataContainer.PropertyValues[propertyAccessor.PropertyIdentifier];
    }

    public bool HasChanged (PropertyAccessor propertyAccessor)
    {
      return GetPropertyValue(propertyAccessor).HasChanged;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
      return GetPropertyValue (propertyAccessor).Value;
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value)
    {
      GetPropertyValue (propertyAccessor).Value = value;
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
      return GetPropertyValue (propertyAccessor).OriginalValue;
    }
  }
}
