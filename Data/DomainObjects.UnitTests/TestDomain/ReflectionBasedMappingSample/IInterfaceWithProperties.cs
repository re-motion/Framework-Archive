using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public interface IInterfaceWithProperties
  {
    string ImplicitProperty { get; set; }
    string ExplicitProperty { get; set; }
    string ExplicitManagedProperty { get; set; }
  }
}