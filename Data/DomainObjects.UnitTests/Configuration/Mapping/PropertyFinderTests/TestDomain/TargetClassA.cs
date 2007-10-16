using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests.TestDomain
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (MixinA))]
  [Uses (typeof (MixinC))]
  [Uses (typeof (MixinD))]
  [Uses (typeof (NonDomainObjectMixin))]
  public abstract class TargetClassA : TargetClassBase
  {
    public abstract int P1 { get; }
    public abstract int P2 { get; set; }
  }
}
