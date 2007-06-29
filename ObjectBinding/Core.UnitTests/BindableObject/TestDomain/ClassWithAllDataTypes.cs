using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    private bool _boolean;
    private TestEnum _enum;
    private string _string;

    public ClassWithAllDataTypes ()
    {
    }

    public bool Boolean
    {
      get { return _boolean; }
      set { _boolean = value; }
    }

    public TestEnum Enum
    {
      get { return _enum; }
      set { _enum = value; }
    }

    public string String
    {
      get { return _string; }
      set { _string = value; }
    }
  }
}