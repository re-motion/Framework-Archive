/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class ArrayUtilityTest
{
  [Test]
  public void TestCombine()
  {
    string[] s1 = { "a", "b", "c" };
    string[] s2 = { "d" };
    string[] s3 = {};
    string[] s4 = { "e", "f" };

    string[] res = (string[]) ArrayUtility.Combine (s1, s2, s3, s4);
    Assert.AreEqual ("abcdef", string.Concat (res));
  }

  [Test]
  public void TestConvert ()
  {
    object[] o1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Convert<object, string> (o1);
    Assert.AreEqual ("abcd", string.Concat (res));
  }

  [Test]
  public void TestConvertWithNull ()
  {
    Assert.IsNull (ArrayUtility.Convert<object, string> (null));
  }

  [Test]
  public void TestInsertFirst()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 0, "X");
    Assert.AreEqual ("Xabcd", string.Concat (res));
  }
  [Test]
  public void TestInsertMiddle()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 2, "X");
    Assert.AreEqual ("abXcd", string.Concat (res));
  }
  [Test]
  public void TestInsertEnd()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 4, "X");
    Assert.AreEqual ("abcdX", string.Concat (res));
  }
  [ExpectedException (typeof (IndexOutOfRangeException))]
  [Test]
  public void TestInsertPastEnd()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 5, "X");
  }

  [Test]
  public void TestSkip ()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Skip (s1, 2);
    Assert.AreEqual ("cd", string.Concat (res));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void TestSkipFail ()
  {
    ArrayUtility.Skip (new int[3], 4);
  }
}

}
