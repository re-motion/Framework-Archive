using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
  [Obsolete ("The tested methods are obsolete.")]
	public class CheckNotNullAndTypeObsolete
	{
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Fail_Null ()
		{
			ArgumentUtility.CheckNotNullAndType ("arg", (object) null, typeof (string));
		}
		[Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
		{
			ArgumentUtility.CheckNotNullAndType ("arg", 13, typeof (string));
		}
		[Test]
		public void Succeed ()
		{
			ArgumentUtility.CheckNotNullAndType ("arg", "test", typeof (string));
		}
	}
}
