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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins
{
  /// <summary>
  /// Contains utility methods to retrieve the types generated by the mixin engine.
  /// </summary>
  public static class CodeGenerationTypeMother
  {
    public static Type GetGeneratedMixinTypeInActiveConfiguration (Type targetType, Type mixinType)
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (targetType);
      return GetGeneratedMixinType (requestingClass, mixinType);
    }

    public static Type GetGeneratedMixinType (Type targetType, Type mixinType)
    {
      var requestingClass = new ClassContext (targetType, mixinType);
      return GetGeneratedMixinType(requestingClass, mixinType);
    }

    public static Type GetGeneratedMixinType (ClassContext requestingClass, Type mixinType)
    {
      ConcreteMixinType concreteMixinType = GetGeneratedMixinTypeAndMetadata(requestingClass, mixinType);
      return concreteMixinType.GeneratedType;
    }

    public static ConcreteMixinType GetGeneratedMixinTypeAndMetadata (Type targetType, Type mixinType)
    {
      var requestingClass = new ClassContext (targetType, mixinType);
      return GetGeneratedMixinTypeAndMetadata (requestingClass, mixinType);
    }

    public static ConcreteMixinType GetGeneratedMixinTypeAndMetadata (ClassContext requestingClass, Type mixinType)
    {
      MixinDefinition mixinDefinition = TargetClassDefinitionFactory
          .CreateTargetClassDefinition (requestingClass)
          .GetMixinByConfiguredType (mixinType);
      Assert.That (mixinDefinition, Is.Not.Null);

      return ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition.GetConcreteMixinTypeIdentifier ());
    }
  }
}