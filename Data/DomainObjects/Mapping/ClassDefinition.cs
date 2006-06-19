using System;
using System.Collections;
using System.Runtime.Serialization;

using Rubicon.Utilities;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.Mapping
{
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
      bool resolveClassType, 
      ClassDefinition baseClass)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    if (entityName == string.Empty) throw new ArgumentEmptyException ("entityName");
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNullOrEmpty ("classTypeName", classTypeName);

    Initialize (id, entityName, storageProviderID, null, classTypeName, resolveClassType, baseClass);
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
    if (entityName == string.Empty) throw new ArgumentEmptyException ("entityName");
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
      bool resolveClassType, 
      ClassDefinition baseClass)
  {
    if (resolveClassType)
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

    _derivedClasses = new ClassDefinitionCollection (new ClassDefinitionCollection (resolveClassType), true);
    _propertyDefinitions = new PropertyDefinitionCollection (this);
    _relationDefinitions = new RelationDefinitionCollection ();

    if (baseClass != null)
    {
      // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
      //       initialized to an empty collection during construction.
      CheckBaseClass (baseClass, id, storageProviderID, classType);
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

  public bool IsSameOrBaseClassOf (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    if (object.ReferenceEquals (this, classDefinition))
      return true;

    ClassDefinition baseClassOfProvidedClassDefinition = classDefinition.BaseClass;
    while (baseClassOfProvidedClassDefinition != null)
    {
      if (object.ReferenceEquals (this, baseClassOfProvidedClassDefinition))
        return true;

      baseClassOfProvidedClassDefinition = baseClassOfProvidedClassDefinition.BaseClass;
    }

    return false;
  }

  public string[] GetAllConcreteEntityNames ()
  {
    if (GetEntityName () != null)
      return new string[] { GetEntityName () };

    List<string> allConcreteEntityNames = new List<string> ();
    FillAllConcreteEntityNames (allConcreteEntityNames);

    return allConcreteEntityNames.ToArray ();
  }

  public ClassDefinitionCollection GetAllDerivedClasses ()
  {
    bool areResolvedTypeNamesRequired = (_classType != null);
    ClassDefinitionCollection allDerivedClasses = new ClassDefinitionCollection (areResolvedTypeNamesRequired);
    FillAllDerivedClasses (allDerivedClasses);
    return allDerivedClasses;
  }

  public ClassDefinition GetInheritanceRootClass ()
  {
    if (_baseClass != null)
      return _baseClass.GetInheritanceRootClass ();

    return this;
  }

  public string GetEntityName ()
  {
    if (_entityName != null)
      return _entityName;

    if (_baseClass == null)
      return null;

    return _baseClass.GetEntityName ();
  }

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

  public string MyEntityName
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

  public bool IsClassTypeResolved
  {
    get { return (_classType != null); }
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

  internal void SetBaseClass (ClassDefinition baseClass)
  {
    ArgumentUtility.CheckNotNull ("baseClass", baseClass);

    if (baseClass == this)
      throw CreateMappingException ("Class '{0}' cannot refer to itself as base class.", _id);

    CheckBaseClass (baseClass, _id, _storageProviderID, _classType);
    PerformSetBaseClass (baseClass);
  }

  internal void ValidateInheritanceHierarchy (Dictionary<string, PropertyDefinition> allPropertyDefinitionsInInheritanceHierarchy)
  {
    if (_classType != null)
    {
      if (GetEntityName () == null && !_classType.IsAbstract)
      {
        throw CreateMappingException (
            "Type '{0}' must be abstract, because neither class '{1}' nor its base classes specify an entity name.",
            _classType.AssemblyQualifiedName, _id);
      }
    }

    if (_baseClass != null && _entityName != null && _baseClass.GetEntityName () != null && _entityName != _baseClass.GetEntityName ())
    {
      throw CreateMappingException (
          "Class '{0}' must not specify an entity name '{1}' which is different from inherited entity name '{2}'.",
          _id, _entityName, _baseClass.GetEntityName ());
    }

    if (_baseClass != null)
    {
      PropertyDefinitionCollection basePropertyDefinitions = _baseClass.GetPropertyDefinitions ();
      foreach (PropertyDefinition propertyDefinition in _propertyDefinitions)
      {
        if (basePropertyDefinitions.Contains (propertyDefinition.PropertyName))
        {
          throw CreateMappingException (
              "Class '{0}' must not define property '{1}', because base class '{2}' already defines a property with the same name.",
              _id, propertyDefinition.PropertyName, basePropertyDefinitions[propertyDefinition.PropertyName].ClassDefinition.ID);
        }
      }
    }

    foreach (PropertyDefinition myPropertyDefinition in _propertyDefinitions)
    {
      if (allPropertyDefinitionsInInheritanceHierarchy.ContainsKey (myPropertyDefinition.ColumnName))
      {
        PropertyDefinition basePropertyDefinition = allPropertyDefinitionsInInheritanceHierarchy[myPropertyDefinition.ColumnName];

        throw CreateMappingException (
            "Property '{0}' of class '{1}' must not define column name '{2}',"
            + " because class '{3}' in same inheritance hierarchy already defines property '{4}' with the same column name.",
            myPropertyDefinition.PropertyName, _id, myPropertyDefinition.ColumnName,
            basePropertyDefinition.ClassDefinition.ID, basePropertyDefinition.PropertyName);
      }

      allPropertyDefinitionsInInheritanceHierarchy.Add (myPropertyDefinition.ColumnName, myPropertyDefinition);
    }

    foreach (ClassDefinition derivedClassDefinition in _derivedClasses)
      derivedClassDefinition.ValidateInheritanceHierarchy (allPropertyDefinitionsInInheritanceHierarchy);
  }

  internal void PropertyDefinitions_Adding (object sender, PropertyDefinitionAddingEventArgs args)
  {
    if (IsClassTypeResolved != args.PropertyDefinition.IsPropertyTypeResolved)
    {
      if (IsClassTypeResolved)
      {
        throw CreateInvalidOperationException ("The PropertyDefinition '{0}' cannot be added to ClassDefinition '{1}', "
            + "because the ClassDefinition's type is resolved and the PropertyDefinition's type is not.", 
            args.PropertyDefinition.PropertyName, _id);
      }
      else
      {
        throw CreateInvalidOperationException ("The PropertyDefinition '{0}' cannot be added to ClassDefinition '{1}', "
            + "because the PropertyDefinition's type is resolved and the ClassDefinition's type is not.", 
            args.PropertyDefinition.PropertyName, _id);
      }
    }
    
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

  private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
  {
    return new InvalidOperationException (string.Format (message, args));
  }

  private void CheckBaseClass (ClassDefinition baseClass, string id, string storageProviderID, Type classType)
  {
    if (classType != null && baseClass.ClassType != null && !classType.IsSubclassOf (baseClass.ClassType))
    {
      throw CreateMappingException (
          "Type '{0}' of class '{1}' is not derived from type '{2}' of base class '{3}'.",
          classType.AssemblyQualifiedName, id, baseClass.ClassType.AssemblyQualifiedName, baseClass.ID);
    }

    if (baseClass.StorageProviderID != storageProviderID)
    {
      throw CreateMappingException (
          "Cannot derive class '{0}' from base class '{1}' handled by different StorageProviders.",
          id, baseClass.ID);
    }
  }

  private void FillAllConcreteEntityNames (List<string> allConcreteEntityNames)
  {
    if (_entityName != null)
    {
      allConcreteEntityNames.Add (_entityName);
      return;
    }

    foreach (ClassDefinition derivedClass in _derivedClasses)
      derivedClass.FillAllConcreteEntityNames (allConcreteEntityNames);
  }

  private void FillAllDerivedClasses (ClassDefinitionCollection allDerivedClasses)
  {
    foreach (ClassDefinition derivedClass in _derivedClasses)
    {
      allDerivedClasses.Add (derivedClass);
      derivedClass.FillAllDerivedClasses (allDerivedClasses);
    }
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
