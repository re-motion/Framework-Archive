// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Reflection.MemberSignatures;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Provides useful methods for investigating method overrides.
  /// This is used by <see cref="MutableType"/>.
  /// </summary>
  public class RelatedMethodFinder : IRelatedMethodFinder
  {
    public MethodInfo GetBaseMethod (string name, MethodSignature signature, Type typeToStartSearch)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("signature", signature);
      ArgumentUtility.CheckNotNull ("typeToStartSearch", typeToStartSearch);

      var baseTypeSequence = typeToStartSearch.CreateSequence (t => t.BaseType);

      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
      var allBaseMethods = baseTypeSequence.SelectMany (t => t.GetMethods (bindingFlags));

      return allBaseMethods.FirstOrDefault (m => m.IsVirtual && m.Name == name && MethodSignature.Create (m).Equals (signature));
    }

    // TODO 4818 extract into extension method
    public MethodInfo GetBaseMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      var rootDefinition = method.GetBaseDefinition ();
      if (method.Equals (rootDefinition))
        return null;

      return GetBaseMethod (method.Name, MethodSignature.Create (method), method.DeclaringType.BaseType);
    }
  }
}