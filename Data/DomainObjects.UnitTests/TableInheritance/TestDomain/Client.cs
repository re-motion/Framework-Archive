using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Client")]
  [DBTable ("TableInheritance_Client")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class Client : DomainObject
  {
    public static Client NewObject ()
    {
      return NewObject<Client> ().With();
    }

    public new static Client GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Client> (id);
    }

    protected Client ()
    {
    }

    [DBBidirectionalRelation ("Client", SortExpression = "CreatedAt asc")]
    public abstract ObjectList<DomainBase> AssignedObjects { get; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}