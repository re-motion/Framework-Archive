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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  // TODO Review: Refactor to an IInterceptedPropertyAccessorFinder. Return only objects for overridable accessors. Use "Tell, Don't Ask" and 
  // "Polymorphism Instead of Conditional" refactoring patterns.
  /// <summary>
  /// Retrieves <see cref="IAccessorInterceptor"/>s that can be used to implement or override accessor methods declared by <see cref="DomainObject"/>
  /// derivatives.
  /// </summary>
  [ConcreteImplementation (typeof (InterceptedPropertyCollectorAdapter))]
  public interface IInterceptedPropertyFinder
  {
    // TODO Review: Pass ClassDefinition as argument
    IEnumerable<Tuple<PropertyInfo, string>> GetProperties (Type domainObjectType);

    bool IsOverridable (MethodInfo mostDerivedMethod);
    bool IsAutomaticPropertyAccessor (MethodInfo mostDerivedAccessor);

    IEnumerable<IAccessorInterceptor> GetPropertyInterceptors (ClassDefinition classDefinition, Type concreteBaseType);
  }
}