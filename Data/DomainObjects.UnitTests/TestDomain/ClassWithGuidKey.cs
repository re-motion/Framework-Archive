using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithGuidKey")]
  public class ClassWithGuidKey: TestDomainBase
  {
    // types

    // static members and constants

    public new static ClassWithGuidKey GetObject (ObjectID id)
    {
      return (ClassWithGuidKey) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithGuidKey()
    {
    }

    public ClassWithGuidKey (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    protected ClassWithGuidKey (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClassWithGuidKeyOptional")]
    public ClassWithValidRelations ClassWithValidRelationsOptional
    {
      get { return (ClassWithValidRelations) GetRelatedObject ("ClassWithValidRelationsOptional"); }
      set { SetRelatedObject ("ClassWithValidRelationsOptional", value); }
    }

    [DBBidirectionalRelation ("ClassWithGuidKeyNonOptional")]
    [Mandatory]
    public ClassWithValidRelations ClassWithValidRelationsNonOptional
    {
      get { return (ClassWithValidRelations) GetRelatedObject ("ClassWithValidRelationsNonOptional"); }
      set { SetRelatedObject ("ClassWithValidRelationsNonOptional", value); }
    }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public ClassWithInvalidRelation ClassWithInvalidRelation
    {
      get { return (ClassWithInvalidRelation) GetRelatedObject ("ClassWithInvalidRelation"); }
      set { SetRelatedObject ("ClassWithInvalidRelation", value); }
    }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public ClassWithRelatedClassIDColumnAndNoInheritance ClassWithRelatedClassIDColumnAndNoInheritance
    {
      get { return (ClassWithRelatedClassIDColumnAndNoInheritance) GetRelatedObject ("ClassWithRelatedClassIDColumnAndNoInheritance"); }
      set { SetRelatedObject ("ClassWithRelatedClassIDColumnAndNoInheritance", value); }
    }
  }
}