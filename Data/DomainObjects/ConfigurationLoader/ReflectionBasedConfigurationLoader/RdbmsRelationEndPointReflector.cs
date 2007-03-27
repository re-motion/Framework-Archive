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

    protected override bool IsVirtualEndRelationEndpoint()
    {
      if (base.IsVirtualEndRelationEndpoint())
        return true;

      return !ContainsKey();
    }

    private bool ContainsKey()
    {
      RdbmsBidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<RdbmsBidirectionalRelationAttribute> (PropertyInfo, true);
      if (attribute != null)
      {
        if (attribute.ContainsForeignKey)
          return true;
        return IsOppositeClassManySide (attribute);
      }
      return true;
    }

    private bool IsOppositeClassManySide (BidirectionalRelationAttribute bidirectionalRelationAttribute)
    {
      return IsManySide (GetOppositePropertyInfo (bidirectionalRelationAttribute));
    }
  }
}