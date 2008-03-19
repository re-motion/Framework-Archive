using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingUnidirectionalRelation1))]
  [DBTable ("MixedDomains_TargetWithUnidirectionalMixin2")]
  [TestDomain]
  public class TargetClassWithUnidirectionalMixin2 : SimpleDomainObject<TargetClassWithUnidirectionalMixin2>
  {
    
  }
}