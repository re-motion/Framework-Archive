﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Globalization;
using Remotion.Validation.Implementation;
using Remotion.Validation.IntegrationTests.CustomImplementations;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentA;

namespace Remotion.Validation.IntegrationTests
{
  [TestFixture]
  public class ValidationGlobalizationIntegrationTests : IntegrationTestBase
  {
    [Test]
    public void CustomValidatorErrorMessages ()
    {
      var person = new Person();
      person.LastName = "Test";

      var validator = ValidationBuilder.BuildValidator<Person>();

      var result = validator.Validate (person);

      Assert.That (result.IsValid, Is.False);
      Assert.That (result.Errors.Count (), Is.EqualTo (2));
      Assert.That (
          result.Errors.Select (e => e.ErrorMessage),
          Is.EquivalentTo (new[] { "NotNullValidator Fake Message", "NotEqualValidator Fake Message" }));
    }
    
    protected override IErrorMessageGlobalizationService GetValidatorGlobalizationService ()
    {
      return new FakeMessageValidatorGlobalizationService();
    }
  }
}