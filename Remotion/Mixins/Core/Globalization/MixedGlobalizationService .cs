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
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Mixins.Globalization
{
  public class MixedGlobalizationService : GlobalizationServiceBase
  {
    public MixedGlobalizationService ()
    {

    }

    protected override IResourceManager GetConcreteResourceManager (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      //foreach mixintype on type
      // get resourceManger + call method with mixintype (GetConcreteResourceManager)
      //create set of all resourceManagers
      //return set;
      // use ClassContext as cache-key



      //TODO AO: implement as decorator: Will use MixedResourceManagerResolver directly. Possibly, also inline MixedResourceManagerResolver?
      return MixedMultiLingualResources.ExistsResource (type) ? MixedMultiLingualResources.GetResourceManager (type, true) : NullResourceManager.Instance;
    }
  }
}