using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObjectWithIdentity]
  [Serializable]
  public class ClassWithIdentity
  {
    private string _string;
    private readonly string _uniqueIdentifier;

    public ClassWithIdentity (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    public ClassWithIdentity ()
      : this (Guid.NewGuid().ToString())
    {
    }

    [OverrideMixinMember]
    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}