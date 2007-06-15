using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Fix: Inheritance Root: Base Class Reflection stops with StorageGroupAttribute but super classes would still be part of mapping since they would get reflected upon them selve
  //TODO: Fix: Inheritance Root: Detect reapplication of StorageGroupAttribute
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to build a <see cref="ReflectionBasedClassDefinition"/> and the <see cref="RelationDefinition"/> 
  /// objects for a type.
  /// </summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="ClassReflector"/>'s constructor signature. </remarks>
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

    public ReflectionBasedClassDefinition GetClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      if (classDefinitions.Contains (_type))
        return (ReflectionBasedClassDefinition) classDefinitions.GetMandatory (_type);

      ReflectionBasedClassDefinition classDefiniton = CreateClassDefinition (classDefinitions);
      classDefinitions.Add (classDefiniton);

      return classDefiniton;
    }

    public List<RelationDefinition> GetRelationDefinitions (
        ClassDefinitionCollection classDefinitions, RelationDefinitionCollection relationDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("relationDefinitions", relationDefinitions);

      List<RelationDefinition> relations = new List<RelationDefinition>();
      foreach (PropertyInfo propertyInfo in GetRelationPropertyInfos())
      {
        RelationReflector relationReflector = RelationReflector.CreateRelationReflector (propertyInfo, classDefinitions);
        RelationDefinition relationDefinition = relationReflector.GetMetadata (relationDefinitions);
        if (relationDefinition != null)
          relations.Add (relationDefinition);
      }

      return relations;
    }

    protected MappingException CreateMappingException (Exception innerException, Type type, string message, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      StringBuilder messageBuilder = new StringBuilder();
      messageBuilder.AppendFormat (message, args);
      messageBuilder.AppendLine();
      messageBuilder.AppendFormat ("Type: {0}", type.FullName);

      return new MappingException (messageBuilder.ToString(), innerException);
    }

    private ReflectionBasedClassDefinition CreateClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      ValidateType();

      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          GetID(),
          GetStorageSpecificIdentifier(),
          GetStorageProviderID(),
          _type,
          IsAbstract(),
          GetBaseClassDefinition (classDefinitions));

      CreatePropertyDefinitions (classDefinition, GetPropertyInfos());

      return classDefinition;
    }

    private void ValidateType ()
    {
      if (_type.IsGenericType)
        throw CreateMappingException (null, _type.GetGenericTypeDefinition(), "Generic domain objects are not supported.");
      
      if (!IsAbstract())
      {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding;
        ConstructorInfo legacyLoadConstructor = _type.GetConstructor (flags, null, new Type[] {typeof (DataContainer)}, null);
        if (legacyLoadConstructor != null)
        {
          throw CreateMappingException (
              null,
              _type,
              "The domain object type has a legacy infrastructure constructor for loading (a nonpublic constructor taking a single DataContainer "
              + "argument). The reflection-based mapping does not use this constructor any longer and requires it to be removed.");
        }
      }

      if (IsInheritenceRoot() && Attribute.IsDefined (_type.BaseType, typeof (StorageGroupAttribute), true))
      {
        Type baseType = _type.BaseType;
        while (!AttributeUtility.IsDefined<StorageGroupAttribute> (baseType, false))
          baseType = baseType.BaseType;

        throw CreateMappingException (
            null,
            _type,
            "The domain object type cannot redefine the '{0}' already defined on base type '{1}'.",
            typeof (StorageGroupAttribute),
            baseType);
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

    protected virtual string GetStorageSpecificIdentifier ()
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

    private bool IsInheritenceRoot ()
    {
      if (_type.BaseType == typeof (DomainObject))
        return true;

      return Attribute.IsDefined (_type, typeof (StorageGroupAttribute), false);
    }

    private ReflectionBasedClassDefinition GetBaseClassDefinition (ClassDefinitionCollection classDefinitions)
    {
      if (IsInheritenceRoot())
        return null;

      ClassReflector classReflector = (ClassReflector) TypesafeActivator.CreateInstance (GetType()).With (_type.BaseType);
      return classReflector.GetClassDefinition (classDefinitions);
    }

    private PropertyInfo[] GetPropertyInfos ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (_type, IsInheritenceRoot());
      return propertyFinder.FindPropertyInfos();
    }

    private PropertyInfo[] GetRelationPropertyInfos ()
    {
      RelationPropertyFinder relationPropertyFinder = new RelationPropertyFinder (_type, IsInheritenceRoot());
      return relationPropertyFinder.FindPropertyInfos();
    }
  }
}