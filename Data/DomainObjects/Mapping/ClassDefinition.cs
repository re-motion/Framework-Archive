using System;
using System.Collections;
using System.Runtime.Serialization;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
// Note: All methods of this class are inheritance-aware. Property accessors are not.
[Serializable]
public class ClassDefinition : ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private string _id;
  private string _entityName;
  private Type _classType;
  private string _classTypeName;
  private string _storageProviderID;
  private ClassDefinition _baseClass;
  private ClassDefinitionCollection _derivedClasses;
  private PropertyDefinitionCollection _propertyDefinitions;
  private RelationDefinitionCollection _relationDefinitions;
  
  // Note: _isPartOfMappingConfiguration is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private bool _isPartOfMappingConfiguration;
  
  // construction and disposing

  public ClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName)
      : this (id, entityName, storageProviderID, classTypeName, resolveClassTypeName, null)
  {
  }


  public ClassDefinition (
      string id, 
      string entityName, 
      string storageProviderID, 
      string classTypeName, 
      bool resolveClassTypeName, 
      ClassDefinition baseClass)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNullOrEmpty ("classTypeName", classTypeName);

    Initialize (id, entityName, storageProviderID, null, classTypeName, resolveClassTypeName, baseClass);
  }

  public ClassDefinition (string id, string entityName, string storageProviderID, Type classType)
      : this (id, entityName, storageProviderID, classType, null)
  {
  }

  public ClassDefinition (
      string id, 
      string entityName, 
      string storageProviderID, 
      Type classType, 
      ClassDefinition baseClass)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNull ("classType", classType);

    Initialize (id, entityName, storageProviderID, classType, null, false, baseClass);
  }

  private void Initialize (
      string id, 
      string entityName, 
      string storageProviderID, 
      Type classType, 
      string classTypeName, 
      bool resolveClassTypeName, 
      ClassDefinition baseClass)
  {
    if (resolveClassTypeName)
      classType = Type.GetType (classTypeName, true);

    if (classType != null)
    {
      CheckClassType (id, classType);
      classTypeName = classType.AssemblyQualifiedName;
    }

    _id = id;
    _entityName = entityName;
    _classType = classType;
    _classTypeName = classTypeName;
    _storageProviderID = storageProviderID;
    
    _derivedClasses = new ClassDefinitionCollection (new ClassDefinitionCollection (), true);
    _propertyDefinitions = new PropertyDefinitionCollection (this);
    _relationDefinitions = new RelationDefinitionCollection ();

    if (baseClass != null)
    {
      // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
      //       initialized to an empty collection during construction.
      CheckBaseClass (baseClass, id, entityName, storageProviderID);
      PerformSetBaseClass (baseClass);
    }  
  }

  protected ClassDefinition (SerializationInfo info, StreamingContext context)
  {
    _id = info.GetString ("ID");
    _isPartOfMappingConfiguration = info.GetBoolean ("IsPartOfMappingConfiguration");

    if (!_isPartOfMappingConfiguration)
    {
      _entityName = info.GetString ("EntityName");
      _classType = (Type) info.GetValue ("ClassType", typeof (Type));
      _classTypeName = info.GetString ("ClassTypeName");
      _storageProviderID = info.GetString ("StorageProviderID");
      _baseClass = (ClassDefinition) info.GetValue ("BaseClass", typeof (ClassDefinition));
      _derivedClasses = (ClassDefinitionCollection) info.GetValue ("DerivedClasses", typeof (ClassDefinitionCollection));
      _propertyDefinitions = (PropertyDefinitionCollection) info.GetValue ("PropertyDefinitions", typeof (PropertyDefinitionCollection));
      _relationDefinitions = (RelationDefinitionCollection) info.GetValue ("RelationDefinitions", typeof (RelationDefinitionCollection));
    }
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

  public bool Contains (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    return _propertyDefinitions.Contains (propertyDefinition);
  }

  public IRelationEndPointDefinition GetOppositeEndPointDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);
    RelationDefinition relationDefinition = GetRelationDefinition (propertyName);

    if (relationDefinition != null && relationEndPointDefinition != null)
      return relationDefinition.GetOppositeEndPointDefinition (relationEndPointDefinition);

    return null;
  }

  public IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    IRelationEndPointDefinition relationEndPointDefinition = GetMandatoryRelationEndPointDefinition (propertyName);
    RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
    return relationDefinition.GetMandatoryOppositeRelationEndPointDefinition (relationEndPointDefinition);
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
      {
        if (!relations.Contains (baseRelation))
          relations.Add (baseRelation);
      }
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
        if (IsMyRelationEndPoint (endPointDefinition))
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

  public RelationDefinition GetMandatoryRelationDefinition (string propertyName)
  {
    RelationDefinition relationDefinition = GetRelationDefinition (propertyName);
    if (relationDefinition == null)
      throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

    return relationDefinition;
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

  public ClassDefinition GetMandatoryOppositeClassDefinition (string propertyName)
  {
    ClassDefinition oppositeClassDefinition = GetOppositeClassDefinition (propertyName);

    if (oppositeClassDefinition == null)
      throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

    return oppositeClassDefinition;
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

  public IRelationEndPointDefinition GetMandatoryRelationEndPointDefinition (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);

    if (relationEndPointDefinition == null)
      throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

    return relationEndPointDefinition;
  }

  public bool IsMyRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

    return (relationEndPointDefinition.ClassDefinition == this && !relationEndPointDefinition.IsNull);
  }

  public bool IsRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

    if (IsMyRelationEndPoint (relationEndPointDefinition))
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

  public PropertyDefinition GetMandatoryPropertyDefinition (string propertyName)
  {
    PropertyDefinition propertyDefinition = GetPropertyDefinition (propertyName);

    if (propertyDefinition == null)
      throw CreateMappingException ("Class '{0}' does not contain the property '{1}'.", ID, propertyName);

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

  public string ClassTypeName
  {
    get { return _classTypeName; }
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

    if (baseClass == this)
      throw CreateMappingException ("Class '{0}' cannot refer to itself as base class.", _id);

    CheckBaseClass (baseClass, _id, _entityName, _storageProviderID);
    CheckBasePropertyDefinitions (baseClass);

    PerformSetBaseClass (baseClass);
  }

  private void CheckBasePropertyDefinitions (ClassDefinition baseClass)
  {
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
  }

  private void PerformSetBaseClass (ClassDefinition baseClass)
  {
    _baseClass = baseClass;
    _baseClass.AddDerivedClass (this);
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

  internal void PropertyDefinitions_Adding (object sender, PropertyDefinitionAddingEventArgs args)
  {
    PropertyDefinitionCollection allPropertyDefinitions = GetPropertyDefinitions ();
    if (allPropertyDefinitions.Contains (args.PropertyDefinition.PropertyName))
    {
      throw CreateMappingException ("Class '{0}' already contains the property '{1}'.",
          _id, args.PropertyDefinition.PropertyName);
    }
  }

  internal void PropertyDefinitions_Added (object sender, PropertyDefinitionAddedEventArgs args)
  {
    args.PropertyDefinition.SetClassDefinition (this);
  }

  #region ISerializable Members

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    GetObjectData (info, context);
  }

  protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("ID", _id);

    bool isPartOfMappingConfiguration = MappingConfiguration.Current.Contains (this);
    info.AddValue ("IsPartOfMappingConfiguration", isPartOfMappingConfiguration);

    if (!isPartOfMappingConfiguration)
    {
      info.AddValue ("EntityName", _entityName);
      info.AddValue ("ClassType", _classType);
      info.AddValue ("ClassTypeName", _classTypeName);
      info.AddValue ("StorageProviderID", _storageProviderID);
      info.AddValue ("BaseClass", _baseClass);
      info.AddValue ("DerivedClasses", _derivedClasses);
      info.AddValue ("PropertyDefinitions", _propertyDefinitions);
      info.AddValue ("RelationDefinitions", _relationDefinitions);
    }
  }

  #endregion

  #region IObjectReference Members

  object IObjectReference.GetRealObject (StreamingContext context)
  {
    if (_isPartOfMappingConfiguration)
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_id);
    else
      return this;
  }

  #endregion
}
}
