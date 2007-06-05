using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class NonSecurableObject : DomainObject
  {
    public static NonSecurableObject NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<NonSecurableObject>().With();
      }
    }

    protected NonSecurableObject ()
    {
    }

    public DataContainer GetDataContainer ()
    {
      return DataContainer;
    }

    public abstract string StringProperty { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract NonSecurableObject Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<NonSecurableObject> Children { get; }
  }
}