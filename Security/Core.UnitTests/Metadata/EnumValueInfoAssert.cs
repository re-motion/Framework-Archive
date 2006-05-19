using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  public static class EnumValueInfoAssert
  {
    public static void Contains (string expectedName, IList<EnumValueInfo> list, string message, params object[] args)
    {
      Assert.DoAssert (new EnumValueInfoListContentsAsserter(expectedName, list, message, args));
    }

    public static void Contains (string expectedName, IList<EnumValueInfo> list, string message)
    {
      Contains (expectedName, list, message, null);
    }

    public static void Contains (string expectedName, IList<EnumValueInfo> list)
    {
      Contains (expectedName, list, string.Empty, null);
    }
  }
}