using System;
using System.Collections;
using System.Collections.Specialized;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class OrderCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  // standard constructor for collections
  public OrderCollection () : base (typeof (Order))
  {
  }

  // standard constructor for collections
  public OrderCollection (OrderCollection collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
  {
  }

  // methods and properties

  public new void SetIsReadOnly (bool isReadOnly)
  {
    base.SetIsReadOnly (isReadOnly);
  }

  #region standard implementation for collections

  public new Order this [int index]  
  {
    get { return (Order) base[index]; }
    set { base[index] = value; }
  }

  public new Order this [ObjectID id]  
  {
    get { return (Order) base[id]; }
  }

  #endregion
}
}
