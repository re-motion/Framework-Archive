using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector: RelationReflectorBase
  {
    private RelationEndPointReflector _endPointReflector = new RdbmsRelationEndPointReflector();

    public RelationReflector()
    {
    }

    public RelationDefinition GetMetadata (ClassDefinitionCollection classDefinitions, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      return new RelationDefinition (
          GetRelationID (propertyInfo),
          _endPointReflector.GetMetadata (classDefinitions, propertyInfo),
          GetOppositeEndPointDefinition (classDefinitions, propertyInfo));
    }

    private string GetRelationID (PropertyInfo propertyInfo)
    {
      return propertyInfo.DeclaringType.Name + "To" + propertyInfo.Name;
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions, PropertyInfo propertyInfo)
    {
      BidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<BidirectionalRelationAttribute> (propertyInfo, true);
      if (attribute == null)
        return new NullRelationEndPointDefinition (GetClassDefinition (classDefinitions, propertyInfo));

      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo (propertyInfo, attribute);
      return _endPointReflector.GetMetadata (classDefinitions, oppositePropertyInfo);
    }

    private ClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions, PropertyInfo propertyInfo)
    {
      if (typeof (ObjectList<>).IsAssignableFrom (propertyInfo.PropertyType))
        return classDefinitions.GetMandatory (propertyInfo.PropertyType.GetGenericArguments()[0]);
      return classDefinitions.GetMandatory (propertyInfo.PropertyType);
    }
  }
}