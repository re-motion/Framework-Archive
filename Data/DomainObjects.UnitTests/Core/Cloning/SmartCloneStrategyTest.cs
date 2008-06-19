
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
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Cloning
{
  [TestFixture]
  public class SmartCloneStrategyTest : CloneStrategyTestBase
  {
    private SmartCloneStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp ();
      _strategy = new SmartCloneStrategy ();
    }

    protected override void HandleReference_OneOne_RealSide_Checks (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneOne_RealSide_Checks_Null (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.Null);
    }

    protected override void HandleReference_OneOne_VirtualSide_Checks (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneOne_VirtualSide_Checks_Null (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.Null);
    }

    [Test]
    public override void HandleReference_OneMany_RealSide ()
    {
      CloneTransaction = SourceTransaction; // need same transaction
      base.HandleReference_OneMany_RealSide ();
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Property 'Remotion.Data.DomainObjects.UnitTests.TestDomain." 
        + "OrderItem.Order' of DomainObject 'OrderItem|.*|System.Guid' cannot be set to DomainObject " 
          + "'Order|.*|System.Guid', because the objects do not belong to the same ClientTransaction.", MatchType = MessageMatch.Regex)]
    public void HandleReference_OneMany_RealSide_ThrowsOnDifferentTransactions ()
    {
      base.HandleReference_OneMany_RealSide ();
    }

    protected override void HandleReference_OneMany_RealSide_Checks (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no clone call for Order

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.SameAs (sourceRelated));
      Assert.That (sourceRelated.OrderItems, List.Contains (cloneReference.DomainObject));
    }

    protected override void HandleReference_OneMany_RealSide_Checks_Null (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.Null);
    }

    protected override void HandleReference_OneMany_VirtualSide_Checks (OrderItem sourceRelated, PropertyAccessor sourceReference, OrderItem cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (((DomainObjectCollection)cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction))[0], Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneMany_VirtualSide_Checks_Null (PropertyAccessor sourceReference, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, SourceTransaction, cloneReference, CloneTransaction, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheckTx (CloneTransaction), Is.Empty);
    }
  }
}