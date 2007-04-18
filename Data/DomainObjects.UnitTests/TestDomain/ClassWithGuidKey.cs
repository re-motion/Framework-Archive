using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithGuidKey")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithGuidKey : TestDomainBase
  {
    public static ClassWithGuidKey NewObject ()
    {
      return DomainObject.NewObject<ClassWithGuidKey> ().With();
    }

    protected ClassWithGuidKey()
    {
    }

    protected ClassWithGuidKey (DataContainer dataContainer)
        : base (dataContainer)
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