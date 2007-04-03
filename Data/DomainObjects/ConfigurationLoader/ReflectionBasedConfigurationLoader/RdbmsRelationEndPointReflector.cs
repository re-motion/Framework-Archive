using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Validate SortExpression on wrong side of BidirectionalRelation
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/> for types persisted in an <b>RDBMS</b>.</summary>
  public class RdbmsRelationEndPointReflector: RelationEndPointReflector
  {
    private readonly DBBidirectionalRelationAttribute _bidirectionalRelationAttribute;

    public RdbmsRelationEndPointReflector (PropertyInfo propertyInfo)
        : base (propertyInfo)
    {
      _bidirectionalRelationAttribute = AttributeUtility.GetCustomAttribute<DBBidirectionalRelationAttribute> (PropertyInfo, true);
    }

    public override bool IsVirtualEndRelationEndpoint()
    {
      if (base.IsVirtualEndRelationEndpoint())
        return true;

      return !ContainsKey();
    }

    private bool ContainsKey()
    {
      if (_bidirectionalRelationAttribute == null)
        return true;

      if (_bidirectionalRelationAttribute.ContainsForeignKey)
          return true;

      return IsCollectionProperyOnOppositeSide (_bidirectionalRelationAttribute);
    }

    private bool IsCollectionProperyOnOppositeSide (BidirectionalRelationAttribute bidirectionalRelationAttribute)
    {
      return IsCollectionPropery (GetOppositePropertyInfo (bidirectionalRelationAttribute));
    }

    protected override string GetSortExpression ()
    {
      if (_bidirectionalRelationAttribute == null)
        return null;

      return _bidirectionalRelationAttribute.SortExpression;
    }
  }
}