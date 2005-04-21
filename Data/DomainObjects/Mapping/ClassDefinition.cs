using System;
using System.Collections;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
// Note: All methods of this class are inheritance-aware. Property accessors are not.
public class ClassDefinition
{
  // types

  // static members and constants

  // member fields

  private string _id;
  private string _entityName;
  private Type _classType;
  private string _storageProviderID;
  private ClassDefinition _baseClass;
  private ClassDefinitionCollection _derivedClasses;
  private PropertyDefinitionCollection _propertyDefinitions;
  private RelationDefinitionCollection _relationDefinitions;
  
  // construction and disposing

  public ClassDefinition (string id, string entityName, Type classType, string storageProviderID)
      : this (id, entityName, classType, storageProviderID, null)
  {
  }

  public ClassDefinition (
      string id, 
      string entityName, 
      Type classType, 
      string storageProviderID, 
      ClassDefinition baseClass)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
    ArgumentUtility.CheckNotNull ("classType", classType);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    CheckClassType (id, classType);
    
    if (baseClass != null)
      CheckBaseClass (baseClass, id, entityName, storageProviderID);

    _id = id;
    _entityName = entityName;
    _classType = classType;
    _storageProviderID = storageProviderID;
    PerformSetBaseClass (baseClass);

    _derivedClasses = new ClassDefinitionCollection (new ClassDefinitionCollection (), true);
    _propertyDefinitions = new PropertyDefinitionCollection ();
    _relationDefinitions = new RelationDefinitionCollection ();

