using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutRelatedClassIDColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutRelatedClassIDColumn : TestDomainBase
  {
        public static ClassWithoutRelatedClassIDColumn NewObject ()
    {
      return DomainObject.NewObject<ClassWithoutRelatedClassIDColumn> ().With();
    }

    protected ClassWithoutRelatedClassIDColumn()
    {
    }

    protected ClassWithoutRelatedClassIDColumn (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  
    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumn", ContainsForeignKey = true)]
    public abstract Distributor Distributor { get; set; }
  }
}