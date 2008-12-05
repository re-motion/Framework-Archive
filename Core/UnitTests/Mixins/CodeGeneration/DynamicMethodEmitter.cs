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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public class DynamicMethodEmitter : IMemberEmitter
  {
    private readonly DynamicMethod _dynamicMethod;
    private readonly DynamicMethodCodeBuilder _codeBuilder;

    public DynamicMethodEmitter (DynamicMethod dynamicMethod)
    {
      _dynamicMethod = dynamicMethod;
      _codeBuilder = new DynamicMethodCodeBuilder (dynamicMethod.GetILGenerator ());
    }

    public DynamicMethod DynamicMethod
    {
      get { return _dynamicMethod; }
    }

    public DynamicMethodCodeBuilder CodeBuilder
    {
      get { return _codeBuilder; }
    }

    public void Generate ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_codeBuilder, "Generate", this, _codeBuilder.Generator);
    }

    public void EnsureValidCodeBlock ()
    {
      throw new NotImplementedException();
    }

    public MemberInfo Member
    {
      get { return _dynamicMethod; }
    }

    public Type ReturnType
    {
      get { return _dynamicMethod.ReturnType; }
    }
  }
}
