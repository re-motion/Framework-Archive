using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  internal interface IPropertyAccessorStrategy
  {
    Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition);
    bool HasChanged (PropertyAccessor propertyAccessor);
    object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor);
    void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value);
    object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor);
  }
}
