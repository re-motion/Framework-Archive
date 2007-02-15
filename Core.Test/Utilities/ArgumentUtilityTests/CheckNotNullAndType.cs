using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullAndType
	{
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Fail_Null ()
		{
			ArgumentUtility.CheckNotNullAndType<string> ("arg", null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Fail_NullValueType ()
		{
			int? arg = null;
			ArgumentUtility.CheckNotNullAndType<string> ("arg", arg);
		}

		[Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
		{
			ArgumentUtility.CheckNotNullAndType<string> ("arg", 13);
		}

		[Test]
		public void Succeed_String ()
		{
			string result = ArgumentUtility.CheckNotNullAndType<string> ("arg", "test");
			Assert.AreEqual ("test", result);
		}
	}
}
