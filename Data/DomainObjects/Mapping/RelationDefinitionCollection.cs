using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Mapping
{
public class RelationDefinitionCollection : CollectionBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationDefinitionCollection ()
  {
  }

  // standard constructor for collections
  public RelationDefinitionCollection (RelationDefinitionCollection collection, bool isCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (RelationDefinition relationDefinition in collection)
    {
      Add (relationDefinition);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
  }

  // methods and properties

  public RelationDefinition GetMandatory (string relationDefinitionID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("relationDefinitionID", relationDefinitionID);

    RelationDefinition relationDefinition = this[relationDefinitionID];
    if (relationDefinition != null)
      return relationDefinition;
    else
      throw CreateMappingException ("Relation '{0}' does not exist.", relationDefinitionID);
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    return Contains (relationDefinition.ID);
  }

  public bool Contains (string relationDefinitionID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("relationDefinitionID", relationDefinitionID);
    return base.ContainsKey (relationDefinitionID);
  }

  public RelationDefinition this [int index]  
  {
    get { return (RelationDefinition) GetObject (index); }
  }

  public RelationDefinition this [string relationDefinitionID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("relationDefinitionID", relationDefinitionID);
      return (RelationDefinition) GetObject (relationDefinitionID); 
    }
  }

  public void Add (RelationDefinition value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    base.Add (value.ID, value);
  }

  #endregion

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
