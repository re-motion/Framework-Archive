using System;
using Rubicon.Globalization;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [BindableObject]
  [MultiLingualResources ("Rubicon.ObjectBinding.UnitTests.Globalization.ClassWithResources")]
  public class ClassWithResources
  {
    private string _value1;
    private string _valueWithoutResource;

    public ClassWithResources ()
    {
    }
    
    public string Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }

    public string ValueWithoutResource
    {
      get { return _valueWithoutResource; }
      set { _valueWithoutResource = value; }
    }
  }
}