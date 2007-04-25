using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class ClassWithInvalidUnidirectionalRelation: DomainObject
  {
    protected ClassWithInvalidUnidirectionalRelation ()
    {
    }

    protected ClassWithInvalidUnidirectionalRelation (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract ObjectList<ClassWithInvalidUnidirectionalRelation> LeftSide { get; }
  }
}