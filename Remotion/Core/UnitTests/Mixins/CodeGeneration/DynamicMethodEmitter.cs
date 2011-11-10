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
