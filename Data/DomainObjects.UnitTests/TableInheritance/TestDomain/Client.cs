using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Client")]
  [DBTable (Name = "TableInheritance_Client")]
  [NotAbstract]
  [TestDomain]
  public abstract class Client : DomainObject
  {
    public new static Client GetObject (ObjectID id)
    {
      return (Client) DomainObject.GetObject (id);
    }

    public static Client NewObject ()
    {
      return NewObject<Client> ().With();
    }

    protected Client ()
    {
    }

    protected Client (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("Client", SortExpression = "CreatedAt asc")]
    public abstract ObjectList<DomainBase> AssignedObjects { get; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}