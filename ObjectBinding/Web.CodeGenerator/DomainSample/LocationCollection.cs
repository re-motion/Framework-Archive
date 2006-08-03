using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace DomainSample
{
public class LocationCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public LocationCollection () : base (typeof (Location))
  {
  }

  // methods and properties

  public new Location this[int index]
  {
    get { return (Location) base[index]; }
    set { base[index] = value; }
  }

  public new Location this[ObjectID id]
  {
    get { return (Location) base[id]; }
  }

}
}
