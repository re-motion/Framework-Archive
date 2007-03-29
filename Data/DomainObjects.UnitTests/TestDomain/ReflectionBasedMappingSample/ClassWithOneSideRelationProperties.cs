using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithOneSideRelationProperties: DomainObject //TestDomainBase
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
    [RdbmsBidirectionalRelation ("BidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BidirectionalOneToOne { get; set; }

    [AutomaticProperty]
    [RdbmsBidirectionalRelation ("BidirectionalOneToMany")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BidirectionalOneToMany { get; }
  }
}