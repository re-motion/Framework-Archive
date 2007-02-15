using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
[Serializable]
public class PropertyDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  public event PropertyDefinitionAddingEventHandler Adding;
  public event PropertyDefinitionAddedEventHandler Added;

  private ClassDefinition _classDefinition;

  // construction and disposing

  public PropertyDefinitionCollection ()
  {
  }

  public PropertyDefinitionCollection (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    _classDefinition = classDefinition;
  }

  // standard constructor for collections
  public PropertyDefinitionCollection (PropertyDefinitionCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (PropertyDefinition propertyDefinition in collection)
    {
      Add (propertyDefinition);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public bool ContainsColumnName (string columnName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

    foreach (PropertyDefinition propertyDefinition in this)
    {
      if (propertyDefinition.ColumnName == columnName)
        return true;
    }

    return false;
  }

  public ClassDefinition ClassDefinition 
  {
    get { return _classDefinition; }
  }

  protected virtual void OnAdding (PropertyDefinitionAddingEventArgs args)
  {
    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of ClassDefinition when adding property definitions is not organized through events.
    if (_classDefinition != null)
      _classDefinition.PropertyDefinitions_Adding (this, args);

    if (Adding != null)
      Adding (this, args);
  }

  protected virtual void OnAdded (PropertyDefinitionAddedEventArgs args)
  {
    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of ClassDefinition when adding property definitions is not organized through events.
    if (_classDefinition != null)
      _classDefinition.PropertyDefinitions_Added (this, args);

    if (Added != null)
      Added (this, args);
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    return BaseContains (propertyDefinition.PropertyName, propertyDefinition);
  }

  public bool Contains (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return BaseContainsKey (propertyName);
  }

  public PropertyDefinition this [int index]  
  {
    get { return (PropertyDefinition) BaseGetObject (index); }
  }

  public PropertyDefinition this [string propertyName]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      return (PropertyDefinition) BaseGetObject (propertyName); 
    }
  }

  public int Add (PropertyDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    OnAdding (new PropertyDefinitionAddingEventArgs (value));
    int position = BaseAdd (value.PropertyName, value);
    OnAdded (new PropertyDefinitionAddedEventArgs (value));

    return position;
  }

  #endregion
}
}