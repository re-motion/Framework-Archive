using System;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  internal interface IPropertyAccessorStrategy
  {
    Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition);
    bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction);
    bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction);
		object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction);
    void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value);
    object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction);
  }
}
