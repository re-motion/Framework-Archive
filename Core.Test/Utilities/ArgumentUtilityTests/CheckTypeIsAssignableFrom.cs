using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckTypeIsAssignableFrom
	{
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail ()
    {
      ArgumentUtility.CheckTypeIsAssignableFrom ("arg", typeof (object), typeof (string));
    }
    [Test]
    public void Succeed_Null ()
    {
      ArgumentUtility.CheckTypeIsAssignableFrom ("arg", null, typeof (object));
    }
    [Test]
    public void Succeed ()
    {
      ArgumentUtility.CheckTypeIsAssignableFrom ("arg", typeof (string), typeof (object));
    }
	}
}
