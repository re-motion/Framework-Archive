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
using Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class TransactionExecutionListenerTest
  {
    private WxeContext _wxeContext;
    private TransactionStrategyBase _transactionStrategyMock;
    private IWxeFunctionExecutionListener _innerListenerStub;
    private IWxeFunctionExecutionListener _transactionListener;

    [SetUp]
    public void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _wxeContext = wxeContextFactory.CreateContext (new TestFunction());
      _transactionStrategyMock = MockRepository.GenerateMock<TransactionStrategyBase>();
      _innerListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener>();
      _transactionListener = new TransactionExecutionListener (_transactionStrategyMock, _innerListenerStub);
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _transactionListener.OnExecutionPlay (_wxeContext);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionPlay (_wxeContext, _innerListenerStub));
    }

    [Test]
    public void OnExecutionStop ()
    {
      _transactionListener.OnExecutionStop (_wxeContext);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionStop (_wxeContext, _innerListenerStub));
    }

    [Test]
    public void OnExecutionPause ()
    {
      _transactionListener.OnExecutionPause (_wxeContext);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionPause (_wxeContext, _innerListenerStub));
    }

    [Test]
    public void OnExecutionFail ()
    {
      Exception exception = new Exception();

      _transactionListener.OnExecutionFail (_wxeContext, exception);

      _transactionStrategyMock.AssertWasCalled (mock => mock.OnExecutionFail (_wxeContext, _innerListenerStub, exception));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_transactionListener.IsNull, Is.False);
    }
  }
}