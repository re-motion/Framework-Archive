using System;
using Rubicon.Mixins;

namespace Rubicon.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithEnum
  {
    public static TypeWithEnum Create ()
    {
      return ObjectFactory.Create<TypeWithEnum> ().With ();
    }

    private TestEnum _enumValue;

    protected TypeWithEnum ()
    {
    }

    public TestEnum EnumValue
    {
      get { return _enumValue; }
      set { _enumValue = value; }
    }
  }

  public enum TestEnum
  {
    First,
    Second,
    Third
  }
}