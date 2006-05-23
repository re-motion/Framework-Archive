using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.Mapping
{
[Serializable]
public class ClassDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  private Hashtable _types = new Hashtable ();
  private bool _areResolvedTypesRequired;

  // construction and disposing

  public ClassDefinitionCollection () : this (true)
  {
  }

  public ClassDefinitionCollection (bool areResolvedTypesRequired)
  {
    _areResolvedTypesRequired = areResolvedTypesRequired;
  }

  // standard constructor for collections

  public ClassDefinitionCollection (ClassDefinitionCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (ClassDefinition classDefinition in collection)
    {
      Add (classDefinition);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public void Validate ()
  {
    ClassDefinitionCollection rootClasses = new ClassDefinitionCollection (this.AreResolvedTypesRequired);
    foreach (ClassDefinition classDefinition in this)
    {
      ClassDefinition rootClassDefinition = classDefinition.GetInheritanceRootClass ();
      if (!rootClasses.Contains (rootClassDefinition))
        rootClasses.Add (rootClassDefinition);
    }

    foreach (ClassDefinition rootClass in rootClasses)
    {
      ValidateRootClass (rootClass);
    }
  }

  public ClassDefinition GetMandatory (Type classType)
  {
    ArgumentUtility.CheckNotNull ("classType", classType);

    if (!_areResolvedTypesRequired)
    {
      throw CreateInvalidOperationException (
          "Collection allows only ClassDefinitions with resolved types and therefore GetMandatory(Type) cannot be used.");
    }

    ClassDefinition classDefinition = this[classType];
    if (classDefinition == null)
      throw CreateMappingException ("Mapping does not contain class '{0}'.", classType);

    return classDefinition;
  }

  public ClassDefinition GetMandatory (string classID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    ClassDefinition classDefinition = this[classID];
    if (classDefinition == null)
      throw CreateMappingException ("Mapping does not contain class '{0}'.", classID);

    return classDefinition;
  }

  public bool AreResolvedTypesRequired
  {
    get { return _areResolvedTypesRequired; }
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return BaseContains (classDefinition.ID, classDefinition);
  }

  public bool Contains (Type classType)
  {
    if (!_areResolvedTypesRequired)
    {
      throw CreateInvalidOperationException (
          "Collection allows only ClassDefinitions with resolved types and therefore Contains(Type) cannot be used.");
    }

    return _types.ContainsKey (classType);
  }

  public bool Contains (string classID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    return BaseContainsKey (classID);
  }

  public ClassDefinition this [int index]  
  {
    get { return (ClassDefinition) BaseGetObject (index); }
  }

  public ClassDefinition this [Type classType]  
  {
    get 
    {
      if (!_areResolvedTypesRequired)
      {
        throw CreateInvalidOperationException (
            "Collection allows only ClassDefinitions with resolved types and therefore this overload of the indexer cannot be used.");
      }

      return (ClassDefinition) _types[classType]; 
    }
  }

  public ClassDefinition this [string classID]
  {
    get
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return (ClassDefinition) BaseGetObject (classID);
    }
  }

  public int Add (ClassDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
  
    if (_areResolvedTypesRequired)
    {
      if (value.ClassType == null)
      {
        throw CreateInvalidOperationException (
            "Collection allows only ClassDefinitions with resolved types and therefore ClassDefinition '{0}' cannot be added.", value.ID);
      }

      if (_types.Contains (value.ClassType))
        throw new ArgumentException (string.Format ("A ClassDefinition with Type '{0}' is already part of this collection.", value.ClassType), "value");
    }

    int position = BaseAdd (value.ID, value);

    if (_areResolvedTypesRequired)
      _types.Add (value.ClassType, value);

    return position;
  }

  #endregion

  private void ValidateRootClass (ClassDefinition rootClass)
  {
    if (!Contains (rootClass))
    {
      throw CreateInvalidOperationException (
          "Validate cannot be invoked, because class '{0}' is a base class of a class in the collection,"
          + " but the base class is not part of the collection itself.",
          rootClass.ID);
    }

    foreach (ClassDefinition derivedClass in rootClass.GetAllDerivedClasses ())
    {
      if (!Contains (derivedClass))
      {
        throw CreateInvalidOperationException (
            "Validate cannot be invoked, because class '{0}' is a derived class of '{1}', but is not part of the collection itself.",
            derivedClass.ID, rootClass.ID);
      }
    }

    rootClass.ValidateInheritanceHierarchy (new Dictionary<string, PropertyDefinition> ());
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }

  private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
  {
    return new InvalidOperationException (string.Format (message, args));
  }
}
}
