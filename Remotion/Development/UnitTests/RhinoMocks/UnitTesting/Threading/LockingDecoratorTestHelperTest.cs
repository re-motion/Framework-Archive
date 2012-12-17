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
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace Remotion.Development.UnitTests.RhinoMocks.UnitTesting.Threading
{
  [TestFixture]
  public class LockingDecoratorTestHelperTest
  {
    private LockingDecoratorTestHelper<IMyInterface> _helperForLockingDecorator;
    private LockingDecoratorTestHelper<IMyInterface> _helperForNonLockingDecorator;
    private LockingDecoratorTestHelper<IMyInterface> _helperForNonDelegatingDecorator;
    private LockingDecoratorTestHelper<IMyInterface> _helperForFaultyDecorator;

    [SetUp]
    public void SetUp ()
    {
      var lockObject = new object();

      _helperForLockingDecorator = CreateLockingDecoratorTestHelper (
          inner =>
          {
            lock (lockObject)
              return inner.Get();
          },
          (inner, s) =>
          {
            lock (lockObject)
              inner.Do (s);
          },
          lockObject);

      _helperForNonLockingDecorator = CreateLockingDecoratorTestHelper (inner => inner.Get(), (inner, s) => inner.Do (s), lockObject);
      _helperForNonDelegatingDecorator = CreateLockingDecoratorTestHelper (inner => "Abc", (inner, s) => { }, lockObject);
      _helperForFaultyDecorator = CreateLockingDecoratorTestHelper (
          inner =>
          {
            lock (lockObject)
              inner.Get();
            return "faulty";
          },
          (inner, s) =>
          {
            lock (lockObject)
              inner.Do ("faulty");
          },
          lockObject);
    }

    [Test]
    public void ExpectSynchronizedDelegation_Func ()
    {
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Get(), "Abc"), Throws.Nothing);
      Assert.That (
          () => _helperForNonLockingDecorator.ExpectSynchronizedDelegation (d => d.Get(), "Abc"),
          Throws.TypeOf<AssertionException>().And.Message.StringStarting ("  Parallel thread should have been blocked."));
      Assert.That (
          () => _helperForNonDelegatingDecorator.ExpectSynchronizedDelegation (d => d.Get(), "Abc"),
          Throws.TypeOf<ExpectationViolationException>().And.Message.EqualTo ("IMyInterface.Get(); Expected #1, Actual #0."));
      Assert.That (
          () => _helperForFaultyDecorator.ExpectSynchronizedDelegation (d => d.Get(), "Abc"),
          Throws.TypeOf<AssertionException>().And.Message.StringStarting ("  Expected string length 3 but was 6. Strings differ at index 0."));
    }

    [Test]
    public void ExpectSynchronizedDelegation_Func_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Get (), "Abc"), Throws.Nothing);
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Get (), "Abc"), Throws.Nothing);
    }

    [Test]
    public void ExpectSynchronizedDelegation_Func_WithResultChecker ()
    {
      Assert.That (
          () => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Get(), "Abc", s => Assert.That (s, Is.EqualTo ("Abc"))),
          Throws.Nothing);
      Assert.That (
          () => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Get (), "Abc", s => Assert.That (s, Is.EqualTo ("Expected"))),
          Throws.TypeOf<AssertionException> ());
    }

    [Test]
    public void ExpectSynchronizedDelegation_Action ()
    {
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (
          () => _helperForNonLockingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<AssertionException>().And.Message.StringStarting ("  Parallel thread should have been blocked."));
      Assert.That (
          () => _helperForNonDelegatingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<ExpectationViolationException>().And.Message.EqualTo ("IMyInterface.Do(\"Abc\"); Expected #1, Actual #0."));
      Assert.That (
          () => _helperForFaultyDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<ExpectationViolationException>().And.Message.EqualTo (
              "IMyInterface.Do(\"faulty\"); Expected #0, Actual #1.\r\nIMyInterface.Do(\"Abc\"); Expected #1, Actual #0."));
    }

    [Test]
    public void ExpectSynchronizedDelegation_Action_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")), Throws.Nothing);
    }

    [Test]
    public void ExpectSynchronizedDelegation_Mixed_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForLockingDecorator.ExpectSynchronizedDelegation (d => d.Get (), "test"), Throws.Nothing);
    }

    private LockingDecoratorTestHelper<IMyInterface> CreateLockingDecoratorTestHelper (
        Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod, object lockObject)
    {
      var innerMock = MockRepository.GenerateStrictMock<IMyInterface> ();
      var decorator = new Decorator (innerMock, getMethod, doMethod);

      return new LockingDecoratorTestHelper<IMyInterface> (decorator, lockObject, innerMock);
    }

    public interface IMyInterface
    {
      string Get ();
      void Do (string s);
    }

    private class Decorator : IMyInterface
    {
      private readonly IMyInterface _inner;
      private readonly Func<IMyInterface, string> _getMethod;
      private readonly Action<IMyInterface, string> _doMethod;

      public Decorator (IMyInterface inner, Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod)
      {
        _inner = inner;
        _getMethod = getMethod;
        _doMethod = doMethod;
      }

      public string Get ()
      {
        return _getMethod (_inner);
      }

      public void Do (string s)
      {
        _doMethod (_inner, s);
      }
    }
  }
}