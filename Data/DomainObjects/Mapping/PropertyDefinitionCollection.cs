using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class PropertyDefinitionCollection : CollectionBase
{
  // types

  // static members and constants

  // member fields

  public event PropertyDefinitionAddingEventHandler Adding;
  public event PropertyDefinitionAddedEventHandler Added;

  // construction and disposing

  public PropertyDefinitionCollection ()
  {
  }

  // standard constructor for collections
  public PropertyDefinitionCollection (PropertyDefinitionCollection collection, bool isCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (PropertyDefinition propertyDefinition in collection)
    {
      Add (propertyDefinition);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
  }

  // methods and properties

  protected virtual void OnAdding (PropertyDefinitionAddingEventArgs args)
  {
    if (Adding != null)
      Adding (this, args);
  }

  protected virtual void OnAdded (PropertyDefinitionAddedEventArgs args)
  {
    if (Added != null)
      Added (this, args);
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    return Contains (propertyDefinition.PropertyName);
  }

  public bool Contains (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return base.ContainsKey (propertyName);
  }

  public PropertyDefinition this [int index]  
  {
    get { return (PropertyDefinition) GetObject (index); }
  }

  public PropertyDefinition this [string propertyName]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      return (PropertyDefinition) GetObject (propertyName); 
    }
  }

  public void Add (PropertyDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    OnAdding (new PropertyDefinitionAddingEventArgs (value));
    base.Add (value.PropertyName, value);
    OnAdded (new PropertyDefinitionAddedEventArgs (value));
  }

  #endregion
}
}