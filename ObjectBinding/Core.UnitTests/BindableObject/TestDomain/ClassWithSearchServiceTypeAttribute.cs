using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  [SearchAvailableObjectsServiceType (typeof (ISearchServiceOnType))]
  public class ClassWithSearchServiceTypeAttribute
  {
    public ClassWithSearchServiceTypeAttribute ()
    {
    }
  }
}