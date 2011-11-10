// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.Validation.Rules
{
  [TestFixture]
  public class DefaultRequiredMethodRulesTest : ValidationTestBase
  {
    [Test]
    public void FailsIfRequiredBaseMethodIsExplit ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear ().AddMixins (typeof (MixinRequiringAllMembersNextCall)).EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersNextCall));
        var log = Validator.Validate (definition);

        Assert.IsTrue (
            HasFailure ("Remotion.Mixins.Validation.Rules.DefaultRequiredMethodRules.RequiredNextCallMethodMustBePublicOrProtected", log));
      }
    }

    [Test]
    public void SucceedsIfRequiredTargetCallMethodIsExplit ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear ().AddMixins (typeof (MixinRequiringAllMembersTargetCall)).EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.BuildUnvalidatedDefinition (
            typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersTargetCall));
        var log = Validator.Validate (definition);

        AssertSuccess (log);
      }
    }
  }
}
