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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class MixinDependencyDefinition : DependencyDefinitionBase
  {
    public MixinDependencyDefinition (RequiredMixinTypeDefinition requiredType, MixinDefinition depender, MixinDependencyDefinition aggregator)
      : base (requiredType, depender, aggregator)
    {
    }

    public override ClassDefinitionBase GetImplementer ()
    {
      if (RequiredType.Type.IsInterface)
        return Depender.TargetClass.ReceivedInterfaces.ContainsKey (RequiredType.Type)
            ? Depender.TargetClass.ReceivedInterfaces[RequiredType.Type].Implementer : null;
      else
        return Depender.TargetClass.Mixins[RequiredType.Type];
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public new RequiredMixinTypeDefinition RequiredType
    {
      get { return (RequiredMixinTypeDefinition) base.RequiredType; }
    }
  }
}
