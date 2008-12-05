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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultMethodRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfOverriddenMethodNotVirtual ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenBaseMethodAbstract ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (AbstractBaseType), typeof (BT1Mixin1));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.AbstractTargetClassMethodMustNotBeOverridden", log));
    }

    [Test]
    public void FailsIfOverriddenMethodFinal ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassWithFinalMethod), typeof (MixinForFinalMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustNotBeFinal", log));
    }

    [Test]
    public void FailsIfOverriddenPropertyMethodNotVirtual ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenEventMethodNotVirtual ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      DefaultValidationLog log = Validator.Validate (definition.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")]);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfOverriddenMixinMethodNotVirtual ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinWithNonVirtualMethodToBeOverridden));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverriddenMethodMustBeVirtual", log));
    }

    [Test]
    public void FailsIfAbstractMixinMethodHasNoOverride ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithAbstractMembers));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.AbstractMixinMethodMustBeOverridden", log));
    }

    [Test]
    public void FailsIfCrossOverridesOnSameMethods ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinOverridingSameClassMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.NoCircularOverrides", log));
    }

    [Test]
    public void SucceedsIfCrossOverridesNotOnSameMethods ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod),
          typeof (MixinOverridingClassMethod));
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }

    [Test]
    public void FailsIfMixinMethodIsOverriddenWhichHasNoThisProperty ()
    {
      TargetClassDefinition definition = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingSingleMixinMethod), typeof (AbstractMixinWithoutBase));
      DefaultValidationLog log = Validator.Validate (definition);

      Assert.IsTrue (HasFailure ("Remotion.Mixins.Validation.Rules.DefaultMethodRules.OverridingMixinMethodsOnlyPossibleWhenMixinDerivedFromMixinBase", log));
    }

    [Test]
    public void SucceedsIfOverridingMembersAreProtected ()
    {
      TargetClassDefinition definition =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      Assert.IsTrue (definition.Mixins[0].HasProtectedOverriders ());
      DefaultValidationLog log = Validator.Validate (definition);

      AssertSuccess (log);
    }
  }
}
