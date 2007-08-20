using System;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
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