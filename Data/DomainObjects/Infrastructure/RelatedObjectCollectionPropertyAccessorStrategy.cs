using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  internal class RelatedObjectCollectionPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly RelatedObjectCollectionPropertyAccessorStrategy Instance = new RelatedObjectCollectionPropertyAccessorStrategy();

    private RelatedObjectCollectionPropertyAccessorStrategy ()
    {
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
      return RelatedObjectPropertyAccessorStrategy.Instance.CreateRelationEndPointID (propertyAccessor);
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      return RelatedObjectPropertyAccessorStrategy.Instance.GetPropertyType (propertyDefinition, relationEndPointDefinition);
    }

    public bool HasChanged (PropertyAccessor propertyAccessor)
    {
      return RelatedObjectPropertyAccessorStrategy.Instance.HasChanged (propertyAccessor);
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
      return propertyAccessor.DomainObject.ClientTransaction.GetRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value)
    {
      throw new InvalidOperationException ("Related object collections cannot be set.");
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
      return propertyAccessor.DomainObject.ClientTransaction.GetOriginalRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
