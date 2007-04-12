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

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract)
        : this (id, entityName, storageProviderID, classType, isAbstract, null)
    {
    }

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classType, (ClassDefinition) baseClass)
    {
      _isAbstract = isAbstract;
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

    //TODO: checks
    protected internal override void ValidateInheritanceHierarchy (Dictionary<string, PropertyDefinition> allPropertyDefinitionsInInheritanceHierarchy)
    {
      ArgumentUtility.CheckNotNull ("allPropertyDefinitionsInInheritanceHierarchy", allPropertyDefinitionsInInheritanceHierarchy);

      base.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);
    }
  }
}