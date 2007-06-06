using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  internal class RelatedObjectPropertyAccessorStrategy : IPropertyAccessorStrategy
  {
    public static readonly RelatedObjectPropertyAccessorStrategy Instance = new RelatedObjectPropertyAccessorStrategy();

    private RelatedObjectPropertyAccessorStrategy ()
    {
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      if (relationEndPointDefinition.PropertyType.Equals (typeof (ObjectID)))
        return relationEndPointDefinition.RelationDefinition.GetOppositeClassDefinition (relationEndPointDefinition).ClassType;
      else
        return relationEndPointDefinition.PropertyType;      
    }

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor accessor)
    {
      return new RelationEndPointID (accessor.DomainObject.ID, accessor.RelationEndPointDefinition);
    }

    public RelationEndPoint GetRelationEndPoint (PropertyAccessor accessor)
    {
      return accessor.DomainObject.ClientTransaction.DataManager.RelationEndPointMap[CreateRelationEndPointID (accessor)];
    }

    public bool HasChanged (PropertyAccessor propertyAccessor)
    {
      RelationEndPoint endPoint = GetRelationEndPoint (propertyAccessor);
      return endPoint != null && endPoint.HasChanged;
    }

    public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
      return propertyAccessor.DomainObject.ClientTransaction.GetRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value)
    {
      propertyAccessor.DomainObject.ClientTransaction.SetRelatedObject (CreateRelationEndPointID (propertyAccessor), (DomainObject) value);
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
      return propertyAccessor.DomainObject.ClientTransaction.GetOriginalRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
