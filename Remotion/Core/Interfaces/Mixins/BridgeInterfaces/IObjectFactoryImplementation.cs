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
using Remotion.Implementation;
using Remotion.Reflection;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Mixins.BridgeImplementations.ObjectFactoryImplementation, Remotion, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>",
    Lifetime = LifetimeKind.Singleton)]
  public interface IObjectFactoryImplementation
  {
    object CreateInstance (bool allowNonPublicConstructors, Type targetOrConcreteType, ParamList constructorParameters, GenerationPolicy generationPolicy, params object[] preparedMixins);
  }
}
