using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithValidRelations")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithValidRelations : TestDomainBase
  {
    public static ClassWithValidRelations Create ()
    {
      return DomainObject.Create<ClassWithValidRelations> ();
    }

    protected ClassWithValidRelations (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
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