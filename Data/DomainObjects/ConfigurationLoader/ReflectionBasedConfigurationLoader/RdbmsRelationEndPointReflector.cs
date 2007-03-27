using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/> for types persisted in an <b>RDBMS</b>.</summary>
  public class RdbmsRelationEndPointReflector : RelationEndPointReflector
  {
    public RdbmsRelationEndPointReflector()
    {
    }

    protected override bool IsVirtualEndRelationEndpoint (PropertyInfo propertyInfo)
    {
      if (base.IsVirtualEndRelationEndpoint (propertyInfo))
        return true;

      return !ContainsKey (propertyInfo);
    }

    private bool ContainsKey (PropertyInfo propertyInfo)
    {
      RdbmsBidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<RdbmsBidirectionalRelationAttribute> (propertyInfo, true);
      if (attribute != null)
      {
        if (attribute.ContainsForeignKey)
          return true;
        return IsOppositeClassManySide (propertyInfo, attribute);
      }
      return true;
    }

    private bool IsOppositeClassManySide (PropertyInfo propertyInfo, BidirectionalRelationAttribute bidirectionalRelationAttribute)
    {
      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo (propertyInfo, bidirectionalRelationAttribute);

      return IsManySide (oppositePropertyInfo);
    }
  }
}