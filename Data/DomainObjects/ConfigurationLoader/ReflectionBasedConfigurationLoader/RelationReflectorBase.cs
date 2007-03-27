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

      if (oppositePropertyInfo == null)
      {
        throw CreateMappingException (
            null,
            PropertyInfo,
            "Opposite relation property '{0}' could not be found on type '{1}'.",
            bidirectionalRelationAttribute.OppositeProperty,
            PropertyInfo.PropertyType);
      }

      return oppositePropertyInfo;
    }
  }
}