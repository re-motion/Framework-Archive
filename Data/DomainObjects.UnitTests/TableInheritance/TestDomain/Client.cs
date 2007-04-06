using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Client")]
  [DBTable (Name = "TableInheritence_")]
  [NotAbstract]
  [TestDomain]
  public abstract class Client : DomainObject
  {
    public new static Client GetObject (ObjectID id)
    {
      return (Client) DomainObject.GetObject (id);
    }

    public Client (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    protected Client (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("Client", SortExpression = "CreatedAt asc")]
    public abstract ObjectList<DomainBase> AssignedObjects { get; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}