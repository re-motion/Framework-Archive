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

using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Collections;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  /// <summary>
  /// Allows tests to generate code into an <see cref="TypeBuilder"/> without having to care about defining the 
  /// </summary>
  public class AdHocCodeGenerator
  {
    private readonly AssemblyBuilder _assemblyBuilder;
    private readonly ModuleBuilder _moduleBuilder;
    private readonly string _filename = "AdHocCodeGenerator.dll";

    private int _typeCounter;

    public AdHocCodeGenerator ()
    {
      _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName ("AdHocCodeGenerator"), AssemblyBuilderAccess.RunAndSave);
      _moduleBuilder = _assemblyBuilder.DefineDynamicModule (_filename);
    }

    public AssemblyBuilder AssemblyBuilder
    {
      get { return _assemblyBuilder; }
    }

    public ModuleBuilder ModuleBuilder
    {
      get { return _moduleBuilder; }
    }

    public TypeBuilder CreateType (string typeName = null)
    {
      typeName = typeName ?? ("Test_" + _typeCounter++);

      return _moduleBuilder.DefineType (typeName, TypeAttributes.Public);
    }

    public Tuple<TypeBuilder, MethodBuilder> CreateMethod (
        string typeName = null, string methodName = null, Type returnType = null, Type[] parameterTypes = null, Action<MethodBuilder> action = null)
    {
      methodName = methodName ?? ("Test_" + Guid.NewGuid().ToString().Replace("-", ""));
      returnType = returnType ?? typeof (void);
      parameterTypes = parameterTypes ?? Type.EmptyTypes;

      var typeBuilder = CreateType (typeName);

      var methodBuilder = typeBuilder.DefineMethod (methodName, MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);
      if (action != null)
        action (methodBuilder);

      return Tuple.Create (typeBuilder, methodBuilder);
    }

    public T CreateMethodAndRun<T> (
        string typeName = null, string methodName = null, Action<MethodBuilder> action = null, bool saveOnError = false)
    {
      var returnType = typeof (T);
      var parameterTypes = Type.EmptyTypes;

      var tuple = CreateMethod (typeName, methodName, returnType, parameterTypes, action);
      var actualType = tuple.Item1.CreateType();

      try
      {
        return (T) actualType.InvokeMember (tuple.Item2.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
      }
      catch (Exception)
      {
        if (saveOnError)
          Console.WriteLine (Save());
        throw;
      }
    }

    public string Save ()
    {
      _assemblyBuilder.Save (_filename);
      return _moduleBuilder.FullyQualifiedName;
    }
  }
}