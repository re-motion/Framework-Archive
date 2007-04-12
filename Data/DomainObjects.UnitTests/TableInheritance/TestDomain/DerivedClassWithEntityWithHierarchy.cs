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

    public static DerivedClassWithEntityWithHierarchy Create ()
    {
      return DomainObject.Create<DerivedClassWithEntityWithHierarchy> ();
    }

    protected DerivedClassWithEntityWithHierarchy (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityWithHierarchy")]
    public abstract DerivedClassWithEntityWithHierarchy ParentDerivedClassWithEntityWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityWithHierarchy", SortExpression = "Name ASC")]
    public virtual ObjectList<DerivedClassWithEntityWithHierarchy> ChildDerivedClassesWithEntityWithHierarchy { get { return (ObjectList<DerivedClassWithEntityWithHierarchy>) GetRelatedObjects(); } }

    public abstract Client ClientFromDerivedClassWithEntity { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntity { get; set; }
  }
}