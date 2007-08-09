using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  [Serializable]
  public class BaseBusinessObjectClass
  {
    private object _public;

    public BaseBusinessObjectClass ()
    {
    }

    public object Public
    {
      get { return _public; }
      set { _public = value; }
    }
  }
}