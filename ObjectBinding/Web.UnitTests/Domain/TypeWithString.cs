using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class TypeWithString
  {
    public static TypeWithString Create ()
    {
      return ObjectFactory.Create<TypeWithString> ().With ();
    }

    public static TypeWithString Create (string firstValue, string secondValue)
    {
      return ObjectFactory.Create<TypeWithString> ().With (firstValue, secondValue);
    }

    private string _stringValue;
    private string[] _stringArray;
    private string _firstValue;
    private string _secondValue;

    protected TypeWithString ()
    {
    }

    protected TypeWithString (string firstValue, string secondValue)
    {
      _firstValue = firstValue;
      _secondValue = secondValue;
    }

    public string StringValue
    {
      get { return _stringValue; }
      set { _stringValue = value; }
    }

    public string[] StringArray
    {
      get { return _stringArray; }
      set { _stringArray = value; }
    }

    public string FirstValue
    {
      get { return _firstValue; }
      set { _firstValue = value; }
    }

    public string SecondValue
    {
      get { return _secondValue; }
      set { _secondValue = value; }
    }
  }
}