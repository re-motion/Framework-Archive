using System;
using System.Collections;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointCollection : CollectionBase
{
  // types

  // static members and constants


  // member fields

  // construction and disposing

  public RelationEndPointCollection ()
  {
  }

  public RelationEndPointCollection (RelationEndPointCollection collection, bool isCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (RelationEndPoint endPoint in collection)
    {
      Add (endPoint);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
  }

  // methods and properties

  public bool BeginDelete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    foreach (RelationEndPoint endPoint in this)
    {
      IRelationEndPointDefinition endPointDefinition = endPoint.OppositeEndPointDefinition;
      RelationEndPoint oldEndPoint = ClientTransaction.Current.GetRelationEndPoint (domainObject, endPointDefinition);
      RelationEndPoint newEndPoint = RelationEndPoint.CreateNullRelationEndPoint (endPointDefinition);

      if (!endPoint.BeginRelationChange (oldEndPoint, newEndPoint))
        return false;
    }
    return true;
  }

  public void EndDelete ()
  {
    foreach (RelationEndPoint endPoint in this)
      endPoint.EndRelationChange ();    
  }

  #region Standard implementation for collections

  public bool Contains (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    return Contains (endPoint.ID);
  }

  public bool Contains (RelationEndPointID id)
  {
    return this.ContainsKey (id);
  }

  public RelationEndPoint this[int index]
  {
    get {return (RelationEndPoint) GetObject (index); }
  }

  public RelationEndPoint this[RelationEndPointID id]
  {
    get { return (RelationEndPoint) GetObject (id); }
  }

  public virtual void Add (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    base.Add (endPoint.ID, endPoint);
  }

  public void Remove (int index)
  {
    Remove (this[index]);
  }

  public void Remove (RelationEndPointID id)
  {
    Remove (this[id]);
  }

  public void Remove (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    base.Remove (endPoint.ID);
  }

  public void Clear ()
  {
    base.ClearCollection ();
  }

  #endregion
}
}
