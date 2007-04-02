using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutRelatedClassIDColumn")]
  public class ClassWithoutRelatedClassIDColumn: TestDomainBase
  {
    // types

    // static members and constants

    public new static ClassWithoutRelatedClassIDColumn GetObject (ObjectID id)
    {
      return (ClassWithoutRelatedClassIDColumn) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithoutRelatedClassIDColumn()
    {
    }

    public ClassWithoutRelatedClassIDColumn (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    protected ClassWithoutRelatedClassIDColumn (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumn", ContainsForeignKey = true)]
    public Distributor Distributor
    {
      get { return (Distributor) GetRelatedObject ("Distributor"); }
      set { SetRelatedObject ("Distributor", value); }
    }
  }
}