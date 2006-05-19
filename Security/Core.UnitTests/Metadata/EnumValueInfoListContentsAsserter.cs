using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  [CLSCompliant (false)] // AbstractAsserter is not CLS complient
  public class EnumValueInfoListContentsAsserter : AbstractAsserter
  {
    // types

    // static members

    // member fields

    private string _expectedName;
    private IList<EnumValueInfo> _list;

    // construction and disposing

    public EnumValueInfoListContentsAsserter (string expectedName, IList<EnumValueInfo> list, string message, params object[] args)
      : base (message, args)
    {
      _expectedName = expectedName;
      _list = list;
    }

    // methods and properties

    public override bool Test ()
    {
      if (_list != null)
      {
        foreach (EnumValueInfo value in _list)
        {
          if (string.Equals (value.Name, _expectedName, StringComparison.Ordinal))
            return true;
        }
      }

      return false;
    }

    public override string Message
    {
      get
      {
        FailureMessage.DisplayExpectedValue (_expectedName);
        FailureMessage.DisplayListElements ("\t but was: ", ExtractNames (_list), 0, 10);

        return base.FailureMessage.ToString ();
      }
    }

    private List<string> ExtractNames (IList<EnumValueInfo> list)
    {
      if (_list == null)
        return null;

      List<string> actualNames = new List<string> ();
      foreach (EnumValueInfo value in list)
        actualNames.Add (value.Name);

      return actualNames;
    }
  }
}