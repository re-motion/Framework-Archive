using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Validate SortExpression on wrong side of BidirectionalRelation
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/> for types persisted in an <b>RDBMS</b>.</summary>
  public class RdbmsRelationEndPointReflector : RelationEndPointReflector
  {
    public RdbmsRelationEndPointReflector (PropertyInfo propertyInfo)
        : this (propertyInfo, typeof (DBBidirectionalRelationAttribute))
    {
    }

    protected RdbmsRelationEndPointReflector (PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType)
        : base (
            propertyInfo,
            ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
                "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (DBBidirectionalRelationAttribute)))
    {
    }

    public DBBidirectionalRelationAttribute DBBidirectionalRelationAttribute
    {
      get { return (DBBidirectionalRelationAttribute) BidirectionalRelationAttribute; }
    }

    public override bool IsVirtualEndRelationEndpoint ()
    {
      if (base.IsVirtualEndRelationEndpoint())
        return true;

      return !ContainsKey();
    }

    private bool ContainsKey ()
    {
      if (!IsBidirectionalRelation)
        return true;

      if (DBBidirectionalRelationAttribute.ContainsForeignKey)
        return true;

      return IsCollectionProperyOnOppositeSide();
    }

    private bool IsCollectionProperyOnOppositeSide ()
    {
      return ReflectionUtility.IsObjectList (GetOppositePropertyInfo().PropertyType);
    }

    protected override string GetSortExpression ()
    {
      if (!IsBidirectionalRelation)
        return null;

      return DBBidirectionalRelationAttribute.SortExpression;
    }
  }
}