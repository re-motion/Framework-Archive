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
          GetRelationID (),
          CreateEndPointReflector (PropertyInfo).GetMetadata (classDefinitions),
          GetOppositeEndPointDefinition (classDefinitions));
    }

    private RelationEndPointReflector CreateEndPointReflector (PropertyInfo propertyInfo)
    {
      return new RdbmsRelationEndPointReflector (propertyInfo);
    }

    private string GetRelationID ()
    {
      return PropertyInfo.DeclaringType.Name + "To" + PropertyInfo.Name;
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      BidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<BidirectionalRelationAttribute> (PropertyInfo, true);
      if (attribute == null)
        return new NullRelationEndPointDefinition (GetClassDefinition (classDefinitions));

      PropertyInfo oppositePropertyInfo = GetOppositePropertyInfo (attribute);
      return CreateEndPointReflector (oppositePropertyInfo).GetMetadata (classDefinitions);
    }

    private ClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (typeof (ObjectList<>).IsAssignableFrom (PropertyInfo.PropertyType))
        return classDefinitions.GetMandatory (PropertyInfo.PropertyType.GetGenericArguments ()[0]);
      return classDefinitions.GetMandatory (PropertyInfo.PropertyType);
    }
  }
}