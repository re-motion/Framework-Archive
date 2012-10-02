// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections;
using NUnit.Framework;
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Reset : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test_WithoutScope ()
    {
      var object1 = new object ();
      var object2 = new object ();

      TransactionMock.BackToRecord ();
      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.Reset ());

        ExecutionContextMock.Expect (mock => mock.GetVariables()).Return (new[] { object1, object2 });
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2 })));
      }
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithoutScope_And_ResetThrows ()
    {
      var exception = new ApplicationException ("Reset Exception");
      TransactionMock.Expect (mock => mock.Reset ()).Throw (exception);
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithScope ()
    {
      var object1 = new object ();
      var object2 = new object ();

      InvokeOnExecutionPlay (_strategy);
      TransactionMock.BackToRecord ();
      var newScopeMock = MockRepository.StrictMock<ITransactionScope>();

      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Reset ());
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (newScopeMock);

        ExecutionContextMock.Expect (mock => mock.GetVariables()).Return (new[] { object1, object2 });
        TransactionMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2 })));
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.SameAs (newScopeMock));
    }

    [Test]
    public void Test_WithScope_And_LeaveThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Leave Exception");
      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll ();

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithScope_And_ResetThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Reset Exception");
      using (MockRepository.Ordered())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Reset ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll();

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }
  }
}
