using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_BaseClassWithInvalidRelationClassIDColumns")]
  [DBTable]
  [TestDomain]
  public abstract class BaseClassWithInvalidRelationClassIDColumns : DomainObject
  {
    protected BaseClassWithInvalidRelationClassIDColumns (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    protected BaseClassWithInvalidRelationClassIDColumns (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract Client Client { get; set; }

    public abstract DomainBase DomainBase { get; set; }

    public abstract DomainBase DomainBaseWithInvalidClassIDValue { get; set; }

    public abstract DomainBase DomainBaseWithInvalidClassIDNullValue { get; set; }
  }
}