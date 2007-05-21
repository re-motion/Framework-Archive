using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="IRelationEndPointDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  //TODO: Validate Property Type Assignable to DomainObject or DomainObjectCollection
  public class RelationEndPointReflector: RelationReflectorBase
  {
    public static RelationEndPointReflector CreateRelationEndPointReflector (PropertyInfo propertyInfo)
    {
      return new RdbmsRelationEndPointReflector (propertyInfo);
    }

    public RelationEndPointReflector (PropertyInfo propertyInfo)
        : base (propertyInfo)
    {
    }

    public IRelationEndPointDefinition GetMetadata (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      Validate();
      ClassDefinition classDefinition;
      try
      {
        classDefinition = classDefinitions.GetMandatory (PropertyInfo.DeclaringType);
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, PropertyInfo, e.Message);
      }

      if (IsVirtualEndRelationEndpoint())
        return CreateVirtualRelationEndPointDefinition (classDefinition);
      else
        return CreateRelationEndPointDefinition (classDefinition);
    }

    public virtual bool IsVirtualEndRelationEndpoint()
    {
      return IsCollectionPropery (PropertyInfo);
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
          PropertyInfo.PropertyType,
          GetSortExpression());
    }

    private CardinalityType GetCardinality()
    {
      return IsCollectionPropery (PropertyInfo) ? CardinalityType.Many : CardinalityType.One;
    }

    protected bool IsCollectionPropery (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return typeof (DomainObjectCollection).IsAssignableFrom (propertyInfo.PropertyType);
    }

    protected virtual string GetSortExpression ()
    {
      return null;
    }

    private bool IsNullable ()
    {
      return IsNullableFromAttribute ();
    }
  }
}