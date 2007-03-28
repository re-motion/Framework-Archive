using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class ClassReflector
  {
    private Type _type;
    private readonly ClassDefinitionCollection _cache;

    public ClassReflector (Type type, ClassDefinitionCollection cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("cache", cache);

      _type = type;
      _cache = cache;
    }

    public Type Type
    {
      get { return _type; }
    }

    public ClassDefinition GetMetadata()
    {
      if (_cache[Type] != null)
        return _cache[Type];

      ClassDefinition classDefinition = CreateClassDefinition();
      _cache.Add (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          GetID(),
          GetStorageSpecificName(),
          GetStorageProviderID(),
          Type,
          GetBaseClassDefinition());

      MemberInfo[] propertyInfos = Type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
          FindPropertiesFilter,
          null);

      CreatePropertyDefinitions(classDefinition, propertyInfos);

      List<PropertyInfo> possibleRelationEndPoints = new List<PropertyInfo> ();
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        if (IsRelationEndPoint (propertyInfo))
          possibleRelationEndPoints.Add (propertyInfo);
      }

      return classDefinition;
    }

    private void CreatePropertyDefinitions (ClassDefinition classDefinition, MemberInfo[] propertyInfos)
    {
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
        classDefinition.MyPropertyDefinitions.Add (propertyReflector.GetMetadata());
      }
    }

    private static bool IsRelationEndPoint (PropertyInfo propertyInfo)
    {
      return typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType);
    }

    private string GetID()
    {
      return Type.FullName;
    }

    private string GetStorageSpecificName()
    {
      return Type.Name;
    }

    private string GetStorageProviderID()
    {
      return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name;
    }

    private ClassDefinition GetBaseClassDefinition()
    {
      if (Type.BaseType == typeof (DomainObject))
        return null;

      ClassReflector classReflector = new ClassReflector (Type.BaseType, _cache);
      return classReflector.GetMetadata();
    }

    private bool FindPropertiesFilter (MemberInfo member, object filterCriteria)
    {
      PropertyInfo propertyInfo = ArgumentUtility.CheckNotNullAndType<PropertyInfo> ("member", member);

      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, true);
      if (storageClassAttribute != null && storageClassAttribute.StorageClass == StorageClass.None)
        return false;

      if (typeof (DomainObjectCollection).IsAssignableFrom (propertyInfo.PropertyType))
        return false;

      if (ReflectionUtility.GetOriginalDeclaringType (propertyInfo) != propertyInfo.DeclaringType)
        return false;

      return true;
    }
  }
}