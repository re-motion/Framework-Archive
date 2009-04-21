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
using System.Reflection;
using Remotion.Collections;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class MixAnalyzer : RelationAnalyzerBase
  {
    private readonly Set<MixAttribute> _handledBindings = new Set<MixAttribute>();

    public MixAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (ICustomAttributeProvider assembly)
    {
      foreach (MixAttribute attribute in assembly.GetCustomAttributes (typeof (MixAttribute), false))
      {
        if (!_handledBindings.Contains (attribute))
        {
          AnalyzeMixAttribute (attribute);
          _handledBindings.Add (attribute);
        }
      }
    }

    public virtual void AnalyzeMixAttribute (MixAttribute mixAttribute)
    {
      AddMixinAndAdjustException (mixAttribute.MixinKind, mixAttribute.TargetType, mixAttribute.MixinType, mixAttribute.IntroducedMemberVisibility, mixAttribute.AdditionalDependencies, mixAttribute.SuppressedMixins);
    }
  }
}
