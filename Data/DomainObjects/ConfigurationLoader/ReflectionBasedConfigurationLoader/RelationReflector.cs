using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector: RelationReflectorBase
  {
    public RelationReflector (PropertyInfo propertyInfo)
        : base (propertyInfo)
    {
    }

    public RelationDefinition GetMetadata (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      return new RelationDefinition (
          RelationID,
          CreateEndPointDefinition (classDefinitions, PropertyInfo),
          GetOppositeEndPointDefinition (classDefinitions));
    }

    private string RelationID
    {
      get { return PropertyInfo.DeclaringType.FullName + "." + PropertyInfo.Name; }
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      BidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<BidirectionalRelationAttribute> (PropertyInfo, true);
      if (attribute == null)
        return new NullRelationEndPointDefinition (GetClassDefinition (classDefinitions));

      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo (attribute);
      return CreateEndPointDefinition (classDefinitions, oppositePropertyInfo);
    }

    private IRelationEndPointDefinition CreateEndPointDefinition (ClassDefinitionCollection classDefinitions, PropertyInfo propertyInfo)
    {
      RelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);
      return relationEndPointReflector.GetMetadata (classDefinitions);
    }

    private ClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (typeof (ObjectList<>).IsAssignableFrom (PropertyInfo.PropertyType))
        return classDefinitions.GetMandatory (PropertyInfo.PropertyType.GetGenericArguments()[0]);
      return classDefinitions.GetMandatory (PropertyInfo.PropertyType);
    }
  }
}