    _propertyDefinitions.Adding += new PropertyDefinitionAddingEventHandler(PropertyDefinitions_Adding);
  }

  private void CheckClassType (string classID, Type classType)
  {
    if (!classType.IsSubclassOf (typeof (DomainObject)))
    {
      throw CreateMappingException ("Type '{0}' of class '{1}'" 
        + " is not derived from 'Rubicon.Data.DomainObjects.DomainObject'.", classType, classID);
    }
  }

  // methods and properties
 
  public IRelationEndPointDefinition GetOppositeEndPointDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    IRelationEndPointDefinition relationEndPointDefinition = GetMandatoryRelationEndPointDefinition (propertyName);
    RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
    return relationDefinition.GetMandatoryOppositeRelationEndPointDefinition (relationEndPointDefinition);
  }

  public IRelationEndPointDefinition GetMandatoryRelationEndPointDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);
    if (relationEndPointDefinition == null)
    {
      throw CreateMappingException (
          "No relation found for class '{0}' and property '{1}'.", ID, propertyName);
    }

    return relationEndPointDefinition;
  }

  public PropertyDefinitionCollection GetPropertyDefinitions ()
  {
    PropertyDefinitionCollection propertyDefinitions = new PropertyDefinitionCollection (
        _propertyDefinitions, false);
    
    if (_baseClass != null)
    {
      foreach (PropertyDefinition basePropertyDefinition in _baseClass.GetPropertyDefinitions ())
        propertyDefinitions.Add (basePropertyDefinition);
    }

    return propertyDefinitions;
  }

  public RelationDefinitionCollection GetRelationDefinitions ()
  {
    RelationDefinitionCollection relations = new RelationDefinitionCollection (_relationDefinitions, false);

    if (_baseClass != null)
    {
      foreach (RelationDefinition baseRelation in _baseClass.GetRelationDefinitions ())
        relations.Add (baseRelation);
    }

    return relations;
  }

  public IRelationEndPointDefinition[] GetRelationEndPointDefinitions ()
  {
    ArrayList relationEndPointDefinitions = new ArrayList ();

    foreach (IRelationEndPointDefinition relationEndPointDefinition in GetMyRelationEndPointDefinitions ())
      relationEndPointDefinitions.Add (relationEndPointDefinition);

    if (_baseClass != null)
    {
      foreach (IRelationEndPointDefinition baseRelationEndPointDefinition in _baseClass.GetRelationEndPointDefinitions ())
        relationEndPointDefinitions.Add (baseRelationEndPointDefinition);
    }

    return (IRelationEndPointDefinition[]) relationEndPointDefinitions.ToArray (typeof (IRelationEndPointDefinition));
  }

  public IRelationEndPointDefinition[] GetMyRelationEndPointDefinitions ()
  {
    ArrayList relationEndPointDefinitions = new ArrayList ();

    foreach (RelationDefinition relationDefinition in _relationDefinitions)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        if (IsRelationEndPoint (endPointDefinition))
          relationEndPointDefinitions.Add (endPointDefinition);
      }
    }

    return (IRelationEndPointDefinition[]) relationEndPointDefinitions.ToArray (typeof (IRelationEndPointDefinition));
  }

  public RelationDefinition GetRelationDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    foreach (RelationDefinition relationDefinition in _relationDefinitions)
    {
      if (relationDefinition.IsEndPoint (_id, propertyName))
        return relationDefinition;
    }

    if (_baseClass != null)
      return _baseClass.GetRelationDefinition (propertyName);

    return null;
  }

  public ClassDefinition GetOppositeClassDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
    if (relationDefinition == null)
      return null;

    ClassDefinition oppositeClass = relationDefinition.GetOppositeClassDefinition (_id, propertyName);
    
    if (oppositeClass != null)
      return oppositeClass;

    if (_baseClass != null)
      return _baseClass.GetOppositeClassDefinition (propertyName);

    return null;
  }

  public IRelationEndPointDefinition GetRelationEndPointDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    foreach (RelationDefinition relationDefinition in _relationDefinitions)
    {
      IRelationEndPointDefinition relationEndPointDefinition = relationDefinition.GetEndPointDefinition (_id, propertyName);
      if (relationEndPointDefinition != null)
        return relationEndPointDefinition;
    }

    if (_baseClass != null)
      return _baseClass.GetRelationEndPointDefinition (propertyName);

    return null;
  }

  public bool IsRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

    if (relationEndPointDefinition.ClassDefinition == this && !relationEndPointDefinition.IsNull)
      return true;

    if (_baseClass != null)
      return _baseClass.IsRelationEndPoint (relationEndPointDefinition);

    return false;
  }

  public PropertyDefinition GetPropertyDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    PropertyDefinition propertyDefinition = _propertyDefinitions[propertyName];

    if (propertyDefinition == null && _baseClass != null)
      return _baseClass.GetPropertyDefinition (propertyName);

    return propertyDefinition;
  }

  public PropertyDefinition this [string propertyName]
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      return _propertyDefinitions[propertyName]; 
    }
  }
  
  public string ID
  {
    get { return _id; }
  }

  public string EntityName
  {
    get { return _entityName; }
  }

  public Type ClassType
  {
    get { return _classType; }
  }

  public string StorageProviderID
  {
    get { return _storageProviderID; }
  }

  public ClassDefinition BaseClass
  {
    get { return _baseClass; }
  }

  public ClassDefinitionCollection DerivedClasses
  {
    get { return _derivedClasses; }
  }

  public PropertyDefinitionCollection MyPropertyDefinitions
	{
		get { return _propertyDefinitions; }
	}

  public RelationDefinitionCollection MyRelationDefinitions
  {
    get { return _relationDefinitions; }
  }

  public bool IsPartOfInheritanceHierarchy
  {
    get { return (_baseClass != null || _derivedClasses.Count > 0); }
  }

  private void CheckBaseClass (
      ClassDefinition baseClass,
      string id,
      string entityName, 
      string storageProviderID)
  {
    if (baseClass.EntityName != entityName)
    {
      throw CreateMappingException (
          "Entity name ('{0}') of class '{1}' and entity name ('{2}') of its base class '{3}' must be equal.",
          entityName, id, baseClass.EntityName, baseClass.ID);
    }

    if (baseClass.StorageProviderID != storageProviderID)
    {
      throw CreateMappingException (
          "Cannot derive class '{0}' from base class '{1}' handled by different StorageProviders.",
          id, baseClass.ID);    
    }
  }

  internal void SetBaseClass (ClassDefinition baseClass)
  {
    ArgumentUtility.CheckNotNull ("baseClass", baseClass);

    CheckBaseClass (baseClass, _id, _entityName, _storageProviderID);

    if (baseClass == this)
      throw CreateMappingException ("Class '{0}' cannot refer to itself as base class.", _id);

    PropertyDefinitionCollection basePropertyDefinitions = baseClass.GetPropertyDefinitions ();
    foreach (PropertyDefinition propertyDefinition in _propertyDefinitions)
    {
      if (basePropertyDefinitions.Contains (propertyDefinition.PropertyName))
      {
        throw CreateMappingException ("Class '{0}' cannot be set as base class for class"
            + " '{1}', because the property '{2}' is defined in both classes.",
            baseClass.ID, this.ID, propertyDefinition.PropertyName);
      }

      if (basePropertyDefinitions.ContainsColumnName (propertyDefinition.ColumnName))
      {
        throw CreateMappingException (
            "Property '{0}' of class '{1}' inherits a property which already defines the column '{2}'.",
            propertyDefinition.PropertyName, this.ID, propertyDefinition.ColumnName);
      }
    }

    PerformSetBaseClass (baseClass);
  }

  private void PerformSetBaseClass (ClassDefinition baseClass)
  {
    _baseClass = baseClass;

    if (baseClass != null)
      baseClass.AddDerivedClass (this);
  }

  private void AddDerivedClass (ClassDefinition derivedClass)
  {
    ClassDefinitionCollection derivedClasses = new ClassDefinitionCollection (_derivedClasses, false);
    derivedClasses.Add (derivedClass);
    _derivedClasses = new ClassDefinitionCollection (derivedClasses, true);
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  private void PropertyDefinitions_Adding (object sender, PropertyDefinitionAddingEventArgs args)
  {
    PropertyDefinitionCollection allPropertyDefinitions = GetPropertyDefinitions ();
    if (allPropertyDefinitions.Contains (args.PropertyDefinition.PropertyName))
    {
      throw CreateMappingException ("Class '{0}' already contains the property '{1}'.",
          _id, args.PropertyDefinition.PropertyName);
    }    
  }
}
}
