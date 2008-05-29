﻿using System;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security
{
  /// <summary>
  /// Marker interface, used as type parameter for the <see cref="IAdapterRegistryImplementation.SetAdapter"/> and 
  /// <see cref="IAdapterRegistryImplementation.GetAdapter{T}"/> methods of <see cref="AdapterRegistry"/>.
  /// </summary>
  public interface ISecurityAdapter : IAdapter
  {
  }
}