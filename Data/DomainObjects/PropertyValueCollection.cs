using System;
using System.Collections;
using System.Collections.Specialized;

namespace Rubicon.Data.DomainObjects
{
public class PropertyValueCollection : CollectionBase
{
  // types

  // static members and constants

  // member fields

  public event PropertyChangingEventHandler PropertyChanging;
  public event PropertyChangedEventHandler PropertyChanged;

  // construction and disposing

  public PropertyValueCollection ()
  {
  }

  // standard constructor for collections
  public PropertyValueCollection (PropertyValueCollection collection, bool isCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (PropertyValue propertyValue in collection)
    {
      Add (propertyValue);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
  }

  // methods and properties

  protected virtual void OnPropertyChanging (PropertyChangingEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  protected virtual void OnPropertyChanged (PropertyChangedEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }

  private void PropertyValue_Changing (object sender, ValueChangingEventArgs e)
  {
    PropertyChangingEventArgs eventArgs = new PropertyChangingEventArgs (
        (PropertyValue) sender, e.OldValue, e.NewValue);

    OnPropertyChanging (eventArgs);

    if (eventArgs.Cancel)
      e.Cancel = true;
  }

  private void PropertyValue_Changed (object sender, EventArgs e)
  {
    PropertyChangedEventArgs eventArgs = new PropertyChangedEventArgs ((PropertyValue) sender);
    OnPropertyChanged (eventArgs);
  }

  private ArgumentException CreateArgumentException (string message, string parameterName, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), parameterName);
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    return Contains (propertyValue.Name);
  }

  public bool Contains (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return base.ContainsKey (propertyName);
  }

  public PropertyValue this [int index]  
  {
    get { return (PropertyValue) GetObject (index); }
  }

  public PropertyValue this [string propertyName]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      if (!ContainsKey (propertyName))
      {
        throw CreateArgumentException ("Property '{0}' does not exist.", "propertyName", propertyName);
      }
  
      return (PropertyValue) GetObject (propertyName); 
    }
  }

  public void Add (PropertyValue value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    if (Contains (value.Name))
      throw CreateArgumentException ("Property '{0}' already exists in collection.", "value", value.Name);

    value.Changing += new ValueChangingEventHandler (PropertyValue_Changing);
    value.Changed += new EventHandler (PropertyValue_Changed);
    base.Add (value.Name, value);
  }

  #endregion
}
}
