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
    private readonly ClassDefinitionCollection _classDefinitions;
    private readonly List<RelationReflector> _relations;

    public ClassReflector (Type type, ClassDefinitionCollection classDefinitions, List<RelationReflector> relations)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relations", relations);

      _type = type;
      _classDefinitions = classDefinitions;
      _relations = relations;
    }

    public Type Type
    {
      get { return _type; }
    }

    public ClassDefinition GetMetadata()
    {
      if (_classDefinitions[Type] != null)
        return _classDefinitions[Type];

      ClassDefinition classDefiniton = CreateClassDefinition();
      _classDefinitions.Add (classDefiniton);

      return classDefiniton;
    }

    private ClassDefinition CreateClassDefinition()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          GetID(),
          GetStorageSpecificName(),
          GetStorageProviderID(),
          Type,
          GetBaseClassDefinition(Type));

      MemberInfo[] propertyInfos = GetPropertyInfos();

      CreatePropertyDefinitions (classDefinition, propertyInfos);

      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        if (IsRelationEndPoint (propertyInfo))
          _relations.Add (new RelationReflector (propertyInfo));
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

    private bool IsRelationEndPoint (PropertyInfo propertyInfo)
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

    private ClassDefinition GetBaseClassDefinition (Type type)
    {
      if (type.BaseType == typeof (DomainObject))
        return null;

      if (Attribute.IsDefined (type.BaseType, typeof (IgnoreForMappingAttribute), false))
        return GetBaseClassDefinition (type.BaseType);

      ClassReflector classReflector = new ClassReflector (type.BaseType, _classDefinitions, _relations);
      return classReflector.GetMetadata();
    }

    private MemberInfo[] GetPropertyInfos()
    {
      return Type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
          FindPropertiesFilter,
          null);
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

    private static bool IsOriginalDeclaringType (PropertyInfo propertyInfo)
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