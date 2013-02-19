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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Represents a <see cref="EventInfo"/> that can be modified.
  /// </summary>
  public class MutableEventInfo : CustomEventInfo, IMutableMember
  {
    public MutableEventInfo (
        ProxyType declaringType,
        string name,
        EventAttributes attributes,
        MutableMethodInfo addMethod,
        MutableMethodInfo removeMethod,
        MutableMethodInfo raiseMethod)
        : base (declaringType, name, attributes, addMethod, removeMethod, raiseMethod)
    {
    }

    public ProxyType MutableDeclaringType
    {
      get { throw new System.NotImplementedException(); }
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      throw new System.NotImplementedException();
    }

    public ReadOnlyCollection<CustomAttributeDeclaration> AddedCustomAttributes
    {
      get { throw new System.NotImplementedException(); }
    }

    public void AddCustomAttribute (CustomAttributeDeclaration customAttribute)
    {
      throw new System.NotImplementedException();
    }
  }
}