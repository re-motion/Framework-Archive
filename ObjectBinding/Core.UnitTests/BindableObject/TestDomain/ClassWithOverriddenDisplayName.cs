using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithOverriddenDisplayName
  {
    public ClassWithOverriddenDisplayName ()
    {
    }

    [Override]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}