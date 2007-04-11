using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [FactoryInstantiated]
  [NotAbstract]
  [DBTable]
  [TestDomain]
  public abstract class OrderItemWithNewPropertyAccess : DomainObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public OrderItemWithNewPropertyAccess (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    // methods and properties

    public int Position
    {
      get { return (int) DataContainer["Position"]; }
      set { DataContainer["Position"] = value; }
    }

    public string Product
    {
      get { return (string) DataContainer["Product"]; }
      set { DataContainer["Product"] = value; }
    }

    [AutomaticProperty]
    public abstract OrderWithNewPropertyAccess Order { get; set; }
  }
}
