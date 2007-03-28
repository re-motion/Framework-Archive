using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationEndPointReflector: RelationReflectorBase
  {
    public RelationEndPointReflector (PropertyInfo propertyInfo)
        : base (propertyInfo)
    {
    }

    public IRelationEndPointDefinition GetMetadata (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      Validate();
      ClassDefinition classDefinition = classDefinitions.GetMandatory (PropertyInfo.DeclaringType);

      if (IsVirtualEndRelationEndpoint())
        return CreateVirtualRelationEndPointDefinition (classDefinition);
      else
        return CreateRelationEndPointDefinition (classDefinition);
    }

    protected virtual bool IsVirtualEndRelationEndpoint()
    {
      return IsManySide (PropertyInfo);
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      return new RelationEndPointDefinition (classDefinition, GetPropertyName(), !IsNullable());
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      return new VirtualRelationEndPointDefinition (
          classDefinition,
          GetPropertyName(),
          !IsNullable(),
          GetCardinality(),
          PropertyInfo.PropertyType);
    }

    private CardinalityType GetCardinality()
    {
      return IsManySide (PropertyInfo) ? CardinalityType.Many : CardinalityType.One;
    }

    protected bool IsManySide (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return typeof (DomainObjectCollection).IsAssignableFrom (propertyInfo.PropertyType);
    }
  }
}