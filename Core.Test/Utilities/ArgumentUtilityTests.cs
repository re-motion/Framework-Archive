using System;
using System.Collections;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.UnitTests.Utilities
{

[TestFixture]
public class ArgumentUtilityTests
{
  [Test]
  [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
  public void CheckNotNull_Fail ()
  {
    ArgumentUtility.CheckNotNull ("arg", null);
  }
  [Test]
  public void CheckNotNull_Succeed ()
  {
    ArgumentUtility.CheckNotNull ("arg", "test");
  }

  [Test]
  [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
  public void CheckNotNullOrEmpty_FailNullString ()
  {
    ArgumentUtility.CheckNotNullOrEmpty ("arg", (string) null);
  }
  [Test]
  [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
  public void CheckNotNullOrEmpty_FailEmptyString ()
  {
    ArgumentUtility.CheckNotNullOrEmpty ("arg", "");
  }
  [Test]
  [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
  public void CheckNotNullOrEmpty_FailEmptyArray ()
  {
    ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[0]);
  }
  [Test]
  [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
  public void CheckNotNullOrEmpty_FailEmptyCollection ()
  {
    ArgumentUtility.CheckNotNullOrEmpty ("arg", new ArrayList());
  }
  [Test]
  public void CheckNotNullOrEmptySucceed_String ()
  {
    ArgumentUtility.CheckNotNull ("arg", "Test");
  }
  [Test]
  public void CheckNotNullOrEmpty_SucceedArray ()
  {
    ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[] {"test"});
  }
  [Test]
  public void CheckNotNullOrEmpty_SucceedCollection ()
  {
    ArrayList list = new ArrayList();
    list.Add ("test");
    ArgumentUtility.CheckNotNullOrEmpty ("arg", list);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void CheckNotNullAndType_FailNull ()
  {
    ArgumentUtility.CheckNotNullAndType ("arg", null, typeof (string));
  }
  [Test]
  [ExpectedException (typeof (ArgumentTypeException))]
  public void CheckNotNullAndType_FailType ()
  {
    ArgumentUtility.CheckNotNullAndType ("arg", 13, typeof (string));
  }
  [Test]
  public void CheckNotNullAndType_Succeed ()
  {
    ArgumentUtility.CheckNotNullAndType ("arg", "test", typeof (string));
  }

  [Test]
  [ExpectedException (typeof (ArgumentTypeException))]
  public void CheckType_FailType ()
  {
    ArgumentUtility.CheckType ("arg", 13, typeof (string));
  }
  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void CheckType_FailValueType ()
  {
    ArgumentUtility.CheckType ("arg", null, typeof (int));
  }
  [Test]
  public void CheckType_SucceedReferenceTypeNull ()
  {
    ArgumentUtility.CheckType ("arg", null, typeof (string));
  }
  [Test]
  public void CheckType_SucceedNotNull ()
  {
    ArgumentUtility.CheckType ("arg", "test", typeof (string));
  }
}

}
