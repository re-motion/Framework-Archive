// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.Linq.SqlBackend.SqlPreparation;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Defines an interface for classes needing to customize the re-store LINQ provider. Instances of types 
  /// implementing this interface are retrieved from the <see cref="ServiceLocator"/> by the <see cref="QueryFactory"/> when the first LINQ query is
  /// executed. They are then used to customize the LINQ provider for the rest of the application.
  /// </summary>
  public interface ILinqParserCustomizer
  {
    IEnumerable<Tuple<IEnumerable<MethodInfo>, Type>> GetCustomNodeTypes ();
    IEnumerable<Tuple<IEnumerable<MethodInfo>, IMethodCallTransformer>> GetCustomMethodCallTransformers ();
    IEnumerable<Tuple<IEnumerable<Type>, IResultOperatorHandler>> GetCustomResultOperatorHandlers ();
  }
}