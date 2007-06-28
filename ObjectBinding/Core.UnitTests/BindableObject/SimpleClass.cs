using System;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [BindableObject]
  public class SimpleClass
  {
    private string _string;

    public SimpleClass ()
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