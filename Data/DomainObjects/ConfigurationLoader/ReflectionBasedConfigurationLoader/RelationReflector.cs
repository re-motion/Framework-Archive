using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Validation: Invalid Opposite Property (Name, Type)
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

      RelationDefinition relationDefinition = new RelationDefinition (
          GetRelationID(),
          CreateEndPointDefinition (classDefinitions, PropertyInfo),
          GetOppositeEndPointDefinition (classDefinitions));

      AddRelationDefinitionToClassDefinitions (relationDefinition);

      return relationDefinition;
    }

    private string GetRelationID()
    {
      return PropertyInfo.DeclaringType.FullName + "." + PropertyInfo.Name;
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      BidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<BidirectionalRelationAttribute> (PropertyInfo, true);
      if (attribute == null)
        return CreateOppositeAnonymousRelationEndPointDefinition (classDefinitions);

      return CreateEndPointDefinition (classDefinitions, GetOppositePropertyInfo (attribute));
    }

    private IRelationEndPointDefinition CreateEndPointDefinition (ClassDefinitionCollection classDefinitions, PropertyInfo propertyInfo)
    {
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (propertyInfo);
      return relationEndPointReflector.GetMetadata (classDefinitions);
    }

    private NullRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (!typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
      {
        throw CreateMappingException (
            null, PropertyInfo, "The property type of an uni-directional relation property must be assignable to {0}.", typeof (DomainObject).FullName);
      }

      try
      {
        return new NullRelationEndPointDefinition (classDefinitions.GetMandatory (PropertyInfo.PropertyType));
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, PropertyInfo, e.Message);
      }
    }

    private void AddRelationDefinitionToClassDefinitions (RelationDefinition relationDefinition)
    {
      IRelationEndPointDefinition endPoint1 = relationDefinition.EndPointDefinitions[0];
      IRelationEndPointDefinition endPoint2 = relationDefinition.EndPointDefinitions[1];

      if (!endPoint1.IsNull)
        endPoint1.ClassDefinition.MyRelationDefinitions.Add (relationDefinition);

      if (endPoint1.ClassDefinition != endPoint2.ClassDefinition && !endPoint2.IsNull)
        endPoint2.ClassDefinition.MyRelationDefinitions.Add (relationDefinition);
    }
  }
}