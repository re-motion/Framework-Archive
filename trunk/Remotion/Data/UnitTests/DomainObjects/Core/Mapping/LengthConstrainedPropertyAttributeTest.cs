// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class LengthConstrainedPropertyAttributeTest
  {
    private class StubLengthConstrainedPropertyAttribute : NullableLengthConstrainedPropertyAttribute
    {
      public StubLengthConstrainedPropertyAttribute ()
      {
      }
    }

    private NullableLengthConstrainedPropertyAttribute _attribute;
    private ILengthConstrainedPropertyAttribute _lengthConstraint;

    [SetUp]
    public void SetUp()
    {
      _attribute = new StubLengthConstrainedPropertyAttribute ();
      _lengthConstraint = _attribute;
    }

    [Test]
    public void GetMaximumLength_FromDefault()
    {
      Assert.IsFalse (_attribute.HasMaximumLength);
      Assert.IsNull (_lengthConstraint.MaximumLength);
    }

    [Test]
    public void GetMaximumLength_FromExplicitValue()
    {
      _attribute.MaximumLength = 100;
      Assert.IsTrue (_attribute.HasMaximumLength);
      Assert.AreEqual (100, _attribute.MaximumLength);
      Assert.AreEqual (100, _lengthConstraint.MaximumLength);
    }

    [Test]
    [ExpectedException(typeof (InvalidOperationException))]
    public void GetMaximumLength_FromDefault_WithInvalidOperationException ()
    {
      Dev.Null = _attribute.MaximumLength;
    }
  }
}
