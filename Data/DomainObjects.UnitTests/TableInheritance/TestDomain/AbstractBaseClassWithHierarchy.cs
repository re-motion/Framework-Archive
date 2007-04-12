using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_AbstractBaseClassWithHierarchy")]
  [TestDomain]
  public abstract class AbstractBaseClassWithHierarchy : DomainObject
  {
    public new static AbstractBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return (AbstractBaseClassWithHierarchy) DomainObject.GetObject (id);
    }

    protected AbstractBaseClassWithHierarchy (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("ChildAbstractBaseClassesWithHierarchy")]
    public abstract AbstractBaseClassWithHierarchy ParentAbstractBaseClassWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentAbstractBaseClassWithHierarchy", SortExpression = "Name DESC")]
    public virtual ObjectList<AbstractBaseClassWithHierarchy> ChildAbstractBaseClassesWithHierarchy { get { return (ObjectList<AbstractBaseClassWithHierarchy>) GetRelatedObjects(); } }

    public abstract Client ClientFromAbstractBaseClass { get; set; }

    public abstract FileSystemItem FileSystemItemFromAbstractBaseClass { get; set; }
  }
}