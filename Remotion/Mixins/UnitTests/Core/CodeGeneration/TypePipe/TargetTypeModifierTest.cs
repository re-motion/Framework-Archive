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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class TargetTypeModifierTest
  {
    private TargetTypeModifier _modifier;

    private Type _requestedType;
    private MutableType _targetType;
    private TargetTypeModifierContext _context;

    [SetUp]
    public void SetUp ()
    {
      _modifier = new TargetTypeModifier();

      _requestedType = ReflectionObjectMother.GetSomeSubclassableType();
      _targetType = new MutableTypeFactory().CreateProxy (_requestedType);
      _context = new TargetTypeModifierContext (_targetType);
    }

    [Test]
    public void ImplementInterfaces ()
    {
      Assert.That (_targetType.AddedInterfaces, Is.Empty);
      var ifc = ReflectionObjectMother.GetSomeInterfaceType();

      _modifier.ImplementInterfaces (_context, new[] { ifc });

      Assert.That (_targetType.AddedInterfaces, Is.EqualTo (new[] { ifc }));
    }
  }
}