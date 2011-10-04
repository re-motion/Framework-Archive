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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ExtensionClientTransactionListenerTest
  {
    private IClientTransactionExtension _extensionMock;
    private ExtensionClientTransactionListener _listener;

    [SetUp]
    public void SetUp ()
    {
      _extensionMock = MockRepository.GenerateStrictMock<IClientTransactionExtension>();
      _listener = new ExtensionClientTransactionListener (_extensionMock);
    }

    [Test]
    public void SubTransactionEvents_Delegated ()
    {
      var tx1 = ClientTransaction.CreateRootTransaction();
      var tx2 = ClientTransaction.CreateRootTransaction();
      
      ExpectDelegation (l => l.SubTransactionCreating (tx1), e => e.SubTransactionCreating (tx1));
      ExpectDelegation (l => l.SubTransactionCreated (tx1, tx2), e => e.SubTransactionCreated (tx1, tx2));
    }

    private void ExpectDelegation (Action<IClientTransactionListener> listenerAction, Action<IClientTransactionExtension> expectedExtensionAction)
    {
      _extensionMock.Expect (expectedExtensionAction);
      _extensionMock.Replay();

      listenerAction (_listener);

      _extensionMock.VerifyAllExpectations();
    }
  }
}