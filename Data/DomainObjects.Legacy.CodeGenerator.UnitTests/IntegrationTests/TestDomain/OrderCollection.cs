using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
  public class OrderCollection : DomainObjectCollection
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public OrderCollection ()
      : base (typeof (Order))
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
