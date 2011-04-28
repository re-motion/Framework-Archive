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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Decorates an <see cref="IDataManagementCommand"/> and adds functionality for registering/unregistering with the opposite of a 
  /// <see cref="RealObjectEndPoint"/>.
  /// </summary>
  public class RealObjectEndPointRegistrationCommandDecorator : IDataManagementCommand
  {
    private readonly IDataManagementCommand _decoratedCommand;
    private readonly IRealObjectEndPoint _realObjectEndPoint;
    private readonly IVirtualEndPoint _oldRelatedEndPoint;
    private readonly IVirtualEndPoint _newRelatedEndPoint;

    public RealObjectEndPointRegistrationCommandDecorator (
        IDataManagementCommand decoratedCommand,
        IRealObjectEndPoint realObjectEndPoint,
        IVirtualEndPoint oldRelatedEndPoint,
        IVirtualEndPoint newRelatedEndPoint)
    {
      ArgumentUtility.CheckNotNull ("decoratedCommand", decoratedCommand);
      ArgumentUtility.CheckNotNull ("realObjectEndPoint", realObjectEndPoint);
      ArgumentUtility.CheckNotNull ("oldRelatedEndPoint", oldRelatedEndPoint);
      ArgumentUtility.CheckNotNull ("newRelatedEndPoint", newRelatedEndPoint);

      _decoratedCommand = decoratedCommand;
      _realObjectEndPoint = realObjectEndPoint;
      _oldRelatedEndPoint = oldRelatedEndPoint;
      _newRelatedEndPoint = newRelatedEndPoint;
    }

    public IDataManagementCommand DecoratedCommand
    {
      get { return _decoratedCommand; }
    }

    public IRealObjectEndPoint RealObjectEndPoint
    {
      get { return _realObjectEndPoint; }
    }

    public IVirtualEndPoint OldRelatedEndPoint
    {
      get { return _oldRelatedEndPoint; }
    }

    public IVirtualEndPoint NewRelatedEndPoint
    {
      get { return _newRelatedEndPoint; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _decoratedCommand.GetAllExceptions();
    }

    public void NotifyClientTransactionOfBegin ()
    {
      _decoratedCommand.NotifyClientTransactionOfBegin();
    }

    public void Begin ()
    {
      _decoratedCommand.Begin();
    }

    public void Perform ()
    {
      this.EnsureCanExecute();

      _oldRelatedEndPoint.UnregisterCurrentOppositeEndPoint (_realObjectEndPoint);
      _decoratedCommand.Perform();
      _newRelatedEndPoint.RegisterCurrentOppositeEndPoint (_realObjectEndPoint);
    }

    public void End ()
    {
      _decoratedCommand.End ();
    }

    public void NotifyClientTransactionOfEnd ()
    {
      _decoratedCommand.NotifyClientTransactionOfEnd ();
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var innerExpandedCommand = _decoratedCommand.ExpandToAllRelatedObjects();
      var decoratedExpandedCommand = new RealObjectEndPointRegistrationCommandDecorator (
          innerExpandedCommand, 
          _realObjectEndPoint, 
          _oldRelatedEndPoint, 
          _newRelatedEndPoint);
      return new ExpandedCommand (decoratedExpandedCommand);
    }
  }
}