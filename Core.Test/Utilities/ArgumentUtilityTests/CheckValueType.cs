using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckValueType
	{
    [Test]
    public void Succeed_Null ()
    {
      int? i = ArgumentUtility.CheckValueType<int> ("arg", null);
      Assert.IsNull (i);
    }

    [Test]
    public void Succeed ()
    {
      int? i = ArgumentUtility.CheckValueType<int> ("arg", 3);
      Assert.AreEqual (3, i);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Type ()
    {
      ArgumentUtility.CheckValueType<int> ("arg", (long) 3);
    }
	}
}
