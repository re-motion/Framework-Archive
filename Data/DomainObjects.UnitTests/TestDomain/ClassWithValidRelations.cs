using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithValidRelations")]
  public class ClassWithValidRelations: TestDomainBase
  {
    // types

    // static members and constants

    public new static ClassWithValidRelations GetObject (ObjectID id)
    {
      return (ClassWithValidRelations) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithValidRelations()
    {
    }

    public ClassWithValidRelations (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    protected ClassWithValidRelations (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClassWithValidRelationsOptional", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyOptionalID")]
    public ClassWithGuidKey ClassWithGuidKeyOptional
    {
      get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKeyOptional"); }
      set { SetRelatedObject ("ClassWithGuidKeyOptional", value); }
    }

    [DBBidirectionalRelation ("ClassWithValidRelationsNonOptional", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyNonOptionalID")]
    [Mandatory]
    public ClassWithGuidKey ClassWithGuidKeyNonOptional
    {
      get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKeyNonOptional"); }
      set { SetRelatedObject ("ClassWithGuidKeyNonOptional", value); }
    }
  }
}