using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class TypeWithAllDataTypes
  {
    public static TypeWithAllDataTypes Create ()
    {
      return ObjectFactory.Create<TypeWithAllDataTypes>().With();
    }

    public static TypeWithAllDataTypes Create (string stringValue, int int32Value)
    {
      return ObjectFactory.Create<TypeWithAllDataTypes> ().With (stringValue, int32Value);
    }

    private string _stringValue;
    private int _int32Value;

    protected TypeWithAllDataTypes ()
    {
    }

    protected TypeWithAllDataTypes (string stringValue, int int32Value)
    {
      _stringValue = stringValue;
      _int32Value = int32Value;
    }

    public string StringValue
    {
      get { return _stringValue; }
      set { _stringValue = value; }
    }

    public int Int32Value
    {
      get { return _int32Value; }
      set { _int32Value = value; }
    }
  }
}