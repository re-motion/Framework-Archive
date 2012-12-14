using System;
using System.Globalization;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using System.Data.SqlTypes;

namespace Rubicon.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaBooleanTest
{
  [Test]
  [Ignore("Logical AND is obsolete.")]
  [Obsolete]
  public void LogicalAnd ()
  {
    AssertLogicalAnd (NaBoolean.True, NaBoolean.True, false, NaBoolean.True);
    AssertLogicalAnd (NaBoolean.True, NaBoolean.False, false, NaBoolean.False);
    AssertLogicalAnd (NaBoolean.True, NaBoolean.Null, false, NaBoolean.False);

    AssertLogicalAnd (NaBoolean.False, NaBoolean.True, true, NaBoolean.False);
    AssertLogicalAnd (NaBoolean.False, NaBoolean.False, true, NaBoolean.False);
    AssertLogicalAnd (NaBoolean.False, NaBoolean.Null, true, NaBoolean.False);

    AssertLogicalAnd (NaBoolean.Null, NaBoolean.True, false, NaBoolean.False);
    AssertLogicalAnd (NaBoolean.Null, NaBoolean.False, false, NaBoolean.False);
    AssertLogicalAnd (NaBoolean.Null, NaBoolean.Null, false, NaBoolean.Null);
  }

  [Obsolete]
  private void AssertLogicalAnd (NaBoolean x, NaBoolean y, bool shortCircuitExpected, NaBoolean expectedResult)
  {
    string operation = string.Format ("Performing {0} && {0}", x, y);
    _returnValueExecuted = false;

    NaBoolean result = x && ReturnValue (y);

    if (shortCircuitExpected)
      Assert.IsFalse (_returnValueExecuted, operation + ": Short circuit was expected.");
    else
      Assert.IsTrue (_returnValueExecuted, operation + ": Short circuit was not expected.");

    Assert.IsTrue (result == expectedResult, string.Format ("{0}: Expected result {1} but was {2}", operation, expectedResult, result));
  }

  private bool _returnValueExecuted;
  private NaBoolean ReturnValue (NaBoolean b)
  {
    _returnValueExecuted = true;
    return b;
  }
}

//[TestFixture]
//public class SqlBooleanTest
//{
//  [Test]
//  public void LogicalAnd ()
//  {
//    AssertLogicalAnd (SqlBoolean.True, SqlBoolean.True, false, SqlBoolean.True);
//    AssertLogicalAnd (SqlBoolean.True, SqlBoolean.False, false, SqlBoolean.False);
//    AssertLogicalAnd (SqlBoolean.True, SqlBoolean.Null, false, SqlBoolean.Null);
//
//    AssertLogicalAnd (SqlBoolean.False, SqlBoolean.True, true, SqlBoolean.False);
//    AssertLogicalAnd (SqlBoolean.False, SqlBoolean.False, true, SqlBoolean.False);
//    AssertLogicalAnd (SqlBoolean.False, SqlBoolean.Null, true, SqlBoolean.False);
//
//    AssertLogicalAnd (SqlBoolean.Null, SqlBoolean.True, false, SqlBoolean.Null);
//    AssertLogicalAnd (SqlBoolean.Null, SqlBoolean.False, false, SqlBoolean.False);
//    AssertLogicalAnd (SqlBoolean.Null, SqlBoolean.Null, false, SqlBoolean.Null);
//  }
//
//  private void AssertLogicalAnd (SqlBoolean x, SqlBoolean y, bool shortCircuitExpected, SqlBoolean expectedResult)
//  {
//    string operation = string.Format ("Performing {0} && {1}", x, y);
//    _returnValueExecuted = false;
//
//    SqlBoolean result = x && ReturnValue (y);
//
//    if (shortCircuitExpected)
//      Assert.IsFalse (_returnValueExecuted, operation + ": Short circuit was expected.");
//    else
//      Assert.IsTrue (_returnValueExecuted, operation + ": Short circuit was not expected.");
//
//    bool equal = (   result.IsTrue && expectedResult.IsTrue
//                  || result.IsFalse && expectedResult.IsFalse
//                  || result.IsNull && expectedResult.IsNull);
//
//    Assert.IsTrue (equal, string.Format ("{0}: Expected result {1} but was {2}", operation, expectedResult, result));
//  }
//
//  private bool _returnValueExecuted;
//  private SqlBoolean ReturnValue (SqlBoolean b)
//  {
//    _returnValueExecuted = true;
//    return b;
//  }
//}

}