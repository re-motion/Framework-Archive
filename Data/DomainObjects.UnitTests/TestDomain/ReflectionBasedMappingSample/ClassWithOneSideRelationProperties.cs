using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithOneSideRelationProperties: TestDomainBase
  {
    protected ClassWithOneSideRelationProperties (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract ClassWithManySideRelationProperties BidirectionalOneToOne { get; set; }

    [AutomaticProperty]
    public abstract ObjectList<ClassWithManySideRelationProperties> BidirectionalOneToMany { get; }
  }
}