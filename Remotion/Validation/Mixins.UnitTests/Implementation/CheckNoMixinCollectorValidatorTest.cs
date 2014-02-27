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
using NUnit.Framework;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class CheckNoMixinCollectorValidatorTest
  {
    private CheckNoMixinCollectorValidator _validator;
    private IComponentValidationCollector _collectorStub;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub = MockRepository.GenerateStub<IComponentValidationCollector>();

      _validator = new CheckNoMixinCollectorValidator();
    }

    [Test]
    public void IsValid_MixinType ()
    {
      _collectorStub.Stub (stub => stub.ValidatedType).Return (typeof (CustomerMixin));

      var result = _validator.IsValid (_collectorStub);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsValid_NoMixinType ()
    {
      _collectorStub.Stub (stub => stub.ValidatedType).Return (typeof (Customer));

      var result = _validator.IsValid (_collectorStub);

      Assert.That (result, Is.True);
    }
  }
}