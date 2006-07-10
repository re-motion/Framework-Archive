using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullOrEmpty
	{
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentNullException))]
		public void Fail_NullString ()
		{
			ArgumentUtility.CheckNotNullOrEmpty ("arg", (string) null);
		}
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
		public void Fail_EmptyString ()
		{
			ArgumentUtility.CheckNotNullOrEmpty ("arg", "");
		}
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
		public void Fail_EmptyArray ()
		{
			ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[0]);
		}
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
		public void Fail_EmptyCollection ()
		{
			ArgumentUtility.CheckNotNullOrEmpty ("arg", new ArrayList ());
		}
		[Test]
		public void Succeed_String ()
		{
			ArgumentUtility.CheckNotNull ("arg", "Test");
		}
		[Test]
		public void Succeed_Array ()
		{
			ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[] { "test" });
		}
		[Test]
		public void Succeed_Collection ()
		{
			ArrayList list = new ArrayList ();
			list.Add ("test");
			ArgumentUtility.CheckNotNullOrEmpty ("arg", list);
		}
	}
}
