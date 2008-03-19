using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingUnidirectionalRelation1))]
  [Uses (typeof (MixinAddingUnidirectionalRelation2))]
  [DBTable ("MixedDomains_TargetWithTwoUnidirectionalMixins")]
  [TestDomain]
  public class TargetClassWithTwoUnidirectionalMixins : SimpleDomainObject<TargetClassWithTwoUnidirectionalMixins>
  {
  }
}