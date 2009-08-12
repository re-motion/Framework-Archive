// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Serializes instances of <see cref="ConcreteMixinTypeIdentifier"/> into a <see cref="SerializationInfo"/> object. The serialization is
  /// completely flat, using only primitive types, so the returned object is always guaranteed to be complete even in the face of the order of 
  /// deserialization of objects not being deterministic.
  /// </summary>
  public class SerializationInfoConcreteMixinTypeIdentifierDeserializer : IConcreteMixinTypeIdentifierDeserializer
  {
    private readonly SerializationInfo _serializationInfo;
    private readonly string _key;

    public SerializationInfoConcreteMixinTypeIdentifierDeserializer (SerializationInfo serializationInfo, string key)
    {
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      _serializationInfo = serializationInfo;
      _key = key;
    }

    public Type GetMixinType ()
    {
      return Type.GetType (_serializationInfo.GetString (_key + ".MixinType"));
    }

    public HashSet<MethodInfo> GetExternalOverriders ()
    {
      return DeserializeMethods (_key + ".ExternalOverriders", null);
    }

    public HashSet<MethodInfo> GetWrappedProtectedMembers (Type mixinType)
    {
      return DeserializeMethods (_key + ".WrappedProtectedMembers", mixinType);
    }

    private HashSet<MethodInfo> DeserializeMethods (string collectionKey, Type declaringType)
    {
      var methods = new HashSet<MethodInfo> ();
      var count = _serializationInfo.GetInt32 (collectionKey + ".Count");

      for (int i = 0; i < count; ++i)
      {
        var methodDeclaringType = declaringType ?? Type.GetType (_serializationInfo.GetString (collectionKey + "[" + i + "].DeclaringType"));

        var method = (MethodInfo) methodDeclaringType.Module.ResolveMethod (_serializationInfo.GetInt32 (collectionKey + "[" + i + "].MetadataToken"));
        if (methodDeclaringType.IsGenericType)
          method = (MethodInfo) MethodInfo.GetMethodFromHandle (method.MethodHandle, methodDeclaringType.TypeHandle);

        methods.Add (method);
      }
      return methods;
    }
  }
}