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
    private Type _classType;
    private string _classTypeName;

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType)
        : this (id, entityName, storageProviderID, classType, null)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, true)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);

      Initialize (classType, null, false, baseClass);
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName)
        : this (id, entityName, storageProviderID, classTypeName, resolveClassTypeName, null)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, resolveClassTypeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("classTypeName", classTypeName);

      Initialize (null, classTypeName, resolveClassTypeName, baseClass);
    }

    private void Initialize (Type classType, string classTypeName, bool resolveClassTypeName, XmlBasedClassDefinition baseClass)
    {
      if (resolveClassTypeName)
        classType = Type.GetType (classTypeName, true);

      if (classType != null)
      {
        CheckClassType (ID, classType);
        classTypeName = classType.AssemblyQualifiedName;
      }

      _classType = classType;
      _classTypeName = classTypeName;

      if (baseClass != null)
      {
        // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
        //       initialized to an empty collection during construction.
        SetBaseClass (baseClass);
      }
    }

    private void CheckClassType (string classID, Type classType)
    {
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Rubicon.Data.DomainObjects.DomainObject'.", classType, classID);
    }

    public XmlBasedClassDefinition (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
      if (!IsPartOfMappingConfiguration)
      {
        _classType = (Type) info.GetValue ("ClassType", typeof (Type));
        _classTypeName = info.GetString ("ClassTypeName");
      }
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

    public override string MyStorageSpecificPrefix
    {
      get { return string.Empty; }
    }

    public override Type ClassType
    {
      get { return _classType; }
    }

    public string ClassTypeName
    {
      get { return _classTypeName; }
    }

    public override bool IsClassTypeResolved
    {
      get { return (_classType != null); }
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

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("ClassType", _classType);
      info.AddValue ("ClassTypeName", _classTypeName);
    }
  }
}