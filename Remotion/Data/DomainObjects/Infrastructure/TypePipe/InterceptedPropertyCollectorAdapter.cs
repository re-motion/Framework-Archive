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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Implements <see cref="IInterceptedPropertyFinder"/> by delegating to a new instance of <see cref="InterceptedPropertyCollector"/>.
  /// </summary>
  public class InterceptedPropertyCollectorAdapter : IInterceptedPropertyFinder
  {
    private static readonly IRelatedMethodFinder s_relatedMethodFinder = new RelatedMethodFinder();

    public IEnumerable<Tuple<PropertyInfo, string>> GetProperties (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);

      return new InterceptedPropertyCollector (null, TypeConversionProvider.Current).GetProperties();
    }

    public bool IsOverridable (MethodInfo mostDerivedMethod)
    {
      ArgumentUtility.CheckNotNull ("mostDerivedMethod", mostDerivedMethod);

      return InterceptedPropertyCollector.IsOverridable (mostDerivedMethod);
    }

    public bool IsAutomaticPropertyAccessor (MethodInfo mostDerivedAccessor)
    {
      ArgumentUtility.CheckNotNull ("mostDerivedAccessor", mostDerivedAccessor);

      return InterceptedPropertyCollector.IsAutomaticPropertyAccessor (mostDerivedAccessor);
    }

    public IEnumerable<IAccessorInterceptor> GetPropertyInterceptors (ClassDefinition classDefinition, Type concreteBaseType)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteBaseType", concreteBaseType, typeof (DomainObject));

      var properties = new InterceptedPropertyCollector (classDefinition, TypeConversionProvider.Current).GetProperties();

      var interceptors = new List<IAccessorInterceptor>();
      foreach (var propertyEntry in properties)
      {
        var property = propertyEntry.Item1;
        var propertyName = propertyEntry.Item2;

        var getter = property.GetGetMethod (true);
        var setter = property.GetSetMethod (true);

        AddAccessorInterceptor (interceptors, concreteBaseType, getter, propertyName);
        AddAccessorInterceptor (interceptors, concreteBaseType, setter, propertyName);
      }

      return interceptors;
    }

    private void AddAccessorInterceptor (List<IAccessorInterceptor> interceptors, Type concreteBaseType, MethodInfo accessor, string propertyName)
    {
      if (accessor == null)
        return;

      var mostDerivedAccessor = s_relatedMethodFinder.GetMostDerivedOverride (accessor, concreteBaseType);
      if (!InterceptedPropertyCollector.IsOverridable (mostDerivedAccessor))
        return;

      var interceptor = CreateAccessorInterceptor (mostDerivedAccessor, propertyName);
      interceptors.Add (interceptor);
    }

    private static IAccessorInterceptor CreateAccessorInterceptor (MethodInfo interceptedAccessor, string propertyName)
    {
      return InterceptedPropertyCollector.IsAutomaticPropertyAccessor (interceptedAccessor)
                 ? null
                 : new WrappingAccessorInterceptor (interceptedAccessor, propertyName);
    }
  }
}