using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class ClassWithInvalidUnidirectionalRelation: DomainObject
  {
    protected ClassWithInvalidUnidirectionalRelation ()
    {
    }

    public abstract ObjectList<ClassWithInvalidUnidirectionalRelation> LeftSide { get; }
  }
}