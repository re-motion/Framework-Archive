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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Tracks <see cref="DataContainer"/> instances that still need to be registered during an ongoing object loading operation. 
  /// This is used by <see cref="FetchEnabledObjectLoader"/> in order to ensure that all fetch requests are performed before any OnLoaded events are 
  /// raised.
  /// </summary>
  public class DataContainersPendingRegistrationCollector
  {
    private readonly Dictionary<ObjectID, DataContainer> _dataContainersPendingRegistration = new Dictionary<ObjectID, DataContainer> ();

    public ReadOnlyCollectionDecorator<DataContainer> DataContainersPendingRegistration
    {
      get { return _dataContainersPendingRegistration.Values.AsReadOnly(); }
    }

    public void AddDataContainers (IEnumerable<DataContainer> pendingDataContainers)
    {
      ArgumentUtility.CheckNotNull ("pendingDataContainers", pendingDataContainers);

      foreach (var pendingDataContainer in pendingDataContainers)
      {
        if (!_dataContainersPendingRegistration.ContainsKey (pendingDataContainer.ID))
          _dataContainersPendingRegistration.Add (pendingDataContainer.ID, pendingDataContainer);
      }
    }
  }
}