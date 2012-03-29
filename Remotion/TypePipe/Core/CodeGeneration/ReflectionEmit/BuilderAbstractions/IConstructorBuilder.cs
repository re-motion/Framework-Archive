﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.BuilderAbstractions
{
  /// <summary>
  /// Defines an interface for <see cref="ConstructorBuilder"/>.
  /// </summary>
  [CLSCompliant (false)]
  public interface IConstructorBuilder
  {
    void SetBody (LambdaExpression body, IILGeneratorFactory ilGeneratorFactory, DebugInfoGenerator debugInfoGeneratorOrNull);
    void Emit (IILGenerator ilGenerator, OpCode opCode);
    void DefineParameter (int iSequence, ParameterAttributes attributes, string strParamName);
  }
}