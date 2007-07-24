using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  [Serializable]
  public class SimpleBusinessObjectClass
  {
    private string _string;

    public SimpleBusinessObjectClass ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }

    private string PrivateString
    {
      get { return null; }
      set { }
    }
  }
}