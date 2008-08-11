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
using Remotion.Implementation;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation("Remotion.Mixins.BridgeImplementations.TypeFactoryImplementation, Remotion, Culture = neutral, Version = <version>, PublicKeyToken = <publicKeyToken>")]
  public interface ITypeFactoryImplementation
  {
    Type GetConcreteType (Type targetType, GenerationPolicy generationPolicy);
    void InitializeUnconstructedInstance (object mixinTarget);
  }
}
