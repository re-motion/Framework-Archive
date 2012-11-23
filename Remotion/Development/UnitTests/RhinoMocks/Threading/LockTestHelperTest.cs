﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Development.RhinoMocks.UnitTesting.Threading;

namespace Remotion.Development.UnitTests.RhinoMocks.Threading
{
  [TestFixture]
  public class LockTestHelperTest
  {
    [Test]
    public void CheckLockIsHeld ()
    {
      var lockObject = new object();

      lock (lockObject)
        Assert.That (() => LockTestHelper.CheckLockIsHeld (lockObject), Throws.Nothing);

      Assert.That (
          () => LockTestHelper.CheckLockIsHeld (lockObject),
          Throws.TypeOf<AssertionException>().With.Message.StartsWith ("  Parallel thread should have been blocked."));
    }
  }
}