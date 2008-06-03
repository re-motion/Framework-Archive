/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding
{
  public delegate object BaseMethodInvoker (object[] args);
  public delegate object MethodInvocationHandler (object instance, MethodInfo method, object[] args, BaseMethodInvoker baseMethod);

  public class DynamicMixinBuilder
  {
    public static ModuleScope Scope = new ModuleScope (true);

    private readonly Type _targetType;
    private readonly Set<MethodInfo> _methodsToOverride = new Set<MethodInfo> ();

    public DynamicMixinBuilder (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      _targetType = targetType;
    }

    public Type BuildMixinType (MethodInvocationHandler methodInvocationHandler)
    {
      ArgumentUtility.CheckNotNull ("methodInvocationHandler", methodInvocationHandler);
      return new DynamicMixinTypeGenerator (Scope, _targetType, _methodsToOverride, methodInvocationHandler).BuildType();
    }

    public void OverrideMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      if (method.DeclaringType != _targetType)
        throw new ArgumentException ("The declaring type of the method must be the target type.", "method");

      _methodsToOverride.Add (method);
    }

    public IEnumerable<MethodInfo> OverriddenMethods
    {
      get { return _methodsToOverride; }
    }
  }
}
