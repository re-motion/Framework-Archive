using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.MixinTestDomain
{
  [DBTable]
  [Instantiable]
  [Uses (typeof (DerivedMixinNotOnBase))]
  public class TargetClassC : DomainObject
  {
    
  }
}