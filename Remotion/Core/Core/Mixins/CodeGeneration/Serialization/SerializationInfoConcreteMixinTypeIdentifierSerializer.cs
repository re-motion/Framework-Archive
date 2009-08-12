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
  public class SerializationInfoConcreteMixinTypeIdentifierSerializer : IConcreteMixinTypeIdentifierSerializer
  {
    private readonly SerializationInfo _serializationInfo;
    private readonly string _key;

    public SerializationInfoConcreteMixinTypeIdentifierSerializer (SerializationInfo serializationInfo, string key)
    {
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      _serializationInfo = serializationInfo;
      _key = key;
    }

    public void AddMixinType (Type mixinType)
    {
      _serializationInfo.AddValue (_key + ".MixinType", mixinType.AssemblyQualifiedName);
    }

    public void AddExternalOverriders (HashSet<MethodInfo> externalOverriders)
    {
      SerializeMethods (_key + ".ExternalOverriders", externalOverriders, true);
    }

    public void AddWrappedProtectedMembers (HashSet<MethodInfo> wrappedProtectedMembers)
    {
      SerializeMethods (_key + ".WrappedProtectedMembers", wrappedProtectedMembers, false);
    }

    private void SerializeMethods (string collectionKey, ICollection<MethodInfo> collection, bool includeDeclaringType)
    {
      _serializationInfo.AddValue (collectionKey + ".Count", collection.Count);

      var index = 0;
      foreach (var methodInfo in collection)
      {
        if (methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition)
          throw new NotSupportedException ("Cannot serialize closed generic methods. This is not supported.");

        if (includeDeclaringType)
          _serializationInfo.AddValue (collectionKey + "[" + index + "].DeclaringType", methodInfo.DeclaringType.AssemblyQualifiedName);

        _serializationInfo.AddValue (collectionKey + "[" + index + "].Name", methodInfo.Name);
        _serializationInfo.AddValue (collectionKey + "[" + index + "].Signature", methodInfo.ToString());

        ++index;
      }
    }
  }
}