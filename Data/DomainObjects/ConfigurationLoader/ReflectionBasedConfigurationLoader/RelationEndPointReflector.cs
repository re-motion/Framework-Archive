using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationEndPointReflector : RelationReflectorBase
  {
    public RelationEndPointReflector()
    {
    }

    public IRelationEndPointDefinition GetMetadata (ClassDefinitionCollection classDefinitions, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      Validate (propertyInfo);
      ClassDefinition classDefinition = classDefinitions.GetMandatory (propertyInfo.DeclaringType);

      if (IsVirtualEndRelationEndpoint (propertyInfo))
        return CreateVirtualRelationEndPointDefinition (classDefinition, propertyInfo);
      else
        return CreateRelationEndPointDefinition (classDefinition, propertyInfo);
    }

    protected virtual bool IsVirtualEndRelationEndpoint (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return IsManySide (propertyInfo);
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      return new RelationEndPointDefinition (classDefinition, GetPropertyName (propertyInfo), !GetNullability (propertyInfo));
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      return new VirtualRelationEndPointDefinition (
          classDefinition,
          GetPropertyName (propertyInfo),
          !GetNullability (propertyInfo),
          GetCardinality (propertyInfo),
          propertyInfo.PropertyType);
    }

    private CardinalityType GetCardinality (PropertyInfo propertyInfo)
    {
      return IsManySide (propertyInfo) ? CardinalityType.Many : CardinalityType.One;
    }

    protected bool IsManySide (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return typeof (DomainObjectCollection).IsAssignableFrom (propertyInfo.PropertyType);
    }
  }
}