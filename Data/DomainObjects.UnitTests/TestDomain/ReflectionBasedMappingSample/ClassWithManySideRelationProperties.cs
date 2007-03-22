using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithManySideRelationProperties: TestDomainBase
  {
    protected ClassWithManySideRelationProperties (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties NoAttribute { get; set; }

    [AutomaticProperty]
    [Mandatory]
    public abstract ClassWithOneSideRelationProperties NotNullable { get; set; }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToMany { get; set; }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties BidirectionalOneToOne { get; set; }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties BidirectionalOneToMany { get; set; }
  }
}