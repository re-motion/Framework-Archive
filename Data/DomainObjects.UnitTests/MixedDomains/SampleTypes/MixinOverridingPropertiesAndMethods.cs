using System;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinOverridingPropertiesAndMethods
      : Mixin<MixinOverridingPropertiesAndMethods.IBaseRequirements, MixinOverridingPropertiesAndMethods.IBaseRequirements>
  {

    [Override]
    public virtual string Property
    {
      get { return Base.Property + "-MixinGetter"; }
      set { Base.Property = value + "-MixinSetter"; }
    }

    [Override]
    public virtual string GetSomething ()
    {
      return Base.GetSomething () + "-MixinMethod";
    }

    public interface IBaseRequirements
    {
      string Property { get; set; }
      string GetSomething ();
    }
  }
}