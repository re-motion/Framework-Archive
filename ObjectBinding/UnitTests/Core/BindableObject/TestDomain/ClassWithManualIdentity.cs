using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [Serializable]
  public class ClassWithManualIdentity : ManualBusinessObject, IBusinessObjectWithIdentity
  {
    private readonly string _uniqueIdentifier;

    public ClassWithManualIdentity (string uniqueIdentifier)
    {
      _uniqueIdentifier = uniqueIdentifier;
    }

    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }
  }
}