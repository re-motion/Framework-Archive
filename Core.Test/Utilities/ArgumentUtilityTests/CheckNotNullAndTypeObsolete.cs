using System;
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
		public void Fail_Null_String ()
		{
			ArgumentUtility.CheckNotNullAndType ("arg", (object) null, typeof (string));
		}
		
    [Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type_String ()
		{
			ArgumentUtility.CheckNotNullAndType ("arg", 13, typeof (string));
		}

    [Test]
    public void Succeed_String ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", "test", typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Null_Int ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", (object) null, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Type_Int ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", 13.0, typeof (int));
    }

    [Test]
    public void Succeed_Int ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", 10, typeof (int));
    }
	}
}
