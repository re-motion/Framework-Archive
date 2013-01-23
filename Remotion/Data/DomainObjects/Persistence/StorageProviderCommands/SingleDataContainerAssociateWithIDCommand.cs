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

using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.StorageProviderCommands
{
  /// <summary>
  /// Executes a given <see cref="IStorageProviderCommand{T,TExecutionContext}"/> and associates the result with a given <see cref="IObjectID{DomainObject}"/>, 
  /// checking whether the return value actually matches the expected <see cref="IObjectID{DomainObject}"/>.
  /// </summary>
  public class SingleDataContainerAssociateWithIDCommand<TExecutionContext> : IStorageProviderCommand<ObjectLookupResult<DataContainer>, TExecutionContext>
  {
    private readonly IObjectID<DomainObject> _expectedObjectID;
    private readonly IStorageProviderCommand<DataContainer, TExecutionContext> _innerCommand;

    public SingleDataContainerAssociateWithIDCommand (IObjectID<DomainObject> expectedObjectID, IStorageProviderCommand<DataContainer, TExecutionContext> innerCommand)
    {
      ArgumentUtility.CheckNotNull ("expectedObjectID", expectedObjectID);
      ArgumentUtility.CheckNotNull ("innerCommand", innerCommand);

      _expectedObjectID = expectedObjectID;
      _innerCommand = innerCommand;
    }

    public IObjectID<DomainObject> ExpectedObjectID
    {
      get { return _expectedObjectID; }
    }

    public IStorageProviderCommand<DataContainer, TExecutionContext> InnerCommand
    {
      get { return _innerCommand; }
    }

    public ObjectLookupResult<DataContainer> Execute (TExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      var dataContainer = InnerCommand.Execute (executionContext);
      if (dataContainer != null && !object.Equals (dataContainer.ID, _expectedObjectID))
      {
        var message = string.Format (
            "The ObjectID of the loaded DataContainer '{0}' and the expected ObjectID '{1}' differ.", dataContainer.ID, _expectedObjectID);
        throw new PersistenceException (message);
      }
      return new ObjectLookupResult<DataContainer> (_expectedObjectID, dataContainer);
    }
  }
}