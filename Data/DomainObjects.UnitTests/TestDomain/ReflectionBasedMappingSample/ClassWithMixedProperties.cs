using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithMixedProperties: TestDomainBase
  {
    protected ClassWithMixedProperties (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [StorageClassNone]
    public object Unmanaged
    {
      get { return null; }
      set { }
    }

    [AutomaticProperty]
    public abstract int Int32 { get; set; }

    [AutomaticProperty]
    public abstract string String { get; set; }

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }
  }
}