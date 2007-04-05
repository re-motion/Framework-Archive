using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.Mapping
{
  [Serializable]
  public class XmlBasedClassDefinition:ClassDefinition
  {
    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType)
        : base (id, entityName, storageProviderID, classType)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName, ClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classTypeName, resolveClassTypeName, baseClass)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classType, isAbstract, baseClass)
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

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract)
        : base (id, entityName, storageProviderID, classType, isAbstract)
    {
    }

    public XmlBasedClassDefinition (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    protected override void ValidateInheritanceHierarchy (Dictionary<string, PropertyDefinition> allPropertyDefinitionsInInheritanceHierarchy)
    {
      ArgumentUtility.CheckNotNull ("allPropertyDefinitionsInInheritanceHierarchy", allPropertyDefinitionsInInheritanceHierarchy);

      base.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);

      if (BaseClass != null)
      {
        PropertyDefinitionCollection basePropertyDefinitions = BaseClass.GetPropertyDefinitions ();
        foreach (PropertyDefinition propertyDefinition in MyPropertyDefinitions)
        {
          if (basePropertyDefinitions.Contains (propertyDefinition.PropertyName))
          {
            throw CreateMappingException (
                "Class '{0}' must not define property '{1}', because base class '{2}' already defines a property with the same name.",
                ID,
                propertyDefinition.PropertyName,
                basePropertyDefinitions[propertyDefinition.PropertyName].ClassDefinition.ID);
          }
        }
      }

      foreach (PropertyDefinition myPropertyDefinition in MyPropertyDefinitions)
      {
        if (allPropertyDefinitionsInInheritanceHierarchy.ContainsKey (myPropertyDefinition.StorageSpecificName))
        {
          PropertyDefinition basePropertyDefinition = allPropertyDefinitionsInInheritanceHierarchy[myPropertyDefinition.StorageSpecificName];

          throw CreateMappingException (
              "Property '{0}' of class '{1}' must not define column name '{2}',"
              + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same column name.",
              myPropertyDefinition.PropertyName,
              ID,
              myPropertyDefinition.StorageSpecificName,
              basePropertyDefinition.ClassDefinition.ID,
              basePropertyDefinition.PropertyName);
        }

        allPropertyDefinitionsInInheritanceHierarchy.Add (myPropertyDefinition.StorageSpecificName, myPropertyDefinition);
      }

      foreach (XmlBasedClassDefinition derivedClassDefinition in DerivedClasses)
        derivedClassDefinition.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }
  }
}