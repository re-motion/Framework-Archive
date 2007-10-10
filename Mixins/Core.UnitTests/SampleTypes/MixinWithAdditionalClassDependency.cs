using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IMixinWithAdditionalClassDependency
  {
    string GetString ();
  }

  [Extends (typeof (TargetClassWithAdditionalDependencies), AdditionalDependencies = new Type[] { typeof ( MixinWithNoAdditionalDependency ) })]
  public class MixinWithAdditionalClassDependency : Mixin<object, ITargetClassWithAdditionalDependencies>, IMixinWithAdditionalClassDependency 
  {
    [Override]
    public string GetString ()
    {
      return "MixinWithAdditionalClassDependency-" + Base.GetString ();
    }
  }
}