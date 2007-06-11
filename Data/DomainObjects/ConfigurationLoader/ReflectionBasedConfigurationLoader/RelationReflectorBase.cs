using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase: MemberReflectorBase
  {
    private readonly BidirectionalRelationAttribute _bidirectionalRelationAttribute;

    protected RelationReflectorBase (PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType)
        : base(propertyInfo)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
          "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (BidirectionalRelationAttribute));

      _bidirectionalRelationAttribute = 
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute(PropertyInfo, bidirectionalRelationAttributeType, true);
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
      Type domainObjectType = GetDomainObjectTypeFromRelationProperty (PropertyInfo);
      PropertyInfo oppositePropertyInfo = domainObjectType.GetProperty (
          BidirectionalRelationAttribute.OppositeProperty,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      ValidateOppositePropertyInfo (oppositePropertyInfo);
      return oppositePropertyInfo;
    }

    private void ValidateOppositePropertyInfo (PropertyInfo oppositePropertyInfo)
    {
      if (oppositePropertyInfo == null)
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Opposite relation property '{0}' could not be found on type '{1}'.",
            BidirectionalRelationAttribute.OppositeProperty,
            GetDomainObjectTypeFromRelationProperty (PropertyInfo));
      }

      if (PropertyInfo.DeclaringType != GetDomainObjectTypeFromRelationProperty (oppositePropertyInfo))
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "The declaring type does not match the type of the opposite relation propery '{0}' declared on type '{1}'.",
            BidirectionalRelationAttribute.OppositeProperty,
            oppositePropertyInfo.DeclaringType);
      }

      BidirectionalRelationAttribute oppositeBidirectionalRelationAttribute = (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (
          oppositePropertyInfo, _bidirectionalRelationAttribute.GetType (), true);
      if (oppositeBidirectionalRelationAttribute == null)
      {
        throw CreateMappingException (
          null,
          PropertyInfo,
          "Opposite relation property '{0}' declared on type '{1}' does not define a matching '{2}'.",
          BidirectionalRelationAttribute.OppositeProperty,
          oppositePropertyInfo.DeclaringType,
          _bidirectionalRelationAttribute.GetType());
      }

      if (!PropertyInfo.Name.Equals (oppositeBidirectionalRelationAttribute.OppositeProperty, StringComparison.Ordinal))
      {
         throw CreateMappingException (
          null,
          PropertyInfo,
          "Opposite relation property '{0}' declared on type declared on type '{1}' defines a '{2}' whose opposite property does not match.",
          BidirectionalRelationAttribute.OppositeProperty,
          oppositePropertyInfo.DeclaringType,
          _bidirectionalRelationAttribute.GetType());
     }
    }

    private Type GetDomainObjectTypeFromRelationProperty (PropertyInfo propertyInfo)
    {
      if (ReflectionUtility.IsObjectList (propertyInfo.PropertyType))
        return ReflectionUtility.GetObjectListTypeParameter (propertyInfo.PropertyType);
      else
        return propertyInfo.PropertyType;
    }
  }
}