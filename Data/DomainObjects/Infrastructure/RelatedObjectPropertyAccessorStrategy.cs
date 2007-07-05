using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

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

    public RelationEndPointID CreateRelationEndPointID (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return new RelationEndPointID (propertyAccessor.DomainObject.ID, propertyAccessor.RelationEndPointDefinition);
    }

    public RelationEndPoint GetRelationEndPoint (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return ClientTransactionScope.CurrentTransaction.DataManager.RelationEndPointMap[CreateRelationEndPointID (propertyAccessor)];
    }

    public bool HasChanged (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      RelationEndPoint endPoint = GetRelationEndPoint (propertyAccessor);
      return endPoint != null && endPoint.HasChanged;
    }

  	public bool IsNull (PropertyAccessor propertyAccessor)
  	{
			ArgumentUtility.CheckNotNull ("accessor", propertyAccessor);
			if (propertyAccessor.RelationEndPointDefinition.IsVirtual)
				return GetValueWithoutTypeCheck (propertyAccessor) == null;
			else // for nonvirtual end points check out the ObjectID, which is stored in the DataContainer; this allows IsNull to avoid loading the object
				return ValuePropertyAccessorStrategy.Instance.GetValueWithoutTypeCheck (propertyAccessor) == null;
  	}

  	public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return ClientTransactionScope.CurrentTransaction.GetRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      ClientTransactionScope.CurrentTransaction.SetRelatedObject (CreateRelationEndPointID (propertyAccessor), (DomainObject) value);
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return ClientTransactionScope.CurrentTransaction.GetOriginalRelatedObject (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
