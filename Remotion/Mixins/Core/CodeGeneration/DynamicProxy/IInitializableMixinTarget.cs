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
namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  /// <summary>
  /// This interface is dynamically added to concrete mixed types generated by <see cref="TypeGenerator"/>. It is used to initialize a mixin
  /// after its construction or deserialization.
  /// </summary>
  // TODO 5370: Remove this interface; this has been replaces by IInitializableObject. In the tests call the method via Reflection instead.
  public interface IInitializableMixinTarget : IMixinTarget
  {
    void Initialize ();
    // TODO 5370: Remove method and implementation.
    void InitializeAfterDeserialization (object[] mixinInstances);
  }
}
