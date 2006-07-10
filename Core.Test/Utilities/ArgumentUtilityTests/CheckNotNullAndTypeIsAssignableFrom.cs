using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullAndTypeIsAssignableFrom
	{
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Null ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", null, typeof (string));
    }
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Type ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", typeof (object), typeof (string));
    }
    [Test]
    public void Succeed ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", typeof (string), typeof (object));
    }

	}
}
