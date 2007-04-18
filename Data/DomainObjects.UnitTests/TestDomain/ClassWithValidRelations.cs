using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithValidRelations")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithValidRelations : TestDomainBase
  {
    public static ClassWithValidRelations NewObject ()
    {
      return DomainObject.NewObject<ClassWithValidRelations> ().With();
    }

    protected ClassWithValidRelations()
    {
    }

    protected ClassWithValidRelations (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ClassWithValidRelationsOptional", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyOptionalID")]
    public abstract ClassWithGuidKey ClassWithGuidKeyOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithValidRelationsNonOptional", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyNonOptionalID")]
    [Mandatory]
    public abstract ClassWithGuidKey ClassWithGuidKeyNonOptional { get; set; }
  }
}