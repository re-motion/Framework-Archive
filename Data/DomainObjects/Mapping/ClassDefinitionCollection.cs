using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class ClassDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  private Hashtable _classIDs = new Hashtable ();

  // construction and disposing

  public ClassDefinitionCollection ()
  {
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

  public ClassDefinition GetMandatory (Type classType)
  {
    ArgumentUtility.CheckNotNull ("classType", classType);

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

  #region Standard implementation for "add-only" collections

  public bool Contains (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return Contains (classDefinition.ClassType);
  }

  public bool Contains (Type classType)
  {
    return BaseContainsKey (classType);
  }

  public bool Contains (string classID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    return _classIDs.Contains (classID);
  }

  public ClassDefinition this [int index]  
  {
    get { return (ClassDefinition) BaseGetObject (index); }
  }

  public ClassDefinition this [Type classType]  
  {
    get { return (ClassDefinition) BaseGetObject (classType); }
  }

  public ClassDefinition this [string classID]
  {
    get
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return (ClassDefinition) _classIDs[classID];
    }
  }

  public int Add (ClassDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
  
    if (_classIDs.Contains (value.ID))
      throw new ArgumentException (string.Format ("A ClassDefinition with ID '{0}' is already part of this collection.", value.ID), "value");

    int position = BaseAdd (value.ClassType, value);
    _classIDs.Add (value.ID, value);

    return position;
  }

  #endregion

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
