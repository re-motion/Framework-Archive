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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class RegisterForAdditionalCommittingEventsTest : CommitFullEventChainTestBase
  {
    [Test]
    public void FullEventChain_WithReiterationDueToRegisterForAdditionalCommittingEvents ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            // This triggers _one_ (not two) additional run for _changedObject
            .WhenCalled (mi => Transaction.Execute (() =>
            {
              ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (ChangedObject);
              ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (ChangedObject);
            }));

        ExpectCommittingEvents (Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock))
            // This triggers one additional run for _newObject
            .WhenCalled (
                mi => Transaction.Execute (() => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (NewObject)));

        ExpectCommittingEvents (Tuple.Create (NewObject, NewObjectEventReceiverMock))
            // This triggers one additional run for _newObject
            .WhenCalled (
                mi => Transaction.Execute (() => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (NewObject)));

        ExpectCommittingEvents (Tuple.Create (NewObject, NewObjectEventReceiverMock));

        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject);

        // Committed events
        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }

    [Test]
    public void FullEventChain_WithReiterationDueToAddedObjectAndRegisterForAdditionalCommittingEvents ()
    {
      using (MockRepository.Ordered ())
      {
        ExpectCommittingEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (DeletedObject, DeletedObjectEventReceiverMock))
            // This triggers _one_ (not two) additional run for _unchangedObject
            .WhenCalled (mi => Transaction.Execute (() =>
            {
              Assert.That (
                  () => ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (UnchangedObject),
                  Throws.ArgumentException.With.Message.EqualTo (
                      string.Format (
                          "The given DomainObject '{0}' cannot be registered due to its state (Unchanged). Only objects that are part of the commit set "
                          + "can be registered. Use MarkAsChanged to add an unchanged object to the commit set.\r\nParameter name: domainObjects",
                          UnchangedObject.ID)));
               UnchangedObject.MarkAsChanged();
               ((ICommittingEventRegistrar) mi.Arguments[2]).RegisterForAdditionalCommittingEvents (UnchangedObject);
            }));

        ExpectCommittingEvents (Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
        
        ExpectCommitValidateEvents (ChangedObject, NewObject, DeletedObject, UnchangedObject);

        // Committed events
        ExpectCommittedEvents (
            Tuple.Create (ChangedObject, ChangedObjectEventReceiverMock),
            Tuple.Create (NewObject, NewObjectEventReceiverMock),
            Tuple.Create (UnchangedObject, UnchangedObjectEventReceiverMock));
      }
      MockRepository.ReplayAll ();

      Transaction.Commit ();

      MockRepository.VerifyAll ();
    }
  }
}