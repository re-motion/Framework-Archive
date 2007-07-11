using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>The <see cref="PropertyFinderBase"/> is used to find all <see cref="PropertyInfo"/> objects relevant for the mapping.</summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="PropertyFinderBase"/>'s constructor signature. </remarks>
  public abstract class PropertyFinderBase
  {
    private readonly Type _type;
    private readonly bool _includeBaseProperties;
    private readonly Set<MethodInfo> _explicitInterfaceImplementations;

    protected PropertyFinderBase (Type type, bool includeBaseProperties)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));

      _type = type;
      _includeBaseProperties = includeBaseProperties;
      _explicitInterfaceImplementations = GetExplicitInterfaceImplementations (type);
    }

    public Type Type
    {
      get { return _type; }
    }

    public bool IncludeBaseProperties
    {
      get { return _includeBaseProperties; }
    }

    public PropertyInfo[] FindPropertyInfos (ReflectionBasedClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

      if (_includeBaseProperties && _type.BaseType != typeof (DomainObject))
      {
        PropertyFinderBase propertyFinder = (PropertyFinderBase) TypesafeActivator.CreateInstance (GetType()).With (_type.BaseType, true);
        propertyInfos.AddRange (propertyFinder.FindPropertyInfos (classDefinition));
      }

      propertyInfos.AddRange (FindPropertyInfosInternal (classDefinition));

      return propertyInfos.ToArray();
    }

    protected virtual bool FindPropertiesFilter (ReflectionBasedClassDefinition classDefinition, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!IsOriginalDeclaringType (propertyInfo))
      {
        CheckForMappingAttributes (propertyInfo);
        return false;
      }

      if (IsUnmanagedProperty (propertyInfo))
        return false;

      if (IsUnmanagedExplictInterfaceImplementation (propertyInfo))
        return false;

      return true;
    }

    protected void CheckForMappingAttributes (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

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

    protected bool IsOriginalDeclaringType (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return ReflectionUtility.GetOriginalDeclaringType (propertyInfo) == propertyInfo.DeclaringType;
    }

    protected bool IsUnmanagedProperty (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, false);
      if (storageClassAttribute == null)
        return false;

      return storageClassAttribute.StorageClass == StorageClass.None;
    }

    protected bool IsUnmanagedExplictInterfaceImplementation (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      bool isExplicitInterfaceImplementation = Array.Exists (
          propertyInfo.GetAccessors (true),
          delegate (MethodInfo accessor) { return _explicitInterfaceImplementations.Contains (accessor); });
      if (!isExplicitInterfaceImplementation)
        return false;

      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, false);
      if (storageClassAttribute == null)
        return true;

      return storageClassAttribute.StorageClass == StorageClass.None;
    }

    private IList<PropertyInfo> FindPropertyInfosInternal (ReflectionBasedClassDefinition classDefinition)
    {
      MemberInfo[] memberInfos = _type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
          FindPropertiesFilter,
          classDefinition);

      PropertyInfo[] propertyInfos = Array.ConvertAll<MemberInfo, PropertyInfo> (
          memberInfos,
          delegate (MemberInfo input) { return (PropertyInfo) input; });

      return propertyInfos;
    }

    private bool FindPropertiesFilter (MemberInfo member, object filterCriteria)
    {
      ReflectionBasedClassDefinition classDefinition = 
          ArgumentUtility.CheckNotNullAndType<ReflectionBasedClassDefinition> ("filterCriteria", filterCriteria);
      return FindPropertiesFilter (classDefinition, (PropertyInfo) member);
    }

    private Set<MethodInfo> GetExplicitInterfaceImplementations (Type type)
    {
      Set<MethodInfo> explicitInterfaceImplementationSet = new Set<MethodInfo>();

      foreach (Type interfaceType in type.GetInterfaces())
      {
        InterfaceMapping interfaceMapping = type.GetInterfaceMap (interfaceType);
        MethodInfo[] explicitInterfaceImplementations = Array.FindAll (
            interfaceMapping.TargetMethods,
            delegate (MethodInfo targetMethod) { return targetMethod.IsSpecialName && !targetMethod.IsPublic; });
        explicitInterfaceImplementationSet.AddRange (explicitInterfaceImplementations);
      }

      return explicitInterfaceImplementationSet;
    }
  }
}