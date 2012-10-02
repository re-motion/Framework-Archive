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
using System.Reflection.Emit;
using Microsoft.Scripting.Ast.Compiler;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation
{
  /// <summary>
  /// Defines a common interface for <see cref="MethodBuilder"/> and <see cref="ConstructorBuilder"/> to be used by <see cref="LambdaCompiler"/>.
  /// </summary>
  /// <remarks>
  /// This class is internal because it should only be used from <see cref="TypeModifier"/> and <see cref="SubclassProxyBuilder"/>.
  /// </remarks>
  internal interface IMethodBuilderForLambdaCompiler
  {
    Type DeclaringType { get; }

    void SetReturnType (Type returnType);
    void SetParameters (Type[] parameterType);
    void DefineParameter (int position, ParameterAttributes parameterAttributes, string parameterName);

    IILGenerator GetILGenerator ();

    MethodBase AsMethodBase ();
  }
}