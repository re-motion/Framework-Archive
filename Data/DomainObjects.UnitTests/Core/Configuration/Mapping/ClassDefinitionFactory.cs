/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  public static class ClassDefinitionFactory
  {
    public static ReflectionBasedClassDefinition CreateOrderDefinition()
    {
      return new ReflectionBasedClassDefinition ("Order", "OrderTable", "StorageProviderID", typeof (Order), false,
          new List<Type> ());
    }

    public static ReflectionBasedClassDefinition CreateOrderDefinitionWithResolvedCustomerProperty()
    {
      ReflectionBasedClassDefinition classDefinition = CreateOrderDefinition();
      classDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(classDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", "CustomerID", typeof (ObjectID), false));

      return classDefinition;
    }
  }
}
