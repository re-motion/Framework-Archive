using System;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [DBTable]
  public class DOWithVirtualPropertiesAndMethods : DomainObject
  {
    public virtual string Property
    {
      get { return CurrentProperty.GetValue<string>(); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual string GetSomething ()
    {
      return "Something";
    }
  }
}