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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public class MixinInfo
  {
    private readonly MixinDefinition _definition;
    private readonly Type _type;
    private readonly ConcreteMixinType _concreteMixinType;

    public MixinInfo (MixinDefinition definition, Type type, ConcreteMixinType concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("type", type);
      // Concrete mixin type may be null.

      _definition = definition;
      _type = type;
      _concreteMixinType = concreteMixinType;
    }

    public MixinDefinition Definition
    {
      get { return _definition; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public ConcreteMixinType ConcreteMixinTypeOrNull
    {
      get { return _concreteMixinType; }
    }
  }
}