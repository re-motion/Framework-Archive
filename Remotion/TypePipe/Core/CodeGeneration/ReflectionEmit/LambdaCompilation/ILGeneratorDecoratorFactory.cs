// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation
{
  /// <summary>
  /// Decorates another <see cref="IILGeneratorFactory"/> in order to create <see cref="ILGeneratorDecorator"/> instances.
  /// </summary>
  [CLSCompliant (false)]
  public class ILGeneratorDecoratorFactory : IILGeneratorFactory
  {
    private readonly IILGeneratorFactory _innerFactory;

    public ILGeneratorDecoratorFactory (IILGeneratorFactory innerFactory)
    {
      ArgumentUtility.CheckNotNull ("innerFactory", innerFactory);
      _innerFactory = innerFactory;
    }

    public IILGeneratorFactory InnerFactory
    {
      get { return _innerFactory; }
    }

    public IILGenerator CreateAdaptedILGenerator (ILGenerator realILGenerator)
    {
      ArgumentUtility.CheckNotNull ("realILGenerator", realILGenerator);

      var innerILGenerator = _innerFactory.CreateAdaptedILGenerator (realILGenerator);
      return new ILGeneratorDecorator (innerILGenerator);
    }
  }
}