using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNull
	{
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentNullException))]
		public void Fail ()
		{
			ArgumentUtility.CheckNotNull ("arg", null);
		}
		[Test]
		public void Succeed ()
		{
			ArgumentUtility.CheckNotNull ("arg", "test");
		}
	}
}
