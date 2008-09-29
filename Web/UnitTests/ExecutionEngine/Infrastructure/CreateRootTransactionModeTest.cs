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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class CreateRootTransactionModeTest
  {
    [Test]
    public void GetStrategy ()
    {
      ITransactionMode transactionMode = new CreateRootTransactionMode<TestTransaction, TestTransactionScope, TestTransactionScopeManager> ();
      var executionListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener> ();
      ITransactionStrategy strategy = transactionMode.GetStrategy (new TestFunction2 (), executionListenerStub);

      Assert.That (strategy, Is.InstanceOfType (typeof (RootTransactionStrategy<TestTransaction, TestTransactionScope, TestTransactionScopeManager>)));
      Assert.That (((TransactionStrategyBase) strategy).InnerListener, Is.SameAs (executionListenerStub));
    }
  }
}