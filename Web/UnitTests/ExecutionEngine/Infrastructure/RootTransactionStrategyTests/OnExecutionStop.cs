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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop : RootTransactionStrategyTestBase
  {
    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerMock);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit());
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerMock);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithParentTransactionStrategy ()
    {
      var strategy = CreateRootTransactionStrategy (true, OuterTransactionStrategyMock);
      var expectedObjects = new[] { new object() };

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit());

        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (expectedObjects);
        OuterTransactionStrategyMock.Expect (mock => mock.RegisterObjects (expectedObjects));

        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerMock);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithChildStrategy ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      strategy.SetChild (ChildTransactionStrategyMock);

      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerMock));
        TransactionMock.Expect (mock => mock.Commit());

        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerMock);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionStop may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);

      Assert.That (strategy.Scope, Is.Null);

      strategy.OnExecutionStop (Context, ExecutionListenerMock);
    }

    [Test]
    public void Test_InnerListenerThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new WxeFatalExecutionException (new Exception ("ChildStrategy Exception"), null);

      InvokeOnExecutionPlay (strategy);
      strategy.SetChild (ChildTransactionStrategyMock);

      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerMock)).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_CommitThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_GetOutParameterThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));

        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Throw (innerException);

        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_RegisterObjectsThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));

        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        OuterTransactionStrategyMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.Is.Anything)).Throw (innerException);

        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave()).Throw (innerException);
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release()).Throw (innerException);
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_InnerListenerThrows_And_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave()).Throw (outerException);
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_CommitThrows_And_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new Exception ("Commit Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave()).Throw (outerException);
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_InnerListenerThrows_And_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release()).Throw (outerException);
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_CommitThrows_And_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new Exception ("Commit Exception");
      var outerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release()).Throw (outerException);
      }

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerMock);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }
  }
}