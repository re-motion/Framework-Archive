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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRuleTest : ValidationRuleTestBase
  {
    private MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule();
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Class type of 'Test' is not resolved.")]
    public void ClassDefinitionWithUnresolvedClassType ()
    {
      var type = typeof (BaseMappingAttributesClass);
      var classDefinition = new ReflectionBasedClassDefinitionWithUnresolvedClassType (
          "Test", new StorageEntityDefinitionStub ("Test"), "DefaultStorageProvider", type, true, null, new PersistentMixinFinder (type, false));

      _validationRule.Validate (classDefinition);
    }

    [Test]
    public void OriginalPropertyDeclaration ()
    {
      var type = typeof (BaseMappingAttributesClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);

      var validationResult = _validationRule.Validate (classDefinition).First();

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void NonOriginalPropertiesDeclarationWithMappingAttribute_NoInheritanceRoot ()
    {
      var type = typeof (DerivedClassWithMappingAttribute);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);
      
      var validationResult = _validationRule.Validate (classDefinition).Where(r=>!r.IsValid).ToArray();

      var expectedMessage1 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.DerivedClassWithMappingAttribute\r\n"
        + "Property: Property1";
      var expectedMessage2 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.DerivedClassWithMappingAttribute\r\n"
        + "Property: Property3";
      Assert.That (validationResult.Length, Is.EqualTo (2));
      AssertMappingValidationResult (validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult (validationResult[1], false, expectedMessage2);
    }

    [Test]
    public void NonOriginalPropertiesDeclarationWithMappingAttribute_InheritanceRoot ()
    {
      var type = typeof (InheritanceRootDerivedMappingAttributesClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false);

      var validationResult = _validationRule.Validate (classDefinition).Where (r => !r.IsValid).ToArray ();

      var expectedMessage1 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property1";
      var expectedMessage2 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property3";
      Assert.That (validationResult.Length, Is.EqualTo (2));
      AssertMappingValidationResult (validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult (validationResult[1], false, expectedMessage2);
    }

    [Test]
    [Ignore ("TODO 3424: Utilities.ReflectionUtility.IsOriginalDeclaration does not work for Mixins")]
    public void NonOriginalPropertiesDeclarationWithMappingAttributeOnMixin_NoInheritanceRoot ()
    {
      var type = typeof (ClassUsingMixinPropertiesNoInheritanceRoot);
      var classDefinition = new ReflectionBasedClassDefinition (
          "ClassUsingMixinPropertiesNoInheritanceRoot",
          new StorageEntityDefinitionStub("ClassUsingMixinPropertiesNoInheritanceRoot"),
          "DefaultStorageProvider",
          type,
          false,
          null,
          new PersistentMixinFinder (type, true));
      
      var validationResult = _validationRule.Validate (classDefinition).ToArray();

      var expectedMessage1 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property1";
      var expectedMessage2 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaration type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property3";
      Assert.That (validationResult.Length, Is.EqualTo (2));
      AssertMappingValidationResult (validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult (validationResult[1], false, expectedMessage2);
    }
  }
}