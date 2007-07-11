using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase : MemberReflectorBase
  {
    private readonly BidirectionalRelationAttribute _bidirectionalRelationAttribute;
    private readonly ReflectionBasedClassDefinition _classDefinition;

    protected RelationReflectorBase (
        ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType)
        : base (propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckClassDefinitionType (classDefinition, propertyInfo);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
          "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (BidirectionalRelationAttribute));

      _classDefinition = classDefinition;
      _bidirectionalRelationAttribute =
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (PropertyInfo, bidirectionalRelationAttributeType, true);
    }

    public ReflectionBasedClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public BidirectionalRelationAttribute BidirectionalRelationAttribute
    {
      get { return _bidirectionalRelationAttribute; }
    }

    protected bool IsBidirectionalRelation
    {
      get { return _bidirectionalRelationAttribute != null; }
    }

    protected PropertyInfo GetOppositePropertyInfo ()
    {
      for (Type type = GetDomainObjectTypeFromRelationProperty (PropertyInfo); type != null; type = type.BaseType)
      {
        PropertyInfo oppositePropertyInfo =
            type.GetProperty (BidirectionalRelationAttribute.OppositeProperty, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (oppositePropertyInfo != null)
          return oppositePropertyInfo;
      }

      throw CreateMappingException (
          null,
          PropertyInfo,
          "Opposite relation property '{0}' could not be found on type '{1}'.",
          BidirectionalRelationAttribute.OppositeProperty,
          GetDomainObjectTypeFromRelationProperty (PropertyInfo));
    }

    protected Type GetDomainObjectTypeFromRelationProperty (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (ReflectionUtility.IsObjectList (propertyInfo.PropertyType))
        return ReflectionUtility.GetObjectListTypeParameter (propertyInfo.PropertyType);
      else
        return propertyInfo.PropertyType;
    }

    private void CheckClassDefinitionType (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      //if (propertyInfo.DeclaringType.IsGenericType && !Utilities.ReflectionUtility.CanAscribe (classDefinition.ClassType, PropertyInfo.DeclaringType))
      //{
      //  throw new ArgumentTypeException (
      //      string.Format (
      //          "The classDefinition's class type '{0}' cannot be ascribed to the property's declaring type.\r\nDeclaring type: {1}, property: {2}",
      //          classDefinition.ClassType,
      //          propertyInfo.DeclaringType,
      //          propertyInfo.Name));
      //}

      if (
        //!propertyInfo.DeclaringType.IsGenericType && 
        !PropertyInfo.DeclaringType.IsAssignableFrom (classDefinition.ClassType))
      {
        throw new ArgumentTypeException (
            string.Format (
                "The classDefinition's class type '{0}' is not assignable to the property's declaring type.\r\nDeclaring type: {1}, property: {2}",
                classDefinition.ClassType,
                propertyInfo.DeclaringType,
                propertyInfo.Name));
      }
    }
  }
}