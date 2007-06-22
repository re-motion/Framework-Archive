using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="PropertyInfo"/>.</summary>
  public class RelationReflector : RelationReflectorBase
  {
    public static RelationReflector CreateRelationReflector (PropertyInfo propertyInfo, ClassDefinitionCollection classDefinitions)
    {
      return new RelationReflector (propertyInfo, classDefinitions);
    }

    private ClassDefinitionCollection _classDefinitions;

    public RelationReflector (PropertyInfo propertyInfo, ClassDefinitionCollection classDefinitions)
      : this (propertyInfo, classDefinitions, typeof (BidirectionalRelationAttribute))
    {
    }

    protected RelationReflector (PropertyInfo propertyInfo, ClassDefinitionCollection classDefinitions, Type bidirectionalRelationAttributeType)
        : base (propertyInfo, bidirectionalRelationAttributeType)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      _classDefinitions = classDefinitions;
    }

    public RelationDefinition GetMetadata (RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      ValidatePropertyInfo();
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (PropertyInfo);

      if (relationEndPointReflector.IsVirtualEndRelationEndpoint())
      {
        ValidateVirtualEndPointPropertyInfo();
        return null;
      }
      else
      {
        if (relationDefinitions.Contains (GetRelationID ()))
          return (RelationDefinition) relationDefinitions[GetRelationID ()];
        
        RelationDefinition relationDefinition = new RelationDefinition (
            GetRelationID(),
            relationEndPointReflector.GetMetadata (_classDefinitions),
            CreateOppositeEndPointDefinition());

        AddRelationDefinitionToClassDefinitions (relationDefinition);
        relationDefinitions.Add (relationDefinition);

        return relationDefinition;
      }
    }

    private void ValidateVirtualEndPointPropertyInfo ()
    {
      RelationEndPointReflector oppositeRelationEndPointReflector = CreateOppositeRelationEndPointReflector ();
      if (oppositeRelationEndPointReflector.IsVirtualEndRelationEndpoint ())
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "A bidirectional relation can only have one virtual relation end point.",
            BidirectionalRelationAttribute.GetType ());
      }
    }

    protected override void ValidatePropertyInfo ()
    {
      base.ValidatePropertyInfo ();

      if (!IsBidirectionalRelation && !typeof (DomainObject).IsAssignableFrom (PropertyInfo.PropertyType))
      {
        throw CreateMappingException (
            null, PropertyInfo, "The property type of an uni-directional relation property must be assignable to {0}.", typeof (DomainObject).FullName);
      }
    }

    private string GetRelationID ()
    {
      return ReflectionUtility.GetPropertyName (PropertyInfo);
    }

    private IRelationEndPointDefinition CreateOppositeEndPointDefinition ()
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition();

      RelationEndPointReflector oppositeRelationEndPointReflector = CreateOppositeRelationEndPointReflector();
      return oppositeRelationEndPointReflector.GetMetadata (_classDefinitions);
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition ()
    {
      try
      {
        return new AnonymousRelationEndPointDefinition (_classDefinitions.GetMandatory (PropertyInfo.PropertyType));
      }
      catch (MappingException e)
      {
        throw CreateMappingException (null, PropertyInfo, e.Message);
      }
    }

    private RelationEndPointReflector CreateOppositeRelationEndPointReflector ()
    {
      return RelationEndPointReflector.CreateRelationEndPointReflector (GetOppositePropertyInfo ());
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