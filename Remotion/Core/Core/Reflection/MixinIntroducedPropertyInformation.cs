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
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public class MixinIntroducedPropertyInformation : IPropertyInformation
  {
    private readonly IPropertyInformation _mixinPropertyInfo;

    public MixinIntroducedPropertyInformation (IPropertyInformation mixinPropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("mixinPropertyInfo", mixinPropertyInfo);

      _mixinPropertyInfo = mixinPropertyInfo;
    }

    public string Name
    {
      get { return _mixinPropertyInfo.Name;  }
    }

    public Type DeclaringType
    {
      get { return _mixinPropertyInfo.DeclaringType;  }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _mixinPropertyInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _mixinPropertyInfo.GetCustomAttribute<T>(inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _mixinPropertyInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _mixinPropertyInfo.IsDefined<T> (inherited);
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _mixinPropertyInfo.FindInterfaceImplementation (implementationType);
    }

    public IPropertyInformation FindInterfaceDeclaration ()
    {
      return _mixinPropertyInfo.FindInterfaceDeclaration();
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return _mixinPropertyInfo.GetIndexParameters();
    }

    public Type PropertyType
    {
      get { return _mixinPropertyInfo.PropertyType; }
    }

    public bool CanBeSetFromOutside
    {
      get { return _mixinPropertyInfo.CanBeSetFromOutside; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      return GetGetMethod (true).Invoke (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      var parameters = indexParameters!=null ? ArrayUtility.Combine (indexParameters, value) : new[] { value };
      GetSetMethod (true).Invoke(instance, parameters);
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      return Maybe.ForValue (_mixinPropertyInfo.GetGetMethod (nonPublic)).Select (mi => new MixinIntroducedMethodInformation (mi)).ValueOrDefault ();
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      return Maybe.ForValue (_mixinPropertyInfo.GetSetMethod (nonPublic)).Select (mi => new MixinIntroducedMethodInformation(mi)).ValueOrDefault ();
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration();
    }
  }
}