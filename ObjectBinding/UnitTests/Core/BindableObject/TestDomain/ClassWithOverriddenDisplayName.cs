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

    [OverrideMixin]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}