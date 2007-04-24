using System;
using Mixins.Definitions;
using System.Reflection;
using Mixins.Validation;

namespace Mixins.Validation.Rules
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
        if (mixin.CustomAttributes.HasItem (attributeDefinition.AttributeType))
          return true;
      }
      return false;
    }

    private bool MixinHasAttributeDuplicates (AttributeDefinition attributeDefinition, MixinDefinition mixin)
    {
      return mixin.BaseClass.CustomAttributes.HasItem (attributeDefinition.AttributeType);
    }

    private bool MemberHasAttributeDuplicates (AttributeDefinition attributeDefinition, MemberDefinition member)
    {
      foreach (MemberDefinition overrider in member.GetOverridesAsMemberDefinitions ())
      {
        if (overrider.CustomAttributes.HasItem (attributeDefinition.AttributeType))
          return true;
      }
      
      if (member.Base != null && member.Base.CustomAttributes.HasItem (attributeDefinition.AttributeType))
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
