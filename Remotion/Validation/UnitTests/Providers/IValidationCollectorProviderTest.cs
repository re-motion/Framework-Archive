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
using Remotion.ServiceLocation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class IValidationCollectorProviderTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance ()
    {
      //TOOD AO: change after new IoC features are integrated
      var factory = new AggregatingValidationCollectorProvider (
          InvolvedTypeProvider.Create (
              types => types.OrderBy (t => t.Name),
              SafeServiceLocator.Current.GetInstance<IValidationTypeFilter>()),
          new IValidationCollectorProvider[]
          {
              new ValidationAttributesBasedCollectorProvider(),
              new ApiBasedComponentValidationCollectorProvider (
                  new DiscoveryServiceBasedValidationCollectorReflector (
                  new ClassTypeAwareValidatedTypeResolverDecorator (
                  new GenericTypeAwareValidatedTypeResolverDecorator (SafeServiceLocator.Current.GetInstance<IValidatedTypeResolver>()))))
          });

      Assert.That (factory, Is.Not.Null);
      Assert.That (factory, Is.TypeOf (typeof (AggregatingValidationCollectorProvider)));
      Assert.That (((AggregatingValidationCollectorProvider) factory).InvolvedTypeProvider, Is.TypeOf (typeof (InvolvedTypeProvider)));
      var validationCollectorProviders = ((AggregatingValidationCollectorProvider) factory).ValidationCollectorProviders;
      Assert.That (validationCollectorProviders[0], Is.TypeOf (typeof (ValidationAttributesBasedCollectorProvider)));
      Assert.That (validationCollectorProviders[1], Is.TypeOf (typeof (ApiBasedComponentValidationCollectorProvider)));
      var validationCollectorReflector = ((ApiBasedComponentValidationCollectorProvider) validationCollectorProviders[1]).ValidationCollectorReflector;
      Assert.That (validationCollectorReflector, Is.TypeOf (typeof (DiscoveryServiceBasedValidationCollectorReflector)));
      Assert.That (
          ((DiscoveryServiceBasedValidationCollectorReflector) validationCollectorReflector).ValidatedTypeResolver,
          Is.TypeOf (typeof (ClassTypeAwareValidatedTypeResolverDecorator)));
    }
  }
}