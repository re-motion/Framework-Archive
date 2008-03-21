using System;
using Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [DBTable]
  [Uses (typeof (MixinAddingBidirectionalRelationTwice))]
  public class TargetClass1ForMixinAddingBidirectionalRelationTwice : SimpleDomainObject<TargetClass1ForMixinAddingBidirectionalRelationTwice>
  {
  }
}