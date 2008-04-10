using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullAndType
	{
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]
    [Test]
    public void Succeed_Int ()
    {
      int result = ArgumentUtility.CheckNotNullAndType<int> ("arg", 1);
      Assert.AreEqual (1, result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Int_Null ()
    {
      ArgumentUtility.CheckNotNullAndType<int> ("arg", null);
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      int result = ArgumentUtility.CheckNotNullAndType<int> ("arg", (int?)1);
      Assert.AreEqual (1, result);
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckNotNullAndType<int?> ("arg", (int?) 1);
      Assert.AreEqual (1, result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullableInt_Null ()
    {
      ArgumentUtility.CheckNotNullAndType<int?> ("arg", null);
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckNotNullAndType<int?> ("arg", 1);
      Assert.AreEqual (1, result);
    }

		[Test]
		public void Succeed_String ()
		{
			string result = ArgumentUtility.CheckNotNullAndType<string> ("arg", "test");
			Assert.AreEqual ("test", result);
		}    
    
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_StringNull ()
    {
      ArgumentUtility.CheckNotNullAndType<string> ("arg", null);
    }

	  private enum TestEnum
	  {
	    TestValue
	  } 

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckNotNullAndType<TestEnum> ("arg", TestEnum.TestValue);
      Assert.AreEqual (TestEnum.TestValue, result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Enum_Null ()
    {
      ArgumentUtility.CheckNotNullAndType<TestEnum> ("arg", null);
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckNotNullAndType<object> ("arg", "test");
      Assert.AreEqual ("test", result);
    }
    
    [Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_String_Int ()
		{
			ArgumentUtility.CheckNotNullAndType<string> ("arg", 1);
		}

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Long_Int ()
    {
      ArgumentUtility.CheckNotNullAndType<long> ("arg", 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Int_String ()
    {
      ArgumentUtility.CheckNotNullAndType<int> ("arg", "test");
    }
  }
}
