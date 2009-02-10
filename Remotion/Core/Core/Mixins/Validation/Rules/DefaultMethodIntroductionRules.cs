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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultMethodIntroductionRules : RuleSetBase
  {
    private readonly ContextStoreMemberLookupUtility<MethodDefinition> _memberLookupUtility = new ContextStoreMemberLookupUtility<MethodDefinition> ();
    private readonly ContextStoreMemberIntroductionLookupUtility<MethodIntroductionDefinition> _introductionLookupUtility =
        new ContextStoreMemberIntroductionLookupUtility<MethodIntroductionDefinition> ();

    private readonly SignatureChecker _signatureChecker = new SignatureChecker ();

    public override void Install (ValidatingVisitor visitor)
    {
      visitor.MethodIntroductionRules.Add (new DelegateValidationRule<MethodIntroductionDefinition> (PublicMethodNameMustBeUniqueInTargetClass));
      visitor.MethodIntroductionRules.Add (new DelegateValidationRule<MethodIntroductionDefinition> (PublicMethodNameMustBeUniqueInOtherMixins));
    }

    [DelegateRuleDescription (Message = "A method introduced by a mixin cannot be public if the target class already has a method of the same name.")]
    private void PublicMethodNameMustBeUniqueInTargetClass (DelegateValidationRule<MethodIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        MethodInfo introducedMethod = args.Definition.InterfaceMember;
        foreach (MethodDefinition method in _memberLookupUtility.GetCachedMembersByName (args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedMethod.Name))
        {
          if (_signatureChecker.MethodSignaturesMatch (method.MethodInfo, introducedMethod))
          {
            args.Log.Fail (args.Self);
            return;
          }
        }
      }
      args.Log.Succeed (args.Self);
    }

    [DelegateRuleDescription (Message = "A method introduced by a mixin cannot be public if another mixin also introduces a public method of the same name.")]
    private void PublicMethodNameMustBeUniqueInOtherMixins (DelegateValidationRule<MethodIntroductionDefinition>.Args args)
    {
      if (args.Definition.Visibility == MemberVisibility.Public)
      {
        MethodInfo introducedMethod = args.Definition.InterfaceMember;
        IEnumerable<MethodIntroductionDefinition> otherIntroductionsWithSameName =
            _introductionLookupUtility.GetCachedPublicIntroductionsByName (
            args.Log.ContextStore, args.Definition.DeclaringInterface.TargetClass, introducedMethod.Name);

        foreach (MethodIntroductionDefinition method in otherIntroductionsWithSameName)
        {
            if (method != args.Definition && _signatureChecker.MethodSignaturesMatch (method.InterfaceMember, introducedMethod))
            {
              args.Log.Fail (args.Self);
              return;
            }
        }
        args.Log.Succeed (args.Self);
      }
    }
  }
}
