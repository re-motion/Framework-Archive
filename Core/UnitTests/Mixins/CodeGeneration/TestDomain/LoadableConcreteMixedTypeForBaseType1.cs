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
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.TestDomain
{
  [ConcreteMixedType (typeof (BaseType1), new[] {MixinKind.Used}, new [] {typeof (BT1Mixin1)}, new Type[0], new Type[0])]
  public class LoadableConcreteMixedTypeForBaseType1
  { }
}