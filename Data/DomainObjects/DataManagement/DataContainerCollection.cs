using System;
using System.Collections;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class DataContainerCollection : CollectionBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DataContainerCollection ()
  {
  }

  // standard constructor for collections
  public DataContainerCollection (DataContainerCollection collection, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (DataContainer dataContainer in collection)
    {
      Add (dataContainer);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public DataContainerCollection GetByState (StateType state)
  {
    ArgumentUtility.CheckValidEnumValue (state, "state");

    DataContainerCollection collection = new DataContainerCollection ();

    foreach (DataContainer dataContainer in this)
    {
      if (dataContainer.State == state)
        collection.Add (dataContainer);
    }

    return collection;
  }

  public DataContainerCollection GetDifference (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DataContainerCollection difference = new DataContainerCollection ();

    foreach (DataContainer dataContainer in this)
    {
      if (!dataContainers.Contains (dataContainer.ID))
        difference.Add (dataContainer);
    }

    return difference;
  }

  public DataContainerCollection Merge (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DataContainerCollection mergedCollection = new DataContainerCollection ();

    foreach (DataContainer dataContainer in this)
    {
      if (dataContainers.Contains (dataContainer.ID))
        mergedCollection.Add (dataContainers[dataContainer.ID]);
      else
        mergedCollection.Add (dataContainer);
    }

    return mergedCollection;
  }

  #region Standard implementation for collections

  public bool Contains (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    return Contains (dataContainer.ID);
  }

  public bool Contains (ObjectID id)
  {
    return this.ContainsKey (id);
  }

  public DataContainer this[int index]
  {
    get {return (DataContainer) GetObject (index); }
  }

  public DataContainer this[ObjectID id]  
  {
    get { return (DataContainer) GetObject (id); }
  }

  public void Add (DataContainer value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    base.Add (value.ID, value);
  }

  public void Remove (int index)
  {
    Remove (this[index]);
  }

  public void Remove (ObjectID id)
  {
    Remove (this[id]);
  }

  public void Remove (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    base.Remove (dataContainer.ID);
  }

  public void Clear ()
  {
    base.ClearCollection ();
  }

  #endregion
}
}
