using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithEntityWithHierarchy")]
  [DBTable (Name = "TableInheritance_DerivedClassWithEntityWithHierarchy")]
  [NotAbstract]
  public abstract class DerivedClassWithEntityWithHierarchy: AbstractBaseClassWithHierarchy
  {
    public new static DerivedClassWithEntityWithHierarchy GetObject (ObjectID id)
    {
      return (DerivedClassWithEntityWithHierarchy) DomainObject.GetObject (id);
    }

    protected DerivedClassWithEntityWithHierarchy (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    protected DerivedClassWithEntityWithHierarchy (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityWithHierarchy")]
    public abstract DerivedClassWithEntityWithHierarchy ParentDerivedClassWithEntityWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityWithHierarchy", SortExpression = "Name ASC")]
    public abstract ObjectList<DerivedClassWithEntityWithHierarchy> ChildDerivedClassesWithEntityWithHierarchy { get; }

    public abstract Client ClientFromDerivedClassWithEntity { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntity { get; set; }
  }
}