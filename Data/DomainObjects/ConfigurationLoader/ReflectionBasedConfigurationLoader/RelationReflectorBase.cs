using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase : MemberReflectorBase
  {
    private readonly BidirectionalRelationAttribute _bidirectionalRelationAttribute;

    protected RelationReflectorBase (PropertyInfo propertyInfo, Type bidirectionalRelationAttributeType)
        : base (propertyInfo)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom (
          "bidirectionalRelationAttributeType", bidirectionalRelationAttributeType, typeof (BidirectionalRelationAttribute));

      _bidirectionalRelationAttribute =
          (BidirectionalRelationAttribute) AttributeUtility.GetCustomAttribute (PropertyInfo, bidirectionalRelationAttributeType, true);
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
  }
}