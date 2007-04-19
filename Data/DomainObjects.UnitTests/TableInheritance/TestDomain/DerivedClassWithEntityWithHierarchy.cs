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

    public static DerivedClassWithEntityWithHierarchy NewObject ()
    {
      return DomainObject.NewObject<DerivedClassWithEntityWithHierarchy> ().With ();
    }

    protected DerivedClassWithEntityWithHierarchy ()
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