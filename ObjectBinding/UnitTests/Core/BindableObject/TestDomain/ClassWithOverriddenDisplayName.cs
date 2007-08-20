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

    [Override]
    public string DisplayName
    {
      get { return "TheDisplayName"; }
    }
  }
}