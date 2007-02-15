using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullAndValueType
	{
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Fail_Null ()
		{
			ArgumentUtility.CheckNotNullAndValueType<int> ("arg", null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Fail_NullValueType ()
		{
			int? arg = null;
			ArgumentUtility.CheckNotNullAndValueType<int> ("arg", arg);
		}

		[Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
		{
			ArgumentUtility.CheckNotNullAndValueType<long> ("arg", 13);
		}

		[Test]
		public void Succeed ()
		{
			int result = ArgumentUtility.CheckNotNullAndValueType<int> ("arg", 3);
			Assert.AreEqual (3, result);
		}
	}
}
