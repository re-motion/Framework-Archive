using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_BaseClassWithInvalidRelationClassIDColumns")]
  [DBTable ("TableInheritance_BaseClassWithInvalidRelationClassIDColumns")]
  [TableInheritanceTestDomain]
  public abstract class BaseClassWithInvalidRelationClassIDColumns : DomainObject
  {
    protected BaseClassWithInvalidRelationClassIDColumns ()
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