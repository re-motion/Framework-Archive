using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    private string _string;

    public ClassWithAllDataTypes ()
    {
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}