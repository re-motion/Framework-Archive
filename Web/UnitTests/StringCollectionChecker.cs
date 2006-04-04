using System;
using System.Collections.Specialized;

using NUnit.Framework;

namespace Rubicon.Web.UnitTests
{

  public class StringCollectionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public StringCollectionChecker ()
    {
    }

    // methods and properties

    public void AreEqual (StringCollection expected, StringCollection actual)
    {
      if (expected != actual)
        Assert.AreEqual (ToArray (expected), ToArray (actual));
    }

    private string[] ToArray (StringCollection collection)
    {
      string[] array = null;
      if (collection != null)
      {
        array = new string[collection.Count];
        collection.CopyTo (array, 0);
      }

      return array;
    }
  }

}
