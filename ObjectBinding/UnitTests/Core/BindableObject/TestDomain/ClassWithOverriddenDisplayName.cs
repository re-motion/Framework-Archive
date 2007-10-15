using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithOverriddenDisplayName
  {
    public ClassWithOverriddenDisplayName ()
    {
    }

    [OverrideMixinMember]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}