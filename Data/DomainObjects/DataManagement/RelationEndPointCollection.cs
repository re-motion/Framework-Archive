using System;
using System.Collections;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointList : IEnumerable
{
  // types

  // static members and constants

  // member fields

  private ArrayList _relationEndPoints;

  // construction and disposing

  public RelationEndPointList ()
  {
    _relationEndPoints = new ArrayList ();
  }

  // methods and properties

  public RelationEndPoint this [int index]
  {
    get { return (RelationEndPoint) _relationEndPoints[index]; }
  }

  public bool Contains (RelationEndPoint relationEndPoint)
  {
    return _relationEndPoints.Contains (relationEndPoint);
  }

  public int Count
  {
    get { return _relationEndPoints.Count; }
  }

  public void Add (RelationEndPoint relationEndPoint)
  {
    _relationEndPoints.Add (relationEndPoint);
  }

  public void Remove (RelationEndPoint relationEndPoint)
  {
    _relationEndPoints.Remove (relationEndPoint);
  }

  public void RemoveAt (int index)
  {
    _relationEndPoints.RemoveAt (index); 
  }

  public void Clear ()
  {
    _relationEndPoints.Clear ();
  }

  public int IndexOf (RelationEndPoint relationEndPoint)
  {
    return _relationEndPoints.IndexOf (relationEndPoint);
  }

  #region IEnumerable Members

  public virtual IEnumerator GetEnumerator ()
  {
    return _relationEndPoints.GetEnumerator ();
  }

  #endregion
}
}
