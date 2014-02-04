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
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundTypeValidatorTest
  {
    private ITypeValidator _typeValidator1;
    private ITypeValidator _typeValidator2;
    private CompoundTypeValidator _compoundTypeValidator;
    private Type _type;

    [SetUp]
    public void SetUp ()
    {
      _type = typeof (string);

      _typeValidator1 = MockRepository.GenerateStub<ITypeValidator>();
      _typeValidator2 = MockRepository.GenerateStub<ITypeValidator> ();

      _compoundTypeValidator = new CompoundTypeValidator(new []{ _typeValidator1, _typeValidator2 });
    }

    [Test]
    public void IsValid_AllValid ()
    {
      _typeValidator1.Stub (stub => stub.IsValid (_type)).Return(true);
      _typeValidator2.Stub (stub => stub.IsValid (_type)).Return(true);

      var result = _compoundTypeValidator.IsValid (_type);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsValid_NoneValid ()
    {
      _typeValidator1.Stub (stub => stub.IsValid (_type)).Return (false);
      _typeValidator2.Stub (stub => stub.IsValid (_type)).Return (false);

      var result = _compoundTypeValidator.IsValid (_type);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsValid_OneValid ()
    {
      _typeValidator1.Stub (stub => stub.IsValid (_type)).Return (false);
      _typeValidator2.Stub (stub => stub.IsValid (_type)).Return (true);

      var result = _compoundTypeValidator.IsValid (_type);

      Assert.That (result, Is.False);
    }
    
  }
}