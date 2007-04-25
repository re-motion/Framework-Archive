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
    private Type _classType;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract)
        : this (id, entityName, storageProviderID, classType, isAbstract, null)
    {
    }

    public ReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, true)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Rubicon.Data.DomainObjects.DomainObject'.", classType, ID);
     
      _classType = classType;
      _isAbstract = isAbstract;
      _storageSpecificPrefix = string.IsNullOrEmpty (entityName) ? null : (entityName + "_");

      if (baseClass != null)
      {
        // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
        //       initialized to an empty collection during construction.
        SetBaseClass (baseClass);
      }
    }

    protected ReflectionBasedClassDefinition (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      if (!IsPartOfMappingConfiguration)
      {
        _classType = (Type) info.GetValue ("ClassType", typeof (Type));
      }
    }

    public new ReflectionBasedClassDefinition BaseClass
    {
      get { return (ReflectionBasedClassDefinition) base.BaseClass; }
    }

    public override bool IsAbstract
    {
      get { return _isAbstract; }
    }

    public override string MyStorageSpecificPrefix
    {
      get { return _storageSpecificPrefix; }
    }

    public override Type ClassType
    {
      get { return _classType; }
    }

    public override bool IsClassTypeResolved
    {
      get { return true; }
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

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("ClassType", _classType);
    }
  }
}