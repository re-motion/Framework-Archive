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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionFactoryTest
  {
    [Test]
    public void CreateRootTransaction ()
    {
      ITransactionFactory transactionFactory = new ClientTransactionFactory ();

      ITransaction transaction = transactionFactory.CreateRootTransaction();
      Assert.That (transaction, Is.InstanceOfType (typeof (ClientTransactionWrapper)));
      Assert.That (transaction.To<ClientTransaction>(), Is.InstanceOfType (typeof (RootClientTransaction)));
    }
  }
}