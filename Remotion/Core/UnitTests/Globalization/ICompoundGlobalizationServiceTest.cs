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
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class ICompoundGlobalizationServiceTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<ICompoundGlobalizationService> ();

      Assert.That (factory, Is.TypeOf (typeof (CompoundGlobalizationService)));
      var compoundGlobalizationServices = ((CompoundGlobalizationService) factory).GlobalizationServices.ToArray();
      Assert.That (compoundGlobalizationServices[0].GetType().Name, Is.EqualTo("MixinGlobalizationService"));
      Assert.That (compoundGlobalizationServices[1], Is.TypeOf(typeof(GlobalizationService)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<ICompoundGlobalizationService> ();
      var factory2 = _serviceLocator.GetInstance<ICompoundGlobalizationService> ();

      Assert.That (factory1, Is.SameAs (factory2));
    }
  }
}