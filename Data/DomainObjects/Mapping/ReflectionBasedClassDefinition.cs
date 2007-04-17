using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mapping from the reflection meta data.</summary>
  [Serializable]
  public class ReflectionBasedClassDefinition: ClassDefinition
  {
    private bool _isAbstract;
    private string _storageSpecificPrefix;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract)
        : this (id, entityName, storageProviderID, classType, isAbstract, null)
    {
    }

    public ReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classType, (ClassDefinition) baseClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      _isAbstract = isAbstract;
      _storageSpecificPrefix = id + "_";
    }

    protected ReflectionBasedClassDefinition (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    public new ReflectionBasedClassDefinition BaseClass
    {
      get { return (ReflectionBasedClassDefinition) base.BaseClass; }
    }

    public override bool IsAbstract
    {
      get { return _isAbstract; }
    }

    public override string StorageSpecificPrefix
    {
      get { return _storageSpecificPrefix; }
    }

    public override void ValidateInheritanceHierarchy (Dictionary<string, List<PropertyDefinition>> allPropertyDefinitionsInInheritanceHierarchy)
    {
      ArgumentUtility.CheckNotNull ("allPropertyDefinitionsInInheritanceHierarchy", allPropertyDefinitionsInInheritanceHierarchy);

      base.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);

      foreach (PropertyDefinition myPropertyDefinition in MyPropertyDefinitions)
      {
        List<PropertyDefinition> basePropertyDefinitions;
        if (!allPropertyDefinitionsInInheritanceHierarchy.TryGetValue (myPropertyDefinition.StorageSpecificName, out basePropertyDefinitions))
        {
          basePropertyDefinitions = new List<PropertyDefinition>();
          allPropertyDefinitionsInInheritanceHierarchy.Add (myPropertyDefinition.StorageSpecificName, basePropertyDefinitions);
        }

        foreach (PropertyDefinition basePropertyDefinition in basePropertyDefinitions)
        {
          bool isEntityDefined = GetEntityName() != null;
          bool isEntityDefinedForBaseProperty = basePropertyDefinition.ClassDefinition.GetEntityName() != null;
          bool isBasePropertyPersistedInSameEntity = basePropertyDefinition.ClassDefinition.GetEntityName() == GetEntityName();

          if (!isEntityDefined && !isEntityDefinedForBaseProperty
              || isBasePropertyPersistedInSameEntity
              || isEntityDefined && !isEntityDefinedForBaseProperty && !isBasePropertyPersistedInSameEntity)
          {
            throw CreateMappingException (
                "Property '{0}' of class '{1}' must not define storage specific name '{2}', because class '{3}', "
                + "persisted in the same entity, already defines property '{4}' with the same storage specific name.",
                myPropertyDefinition.PropertyName,
                ID,
                myPropertyDefinition.StorageSpecificName,
                basePropertyDefinition.ClassDefinition.ID,
                basePropertyDefinition.PropertyName);
          }
        }

        basePropertyDefinitions.Add (myPropertyDefinition);
      }

      foreach (ClassDefinition derivedClassDefinition in DerivedClasses)
        derivedClassDefinition.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}