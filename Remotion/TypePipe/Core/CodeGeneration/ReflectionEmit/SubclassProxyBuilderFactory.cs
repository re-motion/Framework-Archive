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
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Creates <see cref="SubclassProxyBuilder"/> instances.
  /// </summary>
  public class SubclassProxyBuilderFactory : ISubclassProxyBuilderFactory
  {
    private readonly IModuleBuilder _moduleBuilder;
    private readonly DebugInfoGenerator _debugInfoGenerator;

    [CLSCompliant (false)]
    public SubclassProxyBuilderFactory (IModuleBuilder moduleBuilder, DebugInfoGenerator debugInfoGeneratorOrNull)
    {
      ArgumentUtility.CheckNotNull ("moduleBuilder", moduleBuilder);

      _moduleBuilder = moduleBuilder;
      _debugInfoGenerator = debugInfoGeneratorOrNull;
    }

    [CLSCompliant (false)]
    public IModuleBuilder ModuleBuilder
    {
      get { return _moduleBuilder; }
    }

    public DebugInfoGenerator DebugInfoGenerator
    {
      get { return _debugInfoGenerator; }
    }

    public ISubclassProxyBuilder CreateBuilder (MutableType mutableType)
    {
      // TODO 5119: Is non-lazy really what we want?
      // http://csharpindepth.com/Articles/General/Beforefieldinit.aspx

      var typeAttributes = TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
      if (mutableType.IsAbstract)
        typeAttributes |= TypeAttributes.Abstract;

      var typeBuilder = _moduleBuilder.DefineType (mutableType.FullName, typeAttributes, mutableType.UnderlyingSystemType);

      var emittableOperandProvider = new EmittableOperandProvider ();
      typeBuilder.RegisterWith (emittableOperandProvider, mutableType);

      var ilGeneratorFactory = new ILGeneratorDecoratorFactory (new OffsetTrackingILGeneratorFactory (), emittableOperandProvider);
      var memberEmitter = new MemberEmitter (new ExpandingExpressionPreparer(), ilGeneratorFactory);

      var methodTrampolineProvider = new MethodTrampolineProvider (memberEmitter);

      return new SubclassProxyBuilder (
          mutableType, typeBuilder, _debugInfoGenerator, emittableOperandProvider, methodTrampolineProvider, memberEmitter);
    }
  }
}