using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.TestDomain
{
public class OrderCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public OrderCollection () : base (typeof (Order))
  {
  }

  // methods and properties

  public new Order this[int index]
  {
    get { return (Order) base[index]; }
    set { base[index] = value; }
  }

  public new Order this[ObjectID id]
  {
    get { return (Order) base[id]; }
  }

}
}
