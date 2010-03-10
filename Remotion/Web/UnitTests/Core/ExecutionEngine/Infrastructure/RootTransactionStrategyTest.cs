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
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class RootTransactionStrategyTest
  {
    private RootTransactionStrategy _strategy;
    private ITransaction _transactionMock;
    private TransactionStrategyBase _outerTransactionStrategyStub;
    private IWxeFunctionExecutionContext _executionContextStub;

    [SetUp]
    public void SetUp ()
    {
      _transactionMock = MockRepository.GenerateMock<ITransaction>();
      _outerTransactionStrategyStub = MockRepository.GenerateStub<TransactionStrategyBase>();
      _executionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      _executionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      _strategy = new RootTransactionStrategy (true, _transactionMock, _outerTransactionStrategyStub, _executionContextStub);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (_transactionMock));
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (_outerTransactionStrategyStub));
      Assert.That (_strategy.ExecutionContext, Is.SameAs (_executionContextStub));
      Assert.That (_strategy.AutoCommit, Is.True);
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      var innerExecutionListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener>();
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener (innerExecutionListenerStub);

      Assert.That (executionListener, Is.InstanceOfType (typeof (RootTransactionExecutionListener)));
      var transactionExecutionListener = (RootTransactionExecutionListener) executionListener;
      Assert.That (transactionExecutionListener.InnerListener, Is.SameAs (innerExecutionListenerStub));
      Assert.That (transactionExecutionListener.TransactionStrategy, Is.SameAs (_strategy));
    }
  }
}
