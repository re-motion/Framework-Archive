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

    public static Client Create ()
    {
      return DomainObject.Create<Client> ();
    }

    public Client (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("Client", SortExpression = "CreatedAt asc")]
    public virtual ObjectList<DomainBase> AssignedObjects { get { return (ObjectList<DomainBase>) GetRelatedObjects(); } }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}