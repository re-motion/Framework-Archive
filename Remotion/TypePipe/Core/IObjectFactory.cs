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
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.TypePipe
{
  /// <summary>
  /// The main entry point of the pipeline.
  /// This class is used by application developers to create instances for the types generated by the pipeline.
  /// </summary>
  [ConcreteImplementation (typeof (ObjectFactory))]
  public interface IObjectFactory
  {
    T CreateInstance<T> (bool allowNonPublicConstructor = false)
        where T : class;

    T CreateInstance<T> (ParamList constructorArguments, bool allowNonPublicConstructor = false)
        where T : class;

    object CreateInstance (Type requestedType, bool allowNonPublicConstructor = false);
    object CreateInstance (Type requestedType, ParamList constructorArguments, bool allowNonPublicConstructor = false);
  }
}