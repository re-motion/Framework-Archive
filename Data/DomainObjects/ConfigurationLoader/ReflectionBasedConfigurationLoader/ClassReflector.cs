using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Fix: Inheritance Root: Base Class Reflection stops with StorageGroupAttribute but super classes would still be part of mapping since they would get reflected upon them selve
  //TODO: Fix: Inheritance Root: Detect reapplication of StorageGroupAttribute
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to build a <see cref="ReflectionBasedClassDefinition"/> and the <see cref="RelationDefinition"/> 
  /// objects for a type.
  /// </summary>
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

      if (classDefinitions.Contains (_type))
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (_type);

      ReflectionBasedClassDefinition classDefiniton = CreateClassDefinition (classDefinitions);
      classDefinitions.Add (classDefiniton);

      return classDefiniton;
    }

    public List<RelationDefinition> GetRelationDefinitions (ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      List<RelationDefinition> relations = new List<RelationDefinition>();
      foreach (PropertyInfo propertyInfo in GetRelationPropertyInfos ())
      {
        RelationReflector relationReflector = new RelationReflector (propertyInfo, classDefinitions);
        relations.Add (relationReflector.GetMetadata (relationDefinitions));
      }

      return relations;
    }

    private ReflectionBasedClassDefinition CreateClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          GetID(),
          GetStorageSpecificIdentifier(),
          GetStorageProviderID(),
          _type,
          IsAbstract(),
          GetBaseClassDefinition (classDefinitions));

      CreatePropertyDefinitions (classDefinition, GetPropertyInfos());

      ValidateClassDefinition (classDefinition);

      return classDefinition;
    }

    //TODO: Write test for abstract DomainObject with infrasturcture constructor
    //TODO: Write test for fail
    //TODO: Write test that fails for Generic DomainObject
    private void ValidateClassDefinition (ReflectionBasedClassDefinition classDefinition)
    {
      if (!classDefinition.IsAbstract)
      {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding;
        ConstructorInfo legacyLoadConstructor = classDefinition.ClassType.GetConstructor (flags, null, new Type[] {typeof (DataContainer)}, null);
        if (legacyLoadConstructor != null)
        {
          throw new MappingException (
              string.Format (
                  "Domain object type {0} has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer"
                  + " argument). The reflection-based mapping does not use this constructor any longer and requires it to be removed.",
                  classDefinition.ClassType));
        }
      }
    }

    private void CreatePropertyDefinitions (ReflectionBasedClassDefinition classDefinition, MemberInfo[] propertyInfos)
    {
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        PropertyReflector propertyReflector = new PropertyReflector (classDefinition, propertyInfo);
        classDefinition.MyPropertyDefinitions.Add (propertyReflector.GetMetadata());
      }
    }

    private string GetID ()
    {
      ClassIDAttribute attribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute> (_type, false);
      if (attribute != null)
        return attribute.ClassID;
      return _type.Name;
    }

    public virtual string GetStorageSpecificIdentifier ()
    {
      IStorageSpecificIdentifierAttribute attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (_type, false);
      if (attribute != null && !string.IsNullOrEmpty (attribute.Identifier))
        return attribute.Identifier;
      return GetID();
    }

    //TODO: Move type resolving to storagegrouplist
    //TODO: Test for DefaultStorageProvider
    private string GetStorageProviderID ()
    {
      StorageGroupAttribute storageGroupAttribute = AttributeUtility.GetCustomAttribute<StorageGroupAttribute> (_type, true);
      if (storageGroupAttribute == null)
        return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name;

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupAttribute.GetType());
      StorageGroupElement storageGroup = DomainObjectsConfiguration.Current.Storage.StorageGroups[storageGroupName];
      if (storageGroup == null)
        return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name;
      return storageGroup.StorageProviderName;
    }

    private bool IsAbstract ()
    {
      if (_type.IsAbstract)
        return !Attribute.IsDefined (_type, typeof (InstantiableAttribute), false);

      return false;
    }

    private ReflectionBasedClassDefinition GetBaseClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (IsInheritenceRoot())
        return null;

      ClassReflector classReflector = CreateClassReflector (_type.BaseType, GetType ());
      return classReflector.GetClassDefinition (classDefinitions);
    }

    private PropertyInfo[] GetPropertyInfos ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (_type, IsInheritenceRoot());
      return propertyFinder.FindPropertyInfos();
    }

    private PropertyInfo[] GetRelationPropertyInfos ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (_type, IsInheritenceRoot ());
      return Array.FindAll (propertyFinder.FindPropertyInfos (), delegate (PropertyInfo propertyInfo) { return IsRelationEndPoint (propertyInfo); });
    }

    private bool IsInheritenceRoot ()
    {
      if (_type.BaseType == typeof (DomainObject))
        return true;

      return Attribute.IsDefined (_type, typeof (StorageGroupAttribute), false);
    }

    private bool IsRelationEndPoint (PropertyInfo propertyInfo)
    {
      return typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType);
    }
  }
}