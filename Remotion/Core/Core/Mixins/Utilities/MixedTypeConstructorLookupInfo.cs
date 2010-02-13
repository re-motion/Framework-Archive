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
using System.Reflection.Emit;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Text;

namespace Remotion.Mixins.Utilities
{
  public class MixedTypeConstructorLookupInfo : ConstructorLookupInfo
  {
    private const BindingFlags c_allowNonPublicBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags c_publicOnlyBindingFlags = BindingFlags.Public | BindingFlags.Instance;
    //private static readonly ConstructorInfo s_scopeCtor = typeof (MixedObjectInstantiationScope).GetConstructor (new Type[] { typeof (object[]) });
    //private static readonly MethodInfo s_scopeDisposeMethod = typeof (MixedObjectInstantiationScope).GetMethod ("Dispose");

    //private static Delegate CreateConstructionDelegateWithPreparedMixins (ConstructorInfo ctor, Type delegateType)
    //{
    //  ParameterInfo[] parameters = ctor.GetParameters ();
    //  Type[] parameterTypes = new Type[parameters.Length + 1];
    //  parameterTypes[0] = typeof (object[]); // mixin instances
    //  for (int i = 0; i < parameters.Length; ++i)
    //    parameterTypes[i + 1] = parameters[i].ParameterType;

    //  Type type = ctor.DeclaringType;
    //  DynamicMethod method = new DynamicMethod ("ConstructorWrapperWithPreparedMixins", type, parameterTypes, type);

    //  ILGenerator ilgen = method.GetILGenerator ();
    //  Label endOfMethod = ilgen.DefineLabel ();

    //  LocalBuilder newInstanceLocal = ilgen.DeclareLocal (type);
    //  LocalBuilder scopeLocal = ilgen.DeclareLocal (type);

    //  // using (new MixedTypeInstantiationScope (preparedMixins))

    //  ilgen.Emit (OpCodes.Ldarg_0); // load preparedMixins
    //  ilgen.Emit (OpCodes.Newobj, s_scopeCtor); // open up scope
    //  ilgen.Emit (OpCodes.Stloc, scopeLocal); // store scope for later

    //  ilgen.BeginExceptionBlock (); // try

    //  for (int i = 1; i < parameterTypes.Length; ++i) // load ctor arguments
    //    ilgen.Emit (OpCodes.Ldarg, i);

    //  ilgen.Emit (OpCodes.Newobj, ctor); // call ctor of mixed instance
    //  ilgen.Emit (OpCodes.Stloc, newInstanceLocal); // store for later
    //  ilgen.Emit (OpCodes.Leave, endOfMethod); // goto end of method (including execution of finally)

    //  ilgen.BeginFinallyBlock (); // finally

    //  ilgen.Emit (OpCodes.Ldloc, scopeLocal); // reload scope
    //  ilgen.Emit (OpCodes.Callvirt, s_scopeDisposeMethod); // dispose of scope
    //  ilgen.Emit (OpCodes.Endfinally); // end finally

    //  ilgen.EndExceptionBlock (); // end of exception block

    //  ilgen.MarkLabel (endOfMethod); // end of method is here

    //  ilgen.Emit (OpCodes.Ldloc, newInstanceLocal); // reload constructed instance
    //  ilgen.Emit (OpCodes.Ret); // and return

    //  return method.CreateDelegate (delegateType);
    //}

    private readonly Type _targetType;
    private readonly bool _allowNonPublic;

    public MixedTypeConstructorLookupInfo (Type concreteType, Type targetType, bool allowNonPublic)
      : base (concreteType, c_allowNonPublicBindingFlags)
    {
      _targetType = targetType;
      _allowNonPublic = allowNonPublic;
    }

    protected override ConstructorInfo GetConstructor (Type[] parameterTypes)
    {
      ConstructorInfo targetTypeCtor = _targetType.GetConstructor (BindingFlags, null, CallingConventions.Any, parameterTypes, null);

      if (targetTypeCtor == null)
      {
        string message = string.Format ("Type {0} does not contain a constructor with the following signature: ({1}).",
            _targetType.FullName, SeparatedStringBuilder.Build (",", parameterTypes, t => t.FullName));
        throw new MissingMethodException (message);
      }
      else if (!targetTypeCtor.IsPublic && !_allowNonPublic)
      {
        string message = string.Format (
            "Type {0} contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is not set).",
            _targetType.FullName);
        throw new MissingMethodException (message);
      }
      else
        return base.GetConstructor (parameterTypes);
    }

    //// This method reimplements parts of ConstructorWrapper because we need a delegate that interpretes the first argument passed to it
    //// as the preallocated mixin instances.
    //protected override Delegate CreateDelegate (Type delegateType)
    //{
    //  Type[] realArgumentTypes = GetRealArgumentTypes(delegateType);

    //  ConstructorInfo ctor = _concreteType.GetConstructor (BindingFlags, null, CallingConventions.Any, realArgumentTypes, null);
    //  ConstructorInfo targetTypeCtor = _targetType.GetConstructor (BindingFlags, null, CallingConventions.Any, realArgumentTypes, null);
      
    //  if (targetTypeCtor == null || (!targetTypeCtor.IsPublic && !_allowNonPublic))
    //  {
    //    string message = string.Format (
    //        "Type {0} does not contain a constructor with signature ({1}) (allowNonPublic: {2}).", 
    //        _targetType.FullName, SeparatedStringBuilder.Build (",", realArgumentTypes, delegate (Type t) { return t.FullName; }), _allowNonPublic);
    //    throw new MissingMethodException (message);
    //  }
    //  else if (ctor == null)
    //  {
    //    string message = string.Format (
    //        "Concrete type {0} does not contain a constructor with signature ({1}) (although target type '{2}' does).", _targetType.FullName,
    //        SeparatedStringBuilder.Build (",", realArgumentTypes, delegate (Type t) { return t.FullName; }), _targetType.Name);
    //    throw new MissingMethodException (message);
    //  }

    //  return CreateConstructionDelegateWithPreparedMixins (ctor, delegateType);
    //}

    //private Type[] GetRealArgumentTypes (Type delegateType)
    //{
    //  Type[] argumentTypes = GetParameterTypes (delegateType);
    //  if (argumentTypes.Length == 0 || argumentTypes[0] != typeof (object[]))
    //  {
    //    string message = "The delegate type must have at least one argument, which must be of type object[]. This argument will be used to pass " 
    //        + "pre-instantiated mixins to the instance creator.";
    //    throw new ArgumentException (message, "delegateType");
    //  }

    //  Type[] realArgumentTypes = new Type[argumentTypes.Length - 1];
    //  Array.Copy (argumentTypes, 1, realArgumentTypes, 0, realArgumentTypes.Length); // remove first argument, those are the mixin instances
    //  return realArgumentTypes;
    //}

    protected override object GetCacheKey (Type delegateType)
    {
      return Tuple.Create (_allowNonPublic, _targetType, base.GetCacheKey (delegateType));
    }
  }
}
