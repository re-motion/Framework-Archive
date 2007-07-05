using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  [BindableObjectSearchServiceType (typeof (ISearchServiceOnType))]
  public class ClassWithSearchServiceTypeAttribute
  {
    public ClassWithSearchServiceTypeAttribute ()
    {
    }
  }
}