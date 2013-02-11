﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using System.Linq;

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// Represents a property on a constructed type.
  /// </summary>
  public class PropertyOnTypeInstantiation : CustomPropertyInfo
  {
    public PropertyOnTypeInstantiation (
        TypeInstantiation declaringType,
        PropertyInfo property,
        MethodOnTypeInstantiation getMethod,
        MethodOnTypeInstantiation setMethod)
        : base (declaringType, property.Name, typeof (int), 0, getMethod, setMethod, AdaptParameters (declaringType, getMethod))
    {
      foreach (var indexParameter in IndexParameters)
        indexParameter.SetMember (this);
    }

    private static CustomParameterInfo[] AdaptParameters (TypeInstantiation declaringType, MethodOnTypeInstantiation getMethod)
    {
      return getMethod.GetParameters().Select (p => new MemberParameterOnTypeInstantiation (getMethod, declaringType, p))
        .Cast<CustomParameterInfo>().ToArray();
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      throw new NotImplementedException();
    }
  }
}