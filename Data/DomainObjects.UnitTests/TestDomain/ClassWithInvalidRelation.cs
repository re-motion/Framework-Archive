using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithInvalidRelation")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithInvalidRelation : TestDomainBase
  {
    protected ClassWithInvalidRelation ()
    {
    }

    protected ClassWithInvalidRelation (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ClassWithInvalidRelation", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyID")]
    public abstract ClassWithGuidKey ClassWithGuidKey { get; set; }
  }
}