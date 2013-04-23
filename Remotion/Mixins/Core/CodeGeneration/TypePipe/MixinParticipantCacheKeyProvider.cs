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
using Remotion.TypePipe.Caching;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370.
  public class MixinParticipantCacheKeyProvider : ICacheKeyProvider
  {
    public object GetCacheKey (Type requestedType)
    {
      // Using Debug.Assert because it will be compiled away.
      Debug.Assert (requestedType != null);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (requestedType);
      // Can this be null with the new feature that avoids 'uninteresting' types.
      // TODO Review
      Assertion.IsNotNull (classContext, "TODO Review");

      return classContext;
    }

    public object RebuildCacheKey (Type requestedType, Type assembledType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);
      ArgumentUtility.CheckNotNull ("assembledType", assembledType);

      var classContext = MixinTypeUtility.GetClassContextForConcreteType (assembledType);
      Debug.Assert (classContext != null);

      return classContext;
    }
  }
}