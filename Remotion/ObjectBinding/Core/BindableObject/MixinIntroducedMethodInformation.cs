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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Represents a mixin method that is introduced to its target classes.
  /// </summary>
  /// <remarks>
  /// This is mainly just a wrapper around a <see cref="IMethodInformation"/>
  /// describing the method as it is implemented on the mixin type itself. The <see cref="Invoke"/> and <see cref="GetFastInvoker"/> methods, however,
  /// always call the method via its interface declaration. (Every member introduced by a mixin is declared by an interface implemented by the mixin.)
  /// This means that a <see cref="MixinIntroducedMethodInformation"/> can invoke the method both via the mixin instance and via an instance of the
  /// target class.
  /// </remarks>
  /// <seealso cref="MixinIntroducedPropertyInformation"/>
  public class MixinIntroducedMethodInformation : IMethodInformation
  {
    private readonly InterfaceImplementationMethodInformation _mixinMethodInfo;
    private readonly DoubleCheckedLockingContainer<IMethodInformation> _methodInterfaceDeclarationCache;

    public MixinIntroducedMethodInformation (InterfaceImplementationMethodInformation mixinMethodInfo)
    {
      ArgumentUtility.CheckNotNull ("mixinMethodInfo", mixinMethodInfo);

      _mixinMethodInfo = mixinMethodInfo;
      _methodInterfaceDeclarationCache = new DoubleCheckedLockingContainer<IMethodInformation> (_mixinMethodInfo.FindInterfaceDeclaration);
    }

    public string Name
    {
      get { return _mixinMethodInfo.Name; }
    }

    public Type DeclaringType
    {
      get { return _mixinMethodInfo.DeclaringType; }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _mixinMethodInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _mixinMethodInfo.GetCustomAttribute<T> (inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _mixinMethodInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _mixinMethodInfo.IsDefined<T> (inherited);
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _mixinMethodInfo.FindInterfaceImplementation (implementationType);
    }

    public IMethodInformation FindInterfaceDeclaration ()
    {
      return _methodInterfaceDeclarationCache.Value;
    }

    public T GetFastInvoker<T> () where T: class
    {
      return (T) (object) GetFastInvoker (typeof (T)); 
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      return _mixinMethodInfo.GetFastInvoker (delegateType);
    }

    public ParameterInfo[] GetParameters ()
    {
      return _mixinMethodInfo.GetParameters();
    }

    public IMethodInformation GetBaseDefinition ()
    {
      return _mixinMethodInfo.GetBaseDefinition();
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      return _mixinMethodInfo.FindDeclaringProperty ();
    }

    public Type ReturnType
    {
      get { return _mixinMethodInfo.ReturnType; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _mixinMethodInfo.Invoke (instance, parameters);
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (obj.GetType() != GetType()) return false;
      var other = (MixinIntroducedMethodInformation) obj;

      return _mixinMethodInfo.Equals (other._mixinMethodInfo);
    }

    public override int GetHashCode ()
    {
      return _mixinMethodInfo.GetHashCode();
    }

    public override string ToString ()
    {
      return _mixinMethodInfo + " (Mixin)";
    }
  }
}