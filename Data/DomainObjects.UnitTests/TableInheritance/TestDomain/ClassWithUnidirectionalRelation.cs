using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_ClassWithUnidirectionalRelation")]
  [DBTable ("TableInheritance_TableWithUnidirectionalRelation")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class ClassWithUnidirectionalRelation : DomainObject
  {
    public static ClassWithUnidirectionalRelation NewObject ()
    {
      return NewObject<ClassWithUnidirectionalRelation> ().With();
    }

    public new static ClassWithUnidirectionalRelation GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClassWithUnidirectionalRelation> (id);
    }

    protected ClassWithUnidirectionalRelation ()
    {
    }

    public abstract DomainBase DomainBase { get; set; }
  }
}