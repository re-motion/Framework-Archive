using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

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
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return RelatedObjectPropertyAccessorStrategy.Instance.CreateRelationEndPointID (propertyAccessor);
    }

    public Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition)
    {
      return RelatedObjectPropertyAccessorStrategy.Instance.GetPropertyType (propertyDefinition, relationEndPointDefinition);
    }

    public bool HasChanged (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return RelatedObjectPropertyAccessorStrategy.Instance.HasChanged (propertyAccessor);
    }

  	public bool IsNull (PropertyAccessor propertyAccessor)
  	{
			ArgumentUtility.CheckNotNull ("accessor", propertyAccessor);
			return false;
  	}

  	public object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return propertyAccessor.DomainObject.ClientTransaction.GetRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }

    public void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, object value)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      throw new InvalidOperationException ("Related object collections cannot be set.");
    }

    public object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor)
    {
			ArgumentUtility.CheckNotNull ("propertyAccessor", propertyAccessor);
      return propertyAccessor.DomainObject.ClientTransaction.GetOriginalRelatedObjects (CreateRelationEndPointID (propertyAccessor));
    }
  }
}
