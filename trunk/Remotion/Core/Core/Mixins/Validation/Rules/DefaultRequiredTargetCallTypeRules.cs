// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultRequiredTargetCallTypeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.RequiredTargetCallTypeRules.Add (new DelegateValidationRule<RequiredTargetCallTypeDefinition> (FaceClassMustBeAssignableFromTargetType));
      visitor.RequiredTargetCallTypeRules.Add (new DelegateValidationRule<RequiredTargetCallTypeDefinition> (RequiredTargetCallTypeMustBePublic));
    }

    [DelegateRuleDescription (Message = "A class specified as the TTarget type parameter of a mixin is not assignable from the target type.")]
    private void FaceClassMustBeAssignableFromTargetType (DelegateValidationRule<RequiredTargetCallTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsClass ? args.Definition.Type.IsAssignableFrom (args.Definition.TargetClass.Type) : true, args.Log, args.Self);
    }

    [DelegateRuleDescription (Message = "A type specified as the TTarget type parameter of a mixin does not have public visibility.")]
    private void RequiredTargetCallTypeMustBePublic (DelegateValidationRule<RequiredTargetCallTypeDefinition>.Args args)
    {
      SingleMust (args.Definition.Type.IsVisible, args.Log, args.Self);
    }
  }
}
