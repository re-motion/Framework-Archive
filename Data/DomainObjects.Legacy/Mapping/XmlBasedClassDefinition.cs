using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mappign from an xml-file.</summary>
  [Serializable]
  public class XmlBasedClassDefinition: ClassDefinition
  {
    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType)
        : base (id, entityName, storageProviderID, classType)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classTypeName, resolveClassTypeName, baseClass)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName)
        : base (id, entityName, storageProviderID, classTypeName, resolveClassTypeName)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classType, baseClass)
    {
    }

    public XmlBasedClassDefinition (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    public new XmlBasedClassDefinition BaseClass
    {
      get { return (XmlBasedClassDefinition) base.BaseClass; }
    }

    public override bool IsAbstract
    {
      get
      {
        if (ClassType == null)
          throw CreateInvalidOperationException ("Cannot evaluate IsAbstract for ClassDefinition '{0}' since ResolveTypeNames is false.", ID);

        return ClassType.IsAbstract;
      }
    }

    public override string StorageSpecificPrefix
    {
      get { return string.Empty; }
    }

    public override void ValidateInheritanceHierarchy (Dictionary<string, List<PropertyDefinition>> allPropertyDefinitionsInInheritanceHierarchy)
    {
      ArgumentUtility.CheckNotNull ("allPropertyDefinitionsInInheritanceHierarchy", allPropertyDefinitionsInInheritanceHierarchy);

      base.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);

      foreach (PropertyDefinition myPropertyDefinition in MyPropertyDefinitions)
      {
        List<PropertyDefinition> basePropertyDefinitions;
        if (allPropertyDefinitionsInInheritanceHierarchy.TryGetValue (myPropertyDefinition.StorageSpecificName, out basePropertyDefinitions) 
            && basePropertyDefinitions != null && basePropertyDefinitions.Count > 0)
        {
          PropertyDefinition basePropertyDefinition = basePropertyDefinitions[0];

          throw CreateMappingException (
              "Property '{0}' of class '{1}' must not define column name '{2}',"
              + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same column name.",
              myPropertyDefinition.PropertyName,
              ID,
              myPropertyDefinition.StorageSpecificName,
              basePropertyDefinition.ClassDefinition.ID,
              basePropertyDefinition.PropertyName);
        }

        allPropertyDefinitionsInInheritanceHierarchy[myPropertyDefinition.StorageSpecificName] = 
            new List<PropertyDefinition> (new PropertyDefinition[] { myPropertyDefinition });
      }

      foreach (XmlBasedClassDefinition derivedClassDefinition in DerivedClasses)
        derivedClassDefinition.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}