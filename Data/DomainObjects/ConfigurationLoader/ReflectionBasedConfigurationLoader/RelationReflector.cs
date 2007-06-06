using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector : RelationReflectorBase
  {
    private ClassDefinitionCollection _classDefinitions;

    public RelationReflector (PropertyInfo propertyInfo, ClassDefinitionCollection classDefinitions)
        : base (propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      _classDefinitions = classDefinitions;
    }

    public RelationDefinition GetMetadata (RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      //if (relationDefinitions.Contains (GetRelationID()))
      //  return (RelationDefinition) relationDefinitions[GetRelationID()];

      RelationDefinition relationDefinition = CreateRelationDefinition ();
      relationDefinitions.Add (relationDefinition);

      return relationDefinition;
    }

    private RelationDefinition CreateRelationDefinition ()
    {
      RelationDefinition relationDefinition = new RelationDefinition (
          GetRelationID(),
          CreateEndPointDefinition (PropertyInfo),
          GetOppositeEndPointDefinition ());

      AddRelationDefinitionToClassDefinitions (relationDefinition);

      return relationDefinition;
    }

    private string GetRelationID ()
    {
      return ReflectionUtility.GetPropertyName (PropertyInfo);
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition ()
    {
      BidirectionalRelationAttribute attribute = AttributeUtility.GetCustomAttribute<BidirectionalRelationAttribute> (PropertyInfo, true);
      if (attribute == null)
        return CreateOppositeAnonymousRelationEndPointDefinition ();

      return CreateEndPointDefinition (GetOppositePropertyInfo (attribute));
    }

    private IRelationEndPointDefinition CreateEndPointDefinition (PropertyInfo propertyInfo)
    {
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (propertyInfo);
      return relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition ()
    {
      if (!typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
      {
        throw CreateMappingException (
            null, PropertyInfo, "The property type of an uni-directional relation property must be assignable to {0}.", typeof (DomainObject).FullName);
      }

      try
      {
        return new AnonymousRelationEndPointDefinition (_classDefinitions.GetMandatory (PropertyInfo.PropertyType));
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