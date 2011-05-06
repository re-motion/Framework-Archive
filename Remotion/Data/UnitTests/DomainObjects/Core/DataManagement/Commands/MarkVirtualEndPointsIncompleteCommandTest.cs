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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class MarkVirtualEndPointsIncompleteCommandTest
  {
    private MockRepository _mockRepository;
    private IVirtualEndPoint _endPointMock1;
    private IVirtualEndPoint _endPointMock2;

    private MarkVirtualEndPointsIncompleteCommand _command;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _endPointMock1 = _mockRepository.StrictMock<IVirtualEndPoint> ();
      _endPointMock2 = _mockRepository.StrictMock<IVirtualEndPoint> ();

      _command = new MarkVirtualEndPointsIncompleteCommand (new[] { _endPointMock1, _endPointMock2 });
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      _mockRepository.ReplayAll();

      _command.NotifyClientTransactionOfBegin();
    }

    [Test]
    public void Begin ()
    {
      _mockRepository.ReplayAll ();

      _command.Begin ();
    }

    [Test]
    public void Perform_WithCompleteEndPoints ()
    {
      _endPointMock1.Stub (stub => stub.IsDataComplete).Return (true);
      _endPointMock1.Expect (mock => mock.MarkDataIncomplete ());
      
      _endPointMock2.Stub (stub => stub.IsDataComplete).Return (true);
      _endPointMock2.Expect (mock => mock.MarkDataIncomplete ());
      _mockRepository.ReplayAll ();

      _command.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform_WithIncompleteEndPoints ()
    {
      _endPointMock1.Stub (stub => stub.IsDataComplete).Return (false);
      _endPointMock2.Stub (stub => stub.IsDataComplete).Return (false);
      _mockRepository.ReplayAll ();

      _command.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End ()
    {
      _mockRepository.ReplayAll ();

      _command.End ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      _mockRepository.ReplayAll ();

      _command.NotifyClientTransactionOfEnd ();
    }
  }
}