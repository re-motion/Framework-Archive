using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Doc
  //TODO: More property logic to property reflector
  //TODO: Fix: Inheritance Root: Base Class Reflection stops with StorageGroupAttribute but super classes would still be part of mapping since they would get reflected upon them selve
  //TODO: Fix: Inheritance Root: Detect reapplication of StorageGroupAttribute
  public class ClassReflector
  {
    public static ClassReflector CreateClassReflector (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return CreateClassReflector (type, typeof (RdbmsClassReflector));
    }

    //TODO: Replace Hack with factory implementation
    private static ClassReflector CreateClassReflector (Type type, Type classReflectorType)
    {
      if (classReflectorType == typeof (RdbmsClassReflector))
        return new RdbmsClassReflector (type);
      else if (classReflectorType == typeof (ClassReflector))
        return new ClassReflector (type);

      throw new ArgumentException (string.Format ("ClassReflector of Type '{0}' is not supported.", classReflectorType));
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

    public ReflectionBasedClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      if (classDefinitions.Contains (Type))
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (Type);

      ReflectionBasedClassDefinition classDefiniton = CreateClassDefinition (classDefinitions);
      classDefinitions.Add (classDefiniton);

      return classDefiniton;
    }

    public List<RelationDefinition> GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      List<RelationDefinition> relations = new List<RelationDefinition>();
      foreach (PropertyInfo propertyInfo in GetPropertyInfos())
      {
        if (IsRelationEndPoint (propertyInfo))
        {
          RelationReflector relationReflector = new RelationReflector (propertyInfo);
          relations.Add (relationReflector.GetMetadata (classDefinitions));
        }
      }

      return relations;
    }

    private ReflectionBasedClassDefinition CreateClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          GetID(),
          GetStorageSpecificIdentifier(),
          GetStorageProviderID(),
          Type,
          IsAbstract(),
          GetBaseClassDefinition (classDefinitions));

      CreatePropertyDefinitions (classDefinition, GetPropertyInfos());

      return classDefinition;
    }

    private void CreatePropertyDefinitions (ReflectionBasedClassDefinition classDefinition, MemberInfo[] propertyInfos)
    {
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        PropertyReflector propertyReflector = new PropertyReflector (classDefinition, propertyInfo);
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

    public virtual string GetStorageSpecificIdentifier()
    {
      IStorageSpecificIdentifierAttribute attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (Type, false);
      if (attribute != null && !string.IsNullOrEmpty (attribute.Identifier))
        return attribute.Identifier;
      return GetID();
    }

    //TODO: Move type resolving to storagegrouplist
    private string GetStorageProviderID()
    {
      StorageGroupAttribute storageGroupAttribute = AttributeUtility.GetCustomAttribute<StorageGroupAttribute> (Type, true);
      if (storageGroupAttribute == null)
        return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name;

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupAttribute.GetType());
      StorageGroupElement storageGroup = DomainObjectsConfiguration.Current.Storage.StorageGroups[storageGroupName];
      return storageGroup.StorageProviderName;
    }

    private bool IsAbstract()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined (Type, typeof (InstantiableAttribute), false);

      return false;
    }

    private ReflectionBasedClassDefinition GetBaseClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (IsInheritenceRoot())
        return null;

      ClassReflector classReflector = CreateClassReflector (Type.BaseType, GetType());
      return classReflector.GetClassDefinition (classDefinitions);
    }

    private bool IsInheritenceRoot()
    {
      if (Type.BaseType == typeof (DomainObject))
        return true;

      return Attribute.IsDefined (Type, typeof (StorageGroupAttribute), false);
    }

    private PropertyInfo[] GetPropertyInfos()
    {
      List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
      propertyInfos.AddRange (GetPropertyInfosSorted (Type));

      if (IsInheritenceRoot())
      {
        for (Type type = Type.BaseType; type != typeof (DomainObject); type = type.BaseType)
          propertyInfos.AddRange (GetPropertyInfosSorted (type));
      }

      return propertyInfos.ToArray();
    }

    private IList<PropertyInfo> GetPropertyInfosSorted (Type type)
    {
      PropertyInfo[] propertyInfos = Array.ConvertAll<MemberInfo, PropertyInfo> (
          GetPropertyInfos (type),
          delegate (MemberInfo input) { return (PropertyInfo) input; });

      //Array.Sort (
      //    propertyInfos,
      //    delegate (PropertyInfo left, PropertyInfo right) { return string.Compare (ReflectionUtility.GetPropertyName (left), ReflectionUtility.GetPropertyName (right), StringComparison.Ordinal); });

      return propertyInfos;
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