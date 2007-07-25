using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObjectWithIdentity]
  public class ClassWithIdentity
  {
    private readonly string _uniqueIdentifier;

    public ClassWithIdentity (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    [Override]
    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }
  }
}