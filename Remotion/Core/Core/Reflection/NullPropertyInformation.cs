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

namespace Remotion.Reflection
{
  /// <summary>
  /// Null-object implementation of <see cref="IMethodInformation"/>.
  /// </summary>
  public class NullPropertyInformation : IPropertyInformation
  {
    public string Name
    {
      get { return null; }
    }

    public Type DeclaringType
    {
      get { return null; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return null;
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return null;
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return new T[]{};
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return false;
    }

    public Type PropertyType
    {
      get { return null; }
    }

    public bool CanBeSetFromOutside
    {
      get { return false; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      return null;
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      return new NullMethodInformation();
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      return new NullMethodInformation ();
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      throw new InvalidOperationException("FindInterfaceImplementation can only be called on inteface properties.");
    }

    public IPropertyInformation FindInterfaceDeclaration ()
    {
      return null;
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return new ParameterInfo[0];
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration ();
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      return obj.GetType() == GetType();
    }

    public override int GetHashCode ()
    {
      return 0;
    }
  }
}