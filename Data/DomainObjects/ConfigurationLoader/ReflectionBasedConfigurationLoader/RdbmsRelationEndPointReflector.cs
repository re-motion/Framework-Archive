using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/> for types persisted in an <b>RDBMS</b>.</summary>
  public class RdbmsRelationEndPointReflector: RelationEndPointReflector
  {
    public RdbmsRelationEndPointReflector (PropertyInfo propertyInfo)
        : base (propertyInfo)
    {
    }

    public override bool IsVirtualEndRelationEndpoint()
    {
      if (base.IsVirtualEndRelationEndpoint())
        return true;

      return !ContainsKey();
    }

    private bool ContainsKey()
    {
      DBBidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<DBBidirectionalRelationAttribute> (PropertyInfo, true);
      if (attribute != null)
      {
        if (attribute.ContainsForeignKey)
          return true;
        return IsCollectionProperyOnOppositeSide (attribute);
      }
      return true;
    }

    private bool IsCollectionProperyOnOppositeSide (BidirectionalRelationAttribute bidirectionalRelationAttribute)
    {
      return IsCollectionPropery (GetOppositePropertyInfo (bidirectionalRelationAttribute));
    }
  }
}