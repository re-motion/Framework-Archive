using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [Serializable]
  public class DOHidingVirtualProperties : DOWithVirtualProperties
  {
    [DBColumn ("NewPropertyWithGetterAndSetter")]
    public new virtual int PropertyWithGetterAndSetter
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}