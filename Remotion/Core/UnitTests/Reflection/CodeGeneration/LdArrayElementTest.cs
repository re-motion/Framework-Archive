// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.TestDomain;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class LdArrayElementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void LoadArrayElementFromExpression ()
    {
      var method = GetMethodEmitter (false);
      method.SetParameterTypes (new Type[] { typeof (IArrayProvider), typeof (int) });
      method.SetReturnType (typeof (object));
      method.AddStatement (new ILStatement (delegate (IMemberEmitter member, ILGenerator ilgen)
      {
        ilgen.Emit (OpCodes.Ldarg_1); // array provider
        ilgen.Emit (OpCodes.Callvirt, typeof (IArrayProvider).GetMethod ("GetArray")); // array
        ilgen.Emit (OpCodes.Castclass, typeof (object[])); // essentially a nop
        ilgen.Emit (OpCodes.Ldarg_2); // index
        ilgen.Emit (OpCodes.Ldelem, typeof (object));
        ilgen.Emit (OpCodes.Castclass, typeof (object));
        ilgen.Emit (OpCodes.Ret);
      }));

      SimpleArrayProvider provider = new SimpleArrayProvider();
      object result = InvokeMethod (provider, 1);
      Assert.AreEqual (2, result);
    }
  }
}
