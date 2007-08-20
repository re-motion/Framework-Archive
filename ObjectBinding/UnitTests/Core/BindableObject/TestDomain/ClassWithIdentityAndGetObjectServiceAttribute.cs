using System;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [BindableObjectWithIdentity]
  [GetObjectServiceType (typeof (ICustomGetObjectService))]
  public class ClassWithIdentityAndGetObjectServiceAttribute
  {
    private readonly string _uniqueIdentifier;

    public ClassWithIdentityAndGetObjectServiceAttribute (string uniqueIdentifier)
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