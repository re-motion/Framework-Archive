using System;
using Rubicon.Mixins.Definitions;
using System.Reflection;
using Rubicon.Mixins.Validation;

namespace Rubicon.Mixins.Validation.Rules
{
  public class DefaultAttributeRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.AttributeRules.Add (new DelegateValidationRule<AttributeDefinition> (AllowMultipleRequired));
    }

    private void AllowMultipleRequired (DelegateValidationRule<AttributeDefinition>.Args args)
    {
      if (!MultipleAllowed (args.Definition.AttributeType))
      {
        BaseClassDefinition baseClass = args.Definition.DeclaringDefinition as BaseClassDefinition;
        if (baseClass != null)
        {
          if (BaseClassHasAttributeDuplicates (args.Definition, baseClass))
          {
            args.Log.Fail (args.Self);
            return;
          }
          else
          {
            args.Log.Succeed (args.Self);
            return;
          }
        }

        MixinDefinition mixin = args.Definition.DeclaringDefinition as MixinDefinition;
        if (mixin != null)
        {
          if (MixinHasAttributeDuplicates (args.Definition, mixin))
          {
            args.Log.Fail (args.Self);
            return;
          }
          else
          {
            args.Log.Succeed (args.Self);
            return;
          }
        }

        MemberDefinition member = args.Definition.DeclaringDefinition as MemberDefinition;
        if (member != null)
        {
          if (MemberHasAttributeDuplicates (args.Definition, member))
          {
            args.Log.Fail (args.Self);
            return;
          }
          else
          {
            args.Log.Succeed (args.Self);
            return;
          }
        }

        throw new NotSupportedException ("Attributable definition is neither base class, mixin, not member.");
      }
      else
      {
        args.Log.Succeed (args.Self);
      }
    }

    private bool BaseClassHasAttributeDuplicates (AttributeDefinition attributeDefinition, BaseClassDefinition baseClass)
    {
      foreach (MixinDefinition mixin in baseClass.Mixins)
      {
        if (mixin.CustomAttributes.ContainsKey (attributeDefinition.AttributeType))
          return true;
      }
      return false;
    }

    private bool MixinHasAttributeDuplicates (AttributeDefinition attributeDefinition, MixinDefinition mixin)
    {
      return mixin.BaseClass.CustomAttributes.ContainsKey (attributeDefinition.AttributeType);
    }

    private bool MemberHasAttributeDuplicates (AttributeDefinition attributeDefinition, MemberDefinition member)
    {
      foreach (MemberDefinition overrider in member.Overrides)
      {
        if (overrider.CustomAttributes.ContainsKey (attributeDefinition.AttributeType))
          return true;
      }
      
      if (member.BaseAsMember != null && member.BaseAsMember.CustomAttributes.ContainsKey (attributeDefinition.AttributeType))
        return true;

      return false;
    }

    private bool MultipleAllowed (Type attributeType)
    {
      AttributeUsageAttribute[] usageAttributes = 
          (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), true);
      return usageAttributes.Length > 0 && usageAttributes[0].AllowMultiple;
    }
  }
}
