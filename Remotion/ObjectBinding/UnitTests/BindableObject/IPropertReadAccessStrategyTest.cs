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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class IPropertReadAccessStrategyTest
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
      var strategy = _serviceLocator.GetInstance<IPropertyReadAccessStrategy>();

      Assert.That (strategy, Is.TypeOf (typeof (CompundPropertyReadAccessStrategy)));
      Assert.That (((CompundPropertyReadAccessStrategy) strategy).PropertyReadAccessStrategies, Is.Empty);
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var strategy1 = _serviceLocator.GetInstance<IPropertyReadAccessStrategy>();
      var strategy2 = _serviceLocator.GetInstance<IPropertyReadAccessStrategy>();

      Assert.That (strategy1, Is.SameAs (strategy2));
    }
  }
}