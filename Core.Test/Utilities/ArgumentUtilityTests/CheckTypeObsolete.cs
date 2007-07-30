using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
  [Obsolete ("The tested methods are obsolete.")]
	public class CheckTypeObsolete
	{
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
    {
      ArgumentUtility.CheckType ("arg", 13, typeof (string));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
		public void Fail_ValueType ()
    {
      ArgumentUtility.CheckType ("arg", (object) null, typeof (int));
    }

    [Test]
		public void Succeed_ReferenceTypeNull ()
    {
      ArgumentUtility.CheckType ("arg", (object) null, typeof (string));
    }

    [Test]
		public void Succeed_NotNull ()
    {
      ArgumentUtility.CheckType ("arg", "test", typeof (string));
    }
	}
}
