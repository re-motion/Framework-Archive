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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NoneTransactionModeTest
  {
    [Test]
    public void CreateTransactionStrategy_WithoutParentFunction ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction ());
      
      ITransactionMode transactionMode = new NoneTransactionMode ();
      TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy (new TestFunction2 (transactionMode), context);

      Assert.That (strategy, Is.InstanceOfType (typeof (NoneTransactionStrategy)));
      Assert.That (strategy.OuterTransactionStrategy, Is.SameAs (NullTransactionStrategy.Null));
    }

    [Test]
    public void CreateTransactionStrategy_WithParentFunction ()
    {
      ITransactionMode transactionMode = new NoneTransactionMode ();

      WxeFunction parentFunction = new TestFunction2 (new NoneTransactionMode());
      WxeFunction childFunction = new TestFunction2 (transactionMode);
      parentFunction.Add (childFunction);

      WxeStep stepMock = MockRepository.GenerateMock<WxeStep> ();
      childFunction.Add (stepMock);

      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction ());

      stepMock.Expect (mock => mock.Execute (context)).Do (
          invocation =>
          {
            TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy (childFunction, context);
            Assert.That (strategy, Is.InstanceOfType (typeof (NoneTransactionStrategy)));
            Assert.That (strategy.OuterTransactionStrategy, Is.SameAs (((TestFunction2)parentFunction).TransactionStrategy));
          });

      parentFunction.Execute (context);
    }

    [Test]
    public void IsSerializeable ()
    {
      var deserialized = Serializer.SerializeAndDeserialize (new NoneTransactionMode ());

      Assert.That (deserialized.AutoCommit, Is.False);
    }
  }
}