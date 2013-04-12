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
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using System.Linq;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class TargetTypeModifierTest
  {
    private TargetTypeModifier _modifier;

    private Type _requestedType;
    private MutableType _targetType;
    private TargetTypeModifierContext _ctx;

    [SetUp]
    public void SetUp ()
    {
      _modifier = new TargetTypeModifier();

      _requestedType = ReflectionObjectMother.GetSomeSubclassableType();
      _targetType = new MutableTypeFactory().CreateProxy (_requestedType);
      _ctx = new TargetTypeModifierContext (_targetType);
    }

    [Test]
    public void ImplementInterfaces ()
    {
      var ifc = ReflectionObjectMother.GetSomeInterfaceType();

      _modifier.ImplementInterfaces (_ctx, new[] { ifc });

      Assert.That (_targetType.AddedInterfaces, Is.EqualTo (new[] { ifc }));
    }

    [Test]
    public void AddFields ()
    {
      var nextCallProxyType = ReflectionObjectMother.GetSomeType();

      _modifier.AddFields (_ctx, nextCallProxyType);

      var privateStaticAttributes = FieldAttributes.Private | FieldAttributes.Static;
      CheckField (_ctx.ClassContextField, "__classContext", typeof (ClassContext), privateStaticAttributes);
      CheckField (_ctx.MixinArrayInitializerField, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStaticAttributes);
      CheckField (_ctx.ExtensionsField, "__extensions", typeof (object[]), FieldAttributes.Private);
      CheckField (_ctx.FirstField, "__first", nextCallProxyType, FieldAttributes.Private);

      var expctedFields = new[] { _ctx.ClassContextField, _ctx.MixinArrayInitializerField, _ctx.ExtensionsField, _ctx.FirstField };
      Assert.That (_targetType.AddedFields, Is.EqualTo (expctedFields));
    }

    private void CheckField (MutableFieldInfo field, string name, Type type, FieldAttributes attributes)
    {
      Assert.That (field.MutableDeclaringType, Is.SameAs (_targetType));
      Assert.That (field.Name, Is.EqualTo (name));
      Assert.That (field.FieldType, Is.SameAs (type));
      Assert.That (field.Attributes, Is.EqualTo (attributes));
      Assert.That (field.AddedCustomAttributes, Has.Count.EqualTo (1));

      var debuggerBrowsableAttribute = field.AddedCustomAttributes.Single();
      Assert.That (debuggerBrowsableAttribute.Type, Is.SameAs (typeof (DebuggerBrowsableAttribute)));
      Assert.That (debuggerBrowsableAttribute.ConstructorArguments, Is.EqualTo (new[] { DebuggerBrowsableState.Never }));
      Assert.That (debuggerBrowsableAttribute.NamedArguments, Is.Empty);
    }
  }
}