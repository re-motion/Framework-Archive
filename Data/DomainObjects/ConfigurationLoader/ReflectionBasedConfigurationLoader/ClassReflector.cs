using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Doc
  //TODO: Validation: no types having IgnoreForMappingAttribute
  //TODO: More property logic to property reflector
  public class ClassReflector
  {
    public static ClassReflector CreateClassReflector (Type type)
    {
      return new RdbmsClassReflector (type);
    }

    private Type _type;

    public ClassReflector (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));

      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public ClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      if (classDefinitions[Type] != null)
        return classDefinitions[Type];

      ClassDefinition classDefiniton = CreateClassDefinition (classDefinitions);
      classDefinitions.Add (classDefiniton);

      return classDefiniton;
    }

    public List<RelationDefinition> GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      List<RelationDefinition> relations = new List<RelationDefinition>();
      foreach (PropertyInfo propertyInfo in GetPropertyInfos ())
      {
        if (IsRelationEndPoint (propertyInfo))
        {
          RelationReflector relationReflector = new RelationReflector (propertyInfo);
          relations.Add (relationReflector.GetMetadata (classDefinitions));
        }
      }

      return relations;
    }

    private ClassDefinition CreateClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          GetID(),
          GetStorageSpecificName(),
          GetStorageProviderID(),
          Type,
          IsAbstract(),
          GetBaseClassDefinition (classDefinitions));

      CreatePropertyDefinitions (classDefinition, GetPropertyInfos());

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
      ClassIDAttribute attribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute> (Type, false);
      if (attribute != null)
        return attribute.ClassID;
      return Type.Name;
    }

    public virtual string GetStorageSpecificName()
    {
      IStorageSpecificIdentifierAttribute attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (Type, false);
      if (attribute != null && !string.IsNullOrEmpty (attribute.Identifier))
        return attribute.Identifier;
      return GetID();
    }

    private string GetStorageProviderID()
    {
      return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name;
    }

    private bool IsAbstract()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined (Type, typeof (NotAbstractAttribute), false);

      return false;
    }

    private ClassDefinition GetBaseClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (IsInheritenceRoot())
        return null;

      ClassReflector classReflector = new ClassReflector (Type.BaseType);
      return classReflector.GetClassDefinition (classDefinitions);
    }

    private bool IsInheritenceRoot()
    {
      if (Type.BaseType == typeof (DomainObject))
        return true;

      return Attribute.IsDefined (Type, typeof (StorageGroupAttribute), false);
    }

    private MemberInfo[] GetPropertyInfos()
    {
      List<MemberInfo> propertyInfos = new List<MemberInfo>();
      propertyInfos.AddRange (GetPropertyInfos (Type));

      if (IsInheritenceRoot())
      {
        for (Type type = Type.BaseType; type != typeof (DomainObject); type = type.BaseType)
          propertyInfos.AddRange (GetPropertyInfos (type));
      }

      return propertyInfos.ToArray();
    }

    private MemberInfo[] GetPropertyInfos (Type type)
    {
      return type.FindMembers (
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