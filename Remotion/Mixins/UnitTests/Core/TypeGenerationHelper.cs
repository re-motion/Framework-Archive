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
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core
{
  public class TypeGenerationHelper
  {
    public static readonly IPipeline Pipeline = PipelineFactory.Create ("TypeGenerationHelper", new MixinParticipant());

    public static Type ForceTypeGeneration (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetType)
                         ?? new ClassContext (targetType, Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>());

      // Explicitly pass classContext in to the MixinParticipant; that way we generate a mixed type even if there are no mixins on the type.
      return Pipeline.ReflectionService.GetAssembledType (new AssembledTypeID (targetType, new[] { classContext }));
    }

    public static object ForceTypeGenerationAndCreateInstance (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      return Activator.CreateInstance (ForceTypeGeneration (targetType));
    }

    public static T ForceTypeGenerationAndCreateInstance<T> ()
    {
      return (T) ForceTypeGenerationAndCreateInstance (typeof (T));
    }
    
    public static ConcreteMixinType GetGeneratedMixinTypeAndMetadata (ClassContext requestingClass, Type mixinType)
    {
      MixinDefinition mixinDefinition = TargetClassDefinitionFactory.CreateAndValidate (requestingClass)
          .GetMixinByConfiguredType (mixinType);
      Assert.That (mixinDefinition, Is.Not.Null);

      var mixinTypeIdentifier = mixinDefinition.GetConcreteMixinTypeIdentifier();

      var generatedMixinType = Pipeline.ReflectionService.GetAdditionalType (mixinTypeIdentifier);
      return new AttributeBasedMetadataImporter().GetMetadataForMixinType (generatedMixinType);
    }
  }
}