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
using Remotion.ServiceLocation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class IValidationCollectorMergerTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator ();
    }

    [Test]
    public void GetInstance_Once ()
    {
      //TOOD AO: change after new IoC features are integrated
      var factory = new DiagnosticOutputRuleMergeDecorator (
          SafeServiceLocator.Current.GetInstance<IValidationCollectorMerger>(),
          new FluentValidationValidatorFormatterDecorator (SafeServiceLocator.Current.GetInstance<IValidatorFormatter>()));

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf<DiagnosticOutputRuleMergeDecorator> ());
      Assert.That (((DiagnosticOutputRuleMergeDecorator) factory).ValidationCollectorMerger, Is.TypeOf<OrderPrecedenceValidationCollectorMerger> ());
      Assert.That (((DiagnosticOutputRuleMergeDecorator) factory).ValidatorFormatter, Is.TypeOf<FluentValidationValidatorFormatterDecorator> ());
    }

    [Test]
    public void GetInstance_Twice_ReturnsNotSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<IValidationCollectorMerger> ();
      var factory2 = _serviceLocator.GetInstance<IValidationCollectorMerger> ();

      Assert.That (factory1, Is.Not.SameAs (factory2));
    }
  }
}