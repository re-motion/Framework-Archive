using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities.ArgumentUtilityTests
{
  public enum TestEnum
  {
    Value1 = 1,
    Value2 = 2,
    Value3 = 3
  }

  [Flags]
  public enum TestFlags
  {
    Flag1 = 1,
    Flag2 = 2,
    Flag3 = 4,
    Flag13 = Flag1 | Flag3
  }

	[TestFixture]
	public class CheckValidEnumValue
	{
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_UndefinedValue ()
		{
      ArgumentUtility.CheckValidEnumValue ("arg", (TestEnum) 4);
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_PartiallyUndefinedFlags ()
		{
      ArgumentUtility.CheckValidEnumValue ("arg", (TestFlags) (1 | 8));
		}

    [Test]
		public void Succeed_SingleValue ()
		{
			ArgumentUtility.CheckValidEnumValue ("arg", TestEnum.Value1);
		}
		[Test]
		public void Succeed_Flags ()
		{
			ArgumentUtility.CheckValidEnumValue ("arg", TestFlags.Flag1 | TestFlags.Flag2);
		}
	}

	[TestFixture]
	public class CheckValidEnumValueAndTypeAndNotNull
	{
		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentNullException))]
		public void Fail_Null ()
		{
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestEnum> ("arg", null);
		}
    
    [Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_UndefinedValue ()
		{
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestEnum> ("arg", (TestEnum) 4);
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_PartiallyUndefinedFlags ()
		{
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestFlags> ("arg", (TestFlags) (1 | 8));
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentTypeException))]
    public void Fail_WrongType ()
    {
      ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestFlags> ("arg", TestEnum.Value1);
    }

    [Test]
		public void Succeed_SingleValue ()
		{
      TestEnum result = ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestEnum> ("arg", TestEnum.Value1);
      Assert.AreEqual (TestEnum.Value1, result);
		}
		[Test]
		public void Succeed_Flags ()
		{
      TestFlags result = ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<TestFlags> ("arg", TestFlags.Flag1 | TestFlags.Flag2);
      Assert.AreEqual (TestFlags.Flag1 | TestFlags.Flag2, result);
		}
	}

	[TestFixture]
	public class CheckValidEnumValueAndType
	{
		[Test]
		public void Succeed_Null ()
		{
      TestEnum? result = ArgumentUtility.CheckValidEnumValueAndType<TestEnum> ("arg", null);
      Assert.IsNull (result);
		}
    
    [Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_UndefinedValue ()
		{
      ArgumentUtility.CheckValidEnumValueAndType<TestEnum> ("arg", (TestEnum) 4);
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentOutOfRangeException))]
		public void Fail_PartiallyUndefinedFlags ()
		{
      ArgumentUtility.CheckValidEnumValueAndType<TestFlags> ("arg", (TestFlags) (1 | 8));
		}

		[Test]
		[ExpectedExceptionAttribute (typeof (ArgumentTypeException))]
    public void Fail_WrongType ()
    {
      ArgumentUtility.CheckValidEnumValueAndType<TestFlags> ("arg", TestEnum.Value1);
    }

    [Test]
		public void Succeed_SingleValue ()
		{
      TestEnum? result = ArgumentUtility.CheckValidEnumValueAndType<TestEnum> ("arg", TestEnum.Value1);
      Assert.AreEqual (TestEnum.Value1, result);
		}
		[Test]
		public void Succeed_Flags ()
		{
      TestFlags? result = ArgumentUtility.CheckValidEnumValueAndType<TestFlags> ("arg", TestFlags.Flag1 | TestFlags.Flag2);
      Assert.AreEqual (TestFlags.Flag1 | TestFlags.Flag2, result);
		}
	}
}
