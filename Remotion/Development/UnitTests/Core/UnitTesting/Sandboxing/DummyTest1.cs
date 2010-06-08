// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  // This test is meant to be run by SandboxTestRunner.
  [TestFixture]
  public class DummyTest1
  {
    private int _value;

    [SetUp]
    public void SetUp ()
    {
      _value = 10;
    }

    [TearDown]
    public void TearDown ()
    {
      _value = 0;
    }

    [Test]
    public void Test1 ()
    {
      Assert.That (_value, Is.EqualTo (10));
      _value = 100;
    }

    [Test]
    public void Test2 ()
    {
      Assert.That (_value, Is.EqualTo (10));
      _value = 200;
    }

    [Test]
    [Ignore]
    public void IgnoreTest ()
    {
      Assert.Fail ("IgnoredTest");
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ExpectedExceptionTest ()
    {
      throw new NotImplementedException();
    }

    //[Test]
    //public void Test3 ()
    //{
    //  Assert.Fail ("Test");
    //}
  }
}