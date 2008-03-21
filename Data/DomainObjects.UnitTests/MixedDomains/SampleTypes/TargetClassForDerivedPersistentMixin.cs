using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [DBTable]
  [Uses (typeof (DerivedMixinAddingPersistentProperties))]
  public class TargetClassForDerivedPersistentMixin : SimpleDomainObject<TargetClassForDerivedPersistentMixin>
  {
  }
}