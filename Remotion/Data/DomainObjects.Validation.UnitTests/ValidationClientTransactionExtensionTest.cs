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
using Remotion.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class ValidationClientTransactionExtensionTest
  {
    private IValidatorBuilder _validatorBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _validatorBuilderMock = MockRepository.GenerateStrictMock<IValidatorBuilder>();
    }

    [Test]
    public void DefaultKey ()
    {
      Assert.That (ValidationClientTransactionExtension.DefaultKey, Is.EqualTo (typeof (ValidationClientTransactionExtension).FullName));
    }

    [Test]
    public void Key ()
    {
      var extension = new ValidationClientTransactionExtension (_validatorBuilderMock);
      Assert.That (extension.Key, Is.EqualTo (ValidationClientTransactionExtension.DefaultKey));
    }

    //[Test]
    //public void CommitValidate ()
    //{
    //  var data1 = PersistableDataObjectMother.Create ();
    //  var data2 = PersistableDataObjectMother.Create ();

    //  var transaction = ClientTransaction.CreateRootTransaction ();

    //  var validatorMock = MockRepository.GenerateStrictMock<IPersistableDataValidator> ();
    //  var extension = new CommitValidationClientTransactionExtension (
    //      tx =>
    //      {
    //        Assert.That (tx, Is.SameAs (transaction));
    //        return validatorMock;
    //      });

    //  validatorMock.Expect (mock => mock.Validate (data1));
    //  validatorMock.Expect (mock => mock.Validate (data2));
    //  validatorMock.Replay ();

    //  extension.CommitValidate (transaction, Array.AsReadOnly (new[] { data1, data2 }));

    //  validatorMock.VerifyAllExpectations ();
    //}
  }
}