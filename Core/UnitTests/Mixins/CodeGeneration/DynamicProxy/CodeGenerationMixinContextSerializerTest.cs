// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationMixinContextSerializerTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntegrationTest()
    {
      var referenceContext = new MixinContext (MixinKind.Extending, typeof (int), MemberVisibility.Private, new[] {typeof (string), typeof (double)});

      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName ();
      var type = module.DefineType ("CodeGenerationMixinContextSerializerTest.IntegrationTest");
      var method =
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (MixinContext), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var serializer = new CodeGenerationMixinContextSerializer (emitter.CodeBuilder);
      referenceContext.Serialize (serializer);

      emitter.CodeBuilder.AddStatement (new ReturnStatement (serializer.GetConstructorInvocationExpression ()));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      object result = compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result, Is.EqualTo (referenceContext));
    }
  }
}
