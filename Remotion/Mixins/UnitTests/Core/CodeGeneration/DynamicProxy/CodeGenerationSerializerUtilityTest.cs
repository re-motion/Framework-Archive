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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.DynamicProxy
{
  [TestFixture]
  [Ignore ("TODO 5370: Delete.")]
  public class CodeGenerationSerializerUtilityTest : CodeGenerationBaseTest
  {
    [Test]
    public void DeclareAndFillArrayLocal()
    {
      var array = new[] { "1", "2", "3" };

      var module = ConcreteTypeBuilderTestHelper.GetModuleManager (SavedTypeBuilder).Scope.ObtainDynamicModuleWithWeakName ();
      var type = module.DefineType ("CodeGenerationSerializerUtilityTest.DeclareAndFillArrayLocal");
      var method = 
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (string[]), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var local = CodeGenerationSerializerUtility.DeclareAndFillArrayLocal (array, emitter.CodeBuilder, i => new ConstReference (i + "a").ToExpression ());
      emitter.CodeBuilder.AddStatement (new ReturnStatement (local));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      object result = compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result, Is.EqualTo (new[] { "1a", "2a", "3a" }));
    }
  }
}
