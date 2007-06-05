using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>The <see cref="PropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects relevant for the mapping.</summary>
  public class PropertyFinder
  {
    private readonly Type _type;

    private readonly bool _includeBaseProperties;

    public PropertyFinder (Type type, bool includeBaseProperties)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof (DomainObject));

      _type = type;
      _includeBaseProperties = includeBaseProperties;
    }

    public Type Type
    {
      get { return _type; }
    }

    public bool IncludeBaseProperties
    {
      get { return _includeBaseProperties; }
    }

    public PropertyInfo[] FindPropertyInfos ()
    {
      List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

      if (_includeBaseProperties && _type.BaseType != typeof (DomainObject))
      {
        PropertyFinder propertyFinder = new PropertyFinder (_type.BaseType, true);
        propertyInfos.AddRange (propertyFinder.FindPropertyInfos());
      }

      propertyInfos.AddRange (FindPropertyInfosInternal());

      return propertyInfos.ToArray();
    }

    private IList<PropertyInfo> FindPropertyInfosInternal ()
    {
      MemberInfo[] memberInfos = _type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
          FindPropertiesFilter,
          null);

      PropertyInfo[] propertyInfos = Array.ConvertAll<MemberInfo, PropertyInfo> (
          memberInfos,
          delegate (MemberInfo input) { return (PropertyInfo) input; });

      return propertyInfos;
    }

    private bool FindPropertiesFilter (MemberInfo member, object filterCriteria)
    {
      PropertyInfo propertyInfo = (PropertyInfo) member;

      if (!IsOriginalDeclaringType (propertyInfo))
      {
        CheckForMappingAttributes (propertyInfo);
        return false;
      }

      if (!IsManagedProperty (propertyInfo))
        return false;

      if (IsVirtualRelationEndPoint (propertyInfo))
        return false;

      return true;
    }

    private bool IsManagedProperty (PropertyInfo propertyInfo)
    {
      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, false);
      if (storageClassAttribute == null)
        return true;

      return storageClassAttribute.StorageClass != StorageClass.None;
    }

    private void CheckForMappingAttributes (PropertyInfo propertyInfo)
    {
      IMappingAttribute[] mappingAttributes = AttributeUtility.GetCustomAttributes<IMappingAttribute> (propertyInfo, false);
      if (mappingAttributes.Length > 0)
      {
        throw new MappingException (
            string.Format (
                "The '{0}' is a mapping attribute and may only be applied at the property's base definiton.\r\n  Type: {1}, property: {2}",
                mappingAttributes[0].GetType().FullName,
                propertyInfo.DeclaringType.FullName,
                propertyInfo.Name));
      }
    }

    private bool IsOriginalDeclaringType (PropertyInfo propertyInfo)
    {
      return ReflectionUtility.GetOriginalDeclaringType (propertyInfo) == propertyInfo.DeclaringType;
    }

    private bool IsVirtualRelationEndPoint (PropertyInfo propertyInfo)
    {
      RelationEndPointReflector relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector (propertyInfo);
      return relationEndPointReflector.IsVirtualEndRelationEndpoint();
    }
  }
}