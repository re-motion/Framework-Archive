using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithOneSideRelationProperties : DomainObject
  {
    protected ClassWithOneSideRelationProperties ()
    {
    }

    protected ClassWithOneSideRelationProperties (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [BidirectionalRelation ("NoAttribute")]
    public abstract ObjectList<ClassWithManySideRelationProperties> NoAttribute { get; set; }

    [BidirectionalRelation ("NotNullable")]
    [Mandatory]
    public abstract ObjectList<ClassWithManySideRelationProperties> NotNullable { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BidirectionalOneToMany", SortExpression = "The Sort Expression")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BidirectionalOneToMany { get; }
  }
}