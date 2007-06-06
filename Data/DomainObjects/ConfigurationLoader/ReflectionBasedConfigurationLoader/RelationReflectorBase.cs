using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase: MemberReflectorBase
  {
    protected RelationReflectorBase (PropertyInfo propertyInfo)
        : base(propertyInfo)
    {
    }

    protected PropertyInfo GetOppositePropertyInfo (BidirectionalRelationAttribute bidirectionalRelationAttribute)
    {
      ArgumentUtility.CheckNotNull ("bidirectionalRelationAttribute", bidirectionalRelationAttribute);

      PropertyInfo oppositePropertyInfo = PropertyInfo.PropertyType.GetProperty (
          bidirectionalRelationAttribute.OppositeProperty,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      ValidateOppositePropertyInfo (bidirectionalRelationAttribute, oppositePropertyInfo);
      return oppositePropertyInfo;
    }

    private void ValidateOppositePropertyInfo (BidirectionalRelationAttribute bidirectionalRelationAttribute, PropertyInfo oppositePropertyInfo)
    {
      if (oppositePropertyInfo == null)
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Opposite relation property '{0}' could not be found on type '{1}'.",
            bidirectionalRelationAttribute.OppositeProperty,
            PropertyInfo.PropertyType);
      }

      Type oppositePropertyType;
      if (ReflectionUtility.IsObjectList (oppositePropertyInfo.PropertyType))
        oppositePropertyType = ReflectionUtility.GetObjectListTypeParameter (oppositePropertyInfo.PropertyType);
      else
        oppositePropertyType = oppositePropertyInfo.PropertyType;

      if (PropertyInfo.DeclaringType != oppositePropertyType)
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "The declaring type does not match the type of the opposite relation propery '{0}' declared on type '{1}'.",
            bidirectionalRelationAttribute.OppositeProperty,
            oppositePropertyInfo.DeclaringType);
      }
    }
  }
}