using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class ClassDefinitionCollection : BaseCollection
{
  // types

  // static members and constants

  // member fields

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

    ClassDefinition classDefinition = GetByClassID (classID);
    if (classDefinition == null)
      throw CreateMappingException ("Mapping does not contain class '{0}'.", classID);

    return classDefinition;
  }

  public ClassDefinitionCollection GetDirectlyDerivedClassDefinitions (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    ClassDefinitionCollection derivedClasses = new ClassDefinitionCollection ();

    foreach (ClassDefinition definition in this)
    {
      if (definition.BaseClass == classDefinition)
        derivedClasses.Add (definition);
    }

    return derivedClasses;
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return Contains (classDefinition.ClassType);
  }

  public bool Contains (Type classType)
  {
    return base.ContainsKey (classType);
  }

  public ClassDefinition this [int index]  
  {
    get { return (ClassDefinition) GetObject (index); }
  }

  public ClassDefinition this [Type classType]  
  {
    get { return (ClassDefinition) GetObject (classType); }
  }

  public ClassDefinition GetByClassID (string classID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

    foreach (ClassDefinition classDefinition in this)
    {
      if (classDefinition.ID == classID)
        return classDefinition;
    }

    return null;
  }

  public void Add (ClassDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);
    base.Add (value.ClassType, value);
  }

  #endregion

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
