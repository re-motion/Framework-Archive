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
using System.Globalization;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.FutureReflection
{
  public class FuturePropertyInfo : PropertyInfo
  {
    private readonly Type _declaringType;
    private readonly MethodInfo _getMethod;
    private readonly MethodInfo _setMethod;

    public FuturePropertyInfo (Type declaringType, MethodInfo getMethod, MethodInfo setMethod)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("getMethod", getMethod);
      ArgumentUtility.CheckNotNull ("setMethod", setMethod);

      _declaringType = declaringType;
      _getMethod = getMethod;
      _setMethod = setMethod;
    }

    public override Type DeclaringType
    {
      get { return _declaringType; }
    }

    public override MethodInfo GetGetMethod (bool nonPublic)
    {
      return _getMethod;
    }

    #region Not Implemented from PropertyInfo interface

    public override object[] GetCustomAttributes (bool inherit)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override object GetValue (object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override void SetValue (object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo[] GetAccessors (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetSetMethod (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override ParameterInfo[] GetIndexParameters ()
    {
      throw new NotImplementedException();
    }

    public override string Name
    {
      get { throw new NotImplementedException(); }
    }

    public override Type ReflectedType
    {
      get { throw new NotImplementedException(); }
    }

    public override Type PropertyType
    {
      get { throw new NotImplementedException(); }
    }

    public override PropertyAttributes Attributes
    {
      get { throw new NotImplementedException(); }
    }

    public override bool CanRead
    {
      get { throw new NotImplementedException(); }
    }

    public override bool CanWrite
    {
      get { throw new NotImplementedException(); }
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}