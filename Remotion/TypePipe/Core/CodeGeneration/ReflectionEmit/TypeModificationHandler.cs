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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.BuilderAbstractions;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Implements <see cref="ITypeModificationHandler"/> by applying the modifications made to a <see cref="MutableType"/> to a subclass proxy.
  /// Also implements <see cref="IDisposableTypeModificationHandler"/> for cloning unmodified existing constructors and forward declarations of
  /// method and constructor bodies.
  /// </summary>
  public class TypeModificationHandler : IDisposableTypeModificationHandler
  {
    private readonly ITypeBuilder _subclassProxyBuilder;
    private readonly IExpressionPreparer _expressionPreparer;
    private readonly ReflectionToBuilderMap _reflectionToBuilderMap;
    private readonly IILGeneratorFactory _ilGeneratorFactory;
    private readonly DebugInfoGenerator _debugInfoGenerator;
    private readonly List<Action> _disposeActions = new List<Action>();

    private bool _disposed = false;

    [CLSCompliant (false)]
    public TypeModificationHandler (
        ITypeBuilder subclassProxyBuilder,
        IExpressionPreparer expressionPreparer,
        ReflectionToBuilderMap reflectionToBuilderMap,
        IILGeneratorFactory ilGeneratorFactory,
        DebugInfoGenerator debugInfoGeneratorOrNull)
    {
      ArgumentUtility.CheckNotNull ("subclassProxyBuilder", subclassProxyBuilder);
      ArgumentUtility.CheckNotNull ("expressionPreparer", expressionPreparer);
      ArgumentUtility.CheckNotNull ("reflectionToBuilderMap", reflectionToBuilderMap);
      ArgumentUtility.CheckNotNull ("ilGeneratorFactory", ilGeneratorFactory);

      _subclassProxyBuilder = subclassProxyBuilder;
      _expressionPreparer = expressionPreparer;
      _reflectionToBuilderMap = reflectionToBuilderMap;
      _ilGeneratorFactory = ilGeneratorFactory;
      _debugInfoGenerator = debugInfoGeneratorOrNull;
    }

    [CLSCompliant (false)]
    public ITypeBuilder SubclassProxyBuilder
    {
      get { return _subclassProxyBuilder; }
    }

    public IExpressionPreparer ExpressionPreparer
    {
      get { return _expressionPreparer; }
    }

    [CLSCompliant(false)]
    public ReflectionToBuilderMap ReflectionToBuilderMap
    {
      get { return _reflectionToBuilderMap; }
    }

    [CLSCompliant (false)]
    public IILGeneratorFactory ILGeneratorFactory
    {
      get { return _ilGeneratorFactory; }
    }

    public DebugInfoGenerator DebugInfoGenerator
    {
      get { return _debugInfoGenerator; }
    }

    // TODO 4745: EnsureNotDisposed
    public void HandleAddedInterface (Type addedInterface)
    {
      ArgumentUtility.CheckNotNull ("addedInterface", addedInterface);
      _subclassProxyBuilder.AddInterfaceImplementation (addedInterface);
    }

    // TODO 4745: EnsureNotDisposed
    public void HandleAddedField (MutableFieldInfo addedField)
    {
      ArgumentUtility.CheckNotNull ("addedField", addedField);

      var fieldBuilder = _subclassProxyBuilder.DefineField (addedField.Name, addedField.FieldType, addedField.Attributes);
      _reflectionToBuilderMap.AddMapping (addedField, fieldBuilder);

      foreach (var declaration in addedField.AddedCustomAttributeDeclarations)
      {
        var propertyArguments = declaration.NamedArguments.Where (na => na.MemberInfo.MemberType == MemberTypes.Property);
        var fieldArguments = declaration.NamedArguments.Where (na => na.MemberInfo.MemberType == MemberTypes.Field);

        var customAttributeBuilder = new CustomAttributeBuilder (
            declaration.AttributeConstructorInfo, 
            declaration.ConstructorArguments,
            propertyArguments.Select (namedArg => (PropertyInfo) namedArg.MemberInfo).ToArray(),
            propertyArguments.Select (namedArg => namedArg.Value).ToArray(),
            fieldArguments.Select (namedArg => (FieldInfo) namedArg.MemberInfo).ToArray(),
            fieldArguments.Select (namedArg => namedArg.Value).ToArray()
            );

        fieldBuilder.SetCustomAttribute (customAttributeBuilder);
      }
    }

    // TODO 4745: EnsureNotDisposed
    public void HandleAddedConstructor (MutableConstructorInfo addedConstructor)
    {
      ArgumentUtility.CheckNotNull ("addedConstructor", addedConstructor);

      if (!addedConstructor.IsNewConstructor)
        throw new ArgumentException ("The supplied constructor must be a new constructor.", "addedConstructor");

      AddConstructorToSubclassProxy (addedConstructor);
    }

    // TODO 4745: EnsureNotDisposed
    public void HandleModifiedConstructor (MutableConstructorInfo modifiedConstructor)
    {
      ArgumentUtility.CheckNotNull ("modifiedConstructor", modifiedConstructor);

      if (modifiedConstructor.IsNewConstructor || !modifiedConstructor.IsModified)
        throw new ArgumentException ("The supplied constructor must be a modified existing constructor.", "modifiedConstructor");

      AddConstructorToSubclassProxy (modifiedConstructor);
    }

    // TODO 4745: EnsureNotDisposed
    public void HandleUnmodifiedConstructor (MutableConstructorInfo existingConstructor)
    {
      ArgumentUtility.CheckNotNull ("existingConstructor", existingConstructor);

      if (existingConstructor.IsNewConstructor || existingConstructor.IsModified)
        throw new ArgumentException ("The supplied constructor must be an unmodified existing constructor.", "existingConstructor");

      AddConstructorToSubclassProxy (existingConstructor);
    }

    public void Dispose ()
    {
      if (_disposed)
        return;
      _disposed = true;

      foreach (var action in _disposeActions)
        action();
    }

    private void AddConstructorToSubclassProxy (MutableConstructorInfo mutableConstructor)
    {
      var parameterTypes = mutableConstructor.GetParameters ().Select (pe => pe.ParameterType).ToArray ();
      var ctorBuilder = _subclassProxyBuilder.DefineConstructor (mutableConstructor.Attributes, mutableConstructor.CallingConvention, parameterTypes);
      _reflectionToBuilderMap.AddMapping (mutableConstructor, ctorBuilder);

      foreach (var parameterInfo in mutableConstructor.GetParameters ())
        ctorBuilder.DefineParameter (parameterInfo.Position + 1, parameterInfo.Attributes, parameterInfo.Name);

      var body = _expressionPreparer.PrepareConstructorBody (mutableConstructor);
      var bodyLambda = Expression.Lambda (body, mutableConstructor.ParameterExpressions);

      // Bodies need to be generated after all other members have been declared (to allow bodies to reference new members in a circular way).
      _disposeActions.Add (() => ctorBuilder.SetBody (bodyLambda, _ilGeneratorFactory, _debugInfoGenerator));
    }
  }
}