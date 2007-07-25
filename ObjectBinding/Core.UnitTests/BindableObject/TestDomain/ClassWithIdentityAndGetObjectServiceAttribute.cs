using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  [GetObjectServiceType (typeof (ICustomGetObjectService))]
  public class ClassWithIdentityAndGetObjectServiceAttribute
  {
    public ClassWithIdentityAndGetObjectServiceAttribute ()
    {
    }
  }
}