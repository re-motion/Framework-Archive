using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithGuidKey")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithGuidKey : TestDomainBase
  {
    public static ClassWithGuidKey Create ()
    {
      return DomainObject.Create<ClassWithGuidKey> ();
    }


    protected ClassWithGuidKey (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("ClassWithGuidKeyOptional")]
    public abstract ClassWithValidRelations ClassWithValidRelationsOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKeyNonOptional")]
    [Mandatory]
    public abstract ClassWithValidRelations ClassWithValidRelationsNonOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public abstract ClassWithInvalidRelation ClassWithInvalidRelation { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public abstract ClassWithRelatedClassIDColumnAndNoInheritance ClassWithRelatedClassIDColumnAndNoInheritance { get; set; }
  }
}