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

namespace JetBrains.Annotations
{
  /// <summary>
  /// Indicates that the function is used to notify class type property value is changed.
  /// </summary>
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  sealed partial class NotifyPropertyChangedInvocatorAttribute : Attribute
  {
    public NotifyPropertyChangedInvocatorAttribute () { }
    public NotifyPropertyChangedInvocatorAttribute (string parameterName)
    {
      ParameterName = parameterName;
    }

    public string ParameterName { get; private set; }
  }
}