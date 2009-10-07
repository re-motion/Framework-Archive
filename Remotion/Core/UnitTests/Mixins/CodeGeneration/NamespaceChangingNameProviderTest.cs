// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class NamespaceChangingNameProviderTest
  {
    [Test]
    public void GetNameForConcreteMixedType_NormalNameGetsExtendedNamespace ()
    {
      var nameProvider = NamespaceChangingNameProvider.Instance;

      TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      string newName = nameProvider.GetNameForConcreteMixedType (definition);

      Assert.That (newName, Is.EqualTo (typeof (BaseType1).Namespace + ".MixedTypes.BaseType1"));
    }

    [Test]
    public void GetNameForConcreteMixedType_GenericNameGetsExtendedNamespacePlusCharacterReplacements ()
    {
      var nameProvider = NamespaceChangingNameProvider.Instance;

      TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (
          typeof (GenericTargetClass<int>));
      string newName = nameProvider.GetNameForConcreteMixedType (definition);

      Assert.That (newName, 
          Is.EqualTo (typeof (GenericTargetClass<int>).Namespace + 
          ".MixedTypes.GenericTargetClass`1{System_Int32/mscorlib/Version=2_0_0_0/Culture=neutral/PublicKeyToken=b77a5c561934e089}"));
    }
  }
}
