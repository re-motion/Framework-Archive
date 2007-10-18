using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  [Uses (typeof (MixinBase))]
  public abstract class TargetClassBase : DomainObject
  {
    public abstract int P0 { get; }
  }
}
