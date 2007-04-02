using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [NotAbstractAttribute]
  public abstract class ClassWithOneSideRelationProperties : DomainObject //TestDomainBase
  {
    protected ClassWithOneSideRelationProperties (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    [BidirectionalRelation ("NoAttribute")]
    public abstract ObjectList<ClassWithManySideRelationProperties> NoAttribute { get; set; }

    [AutomaticProperty]
    [BidirectionalRelation ("NotNullable")]
    [Mandatory]
    public abstract ObjectList<ClassWithManySideRelationProperties> NotNullable { get; set; }

    [AutomaticProperty]
    [DBBidirectionalRelation ("BidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BidirectionalOneToOne { get; set; }

    [AutomaticProperty]
    [DBBidirectionalRelation ("BidirectionalOneToMany")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BidirectionalOneToMany { get; }
  }
}