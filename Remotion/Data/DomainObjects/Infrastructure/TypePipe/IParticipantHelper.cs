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
  /// An interface that adds an level of abstraction for configuration singletons and static helper functiones needed by 
  /// <see cref="InterceptedDomainObjectParticipant"/>.
  /// </summary>
  public interface IParticipantHelper
  {
    Type GetPublicDomainObjectType (Type concreteType);
    ClassDefinition GetTypeDefinition (Type domainObjectType);

    IEnumerable<Tuple<PropertyInfo, string>> GetInterceptedProperties (Type domainObjectType);

    MethodInfo GetMostDerivedMethodOverride (MethodInfo method, Type typeToStartsearch);
  }

  public class ParticipantHelper : IParticipantHelper
  {
    private static readonly IRelatedMethodFinder s_relatedMethodFinder = new RelatedMethodFinder();

    public Type GetPublicDomainObjectType (Type concreteType)
    {
      throw new NotImplementedException();
    }

    public ClassDefinition GetTypeDefinition (Type domainObjectType)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (domainObjectType);
      if (classDefinition.IsAbstract)
      {
        var message = string.Format (
            "Cannot instantiate type {0} as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
            classDefinition.ClassType.FullName);
        throw new NonInterceptableTypeException (message, classDefinition.ClassType);
      }

      return classDefinition;
    }

    public IEnumerable<Tuple<PropertyInfo, string>> GetInterceptedProperties (Type domainObjectType)
    {
      ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);

      return new InterceptedPropertyCollector (domainObjectType, TypeConversionProvider.Current).GetProperties();
    }

    public MethodInfo GetMostDerivedMethodOverride (MethodInfo method, Type typeToStartsearch)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("typeToStartsearch", typeToStartsearch);

      return s_relatedMethodFinder.GetMostDerivedOverride (method.GetBaseDefinition(), typeToStartsearch);
    }
  }
}