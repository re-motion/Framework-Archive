using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public class RelationReflectorBase: MemberReflectorBase
  {
    protected PropertyInfo GetOppositePropertyInfo (PropertyInfo propertyInfo, BidirectionalRelationAttribute bidirectionalRelationAttribute)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("bidirectionalRelationAttribute", bidirectionalRelationAttribute);

      PropertyInfo oppositePropertyInfo = propertyInfo.PropertyType.GetProperty (
          bidirectionalRelationAttribute.OppositeProperty,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      if (oppositePropertyInfo == null)
      {
        throw CreateMappingException (
            null,
            propertyInfo,
            "Opposite relation property '{0}' could not be found on type '{1}'.",
            bidirectionalRelationAttribute.OppositeProperty,
            propertyInfo.PropertyType);
      }

      return oppositePropertyInfo;
    }
  }
}