using System;
using System.Collections;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class StorageProviderCollection : CollectionBase, IDisposable
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderCollection ()
  {
  }

  // standard constructor for collections
  public StorageProviderCollection (StorageProviderCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (StorageProvider provider in collection)
    {
      Add (provider);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  #region IDisposable Members

  public virtual void Dispose ()
  {
    for (int i = Count - 1; i>= 0; i--)
      this[i].Dispose ();      

    ClearCollection ();
    GC.SuppressFinalize (this);
  }

  #endregion

  // methods and properties

  #region Standard implementation for "add-only" collections

  public bool Contains (StorageProvider provider)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);

    return Contains (provider.ID);
  }

  public bool Contains (string storageProviderID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    return base.ContainsKey (storageProviderID);
  }

  public StorageProvider this [int index]  
  {
    get { return (StorageProvider) GetObject (index); }
  }

  public StorageProvider this [string storageProviderID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
      return (StorageProvider) GetObject (storageProviderID); 
    }
  }

  public void Add (StorageProvider value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    base.Add (value.ID, value);
  }

  #endregion
}
}