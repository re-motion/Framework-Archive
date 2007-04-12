using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithEntityFromBaseClassWithHierarchy")]
  [NotAbstract]
  public abstract class DerivedClassWithEntityFromBaseClassWithHierarchy: DerivedClassWithEntityWithHierarchy
  {
    public new static DerivedClassWithEntityFromBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return (DerivedClassWithEntityFromBaseClassWithHierarchy) DomainObject.GetObject (id);
    }

    public static DerivedClassWithEntityFromBaseClassWithHierarchy Create ()
    {
      return DomainObject.Create<DerivedClassWithEntityFromBaseClassWithHierarchy> ();
    }

    protected DerivedClassWithEntityFromBaseClassWithHierarchy (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityFromBaseClassWithHierarchy")]
    public abstract DerivedClassWithEntityFromBaseClassWithHierarchy ParentDerivedClassWithEntityFromBaseClassWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityFromBaseClassWithHierarchy", SortExpression = "Name ASC")]
    public virtual ObjectList<DerivedClassWithEntityFromBaseClassWithHierarchy> ChildDerivedClassesWithEntityFromBaseClassWithHierarchy { get { return (ObjectList<DerivedClassWithEntityFromBaseClassWithHierarchy>) GetRelatedObjects(); } }

    public abstract Client ClientFromDerivedClassWithEntityFromBaseClass { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntityFromBaseClass { get; set; }
  }
}