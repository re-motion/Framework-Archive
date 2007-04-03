using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNull
	{
    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
    public void Nullable_Fail ()
    {
      ArgumentUtility.CheckNotNull ("arg", (int?) null);
    }
    [Test]
    public void Nullable_Succeed ()
    {
      ArgumentUtility.CheckNotNull ("arg", (int?) 0);
    }

    [Test]
    public void Value_Succeed ()
    {
      ArgumentUtility.CheckNotNull ("arg", (int) 0);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
    public void Reference_Fail ()
    {
      ArgumentUtility.CheckNotNull ("arg", (string) null);
    }
    [Test]
    public void Reference_Succeed ()
    {
      ArgumentUtility.CheckNotNull ("arg", string.Empty);
    }
	}
}
