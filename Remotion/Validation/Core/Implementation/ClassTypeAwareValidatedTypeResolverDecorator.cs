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
using Remotion.Utilities;
using Remotion.Validation.Attributes;

namespace Remotion.Validation.Implementation
{
  public class ClassTypeAwareValidatedTypeResolverDecorator : IValidatedTypeResolver
  {
    private readonly IValidatedTypeResolver _resolver;

    public ClassTypeAwareValidatedTypeResolverDecorator (IValidatedTypeResolver resolver)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      
      _resolver = resolver;
    }

    public Type GetValidatedType (Type collectorType)
    {
      ArgumentUtility.CheckNotNull ("collectorType", collectorType);

      var genericType = _resolver.GetValidatedType (collectorType);
      if (collectorType.IsDefined (typeof (ApplyWithClassAttribute), false)) 
      {
        var classType = AttributeUtility.GetCustomAttribute<ApplyWithClassAttribute> (collectorType, false).ClassType;
        CheckGenericTypeAssignableFromDefinedType (collectorType, genericType, classType, typeof (ApplyWithClassAttribute).Name);
        return classType;
      }

      return genericType;
    }

    private void CheckGenericTypeAssignableFromDefinedType (Type collectorType, Type genericType, Type classOrMixinTypee, string attributeName)
    {
      if (!genericType.IsAssignableFrom (classOrMixinTypee))
      {
        throw new InvalidOperationException (
            string.Format (
                "Invalid '{0}'-definition for collector '{1}': type '{2}' is not assignable from '{3}'.",
                attributeName,
                collectorType.FullName,
                genericType.FullName,
                classOrMixinTypee.FullName));
      }
    }
  }
}