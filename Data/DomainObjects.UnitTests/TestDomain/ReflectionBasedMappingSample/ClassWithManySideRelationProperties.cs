using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithManySideRelationProperties : DomainObject
  {
    protected ClassWithManySideRelationProperties (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [BidirectionalRelation ("NoAttribute")]
    public abstract ClassWithOneSideRelationProperties NoAttribute { get; set; }

    [Mandatory]
    [BidirectionalRelation ("NotNullable")]
    public abstract ClassWithOneSideRelationProperties NotNullable { get; set; }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToMany { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToOne", ContainsForeignKey = true)]
    public abstract ClassWithOneSideRelationProperties BidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToMany")]
    public abstract ClassWithOneSideRelationProperties BidirectionalOneToMany { get; set; }
  }
}