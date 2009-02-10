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
using System.Collections;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Mixins.Globalization
{
  public class MixedResourceManagerResolver<TAttribute> : ResourceManagerResolver<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    public override IResourceManager GetResourceManager (Type objectType, bool includeHierarchy, out Type definingType)
    {
      if (MixinTypeUtility.IsGeneratedConcreteMixedType (objectType))
        objectType = MixinTypeUtility.GetUnderlyingTargetType (objectType);

      return base.GetResourceManager (objectType, includeHierarchy, out definingType);
    }

    protected override object GetResourceManagerSetCacheKey (Type definingType, bool includeHierarchy)
    {
      return Tuple.NewTuple (
          base.GetResourceManagerSetCacheKey (definingType, includeHierarchy),
          TargetClassDefinitionUtility.GetContext (definingType, MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured));
    }

    protected override ResourceDefinition<TAttribute> GetResourceDefinition (Type type, Type currentType)
    {
			ResourceDefinition<TAttribute> resourcesOnType = base.GetResourceDefinition (type, currentType);
			if (type == currentType)
			{
				// only on first call, check mixins
				AddSupplementingAttributesFromMixins (currentType, resourcesOnType);
			}
      return resourcesOnType;
    }

    private void AddSupplementingAttributesFromMixins (Type type, ResourceDefinition<TAttribute> resourcesOnType)
    {
      TargetClassDefinition mixinConfiguration = TargetClassDefinitionUtility.GetActiveConfiguration (type);
      if (mixinConfiguration != null)
      {
        foreach (MixinDefinition mixinDefinition in mixinConfiguration.Mixins)
          AddSupplementingAttiributesFromMixin (mixinDefinition, resourcesOnType, true);
      }
    }

    private void AddSupplementingAttiributesFromMixin (MixinDefinition mixinDefinition, ResourceDefinition<TAttribute> resourcesOnType, bool isHierarchyIncluded)
    {
      IEnumerable<ResourceDefinition<TAttribute>> resourcesOnMixin = GetResourceDefinitionStream (mixinDefinition.Type, isHierarchyIncluded);
      foreach (ResourceDefinition<TAttribute> resourceOnMixin in resourcesOnMixin)
        resourcesOnType.AddSupplementingAttributes (resourceOnMixin.GetAllAttributePairs());
    }
  }
}
