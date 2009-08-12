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
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Holds all information necessary to identify a concrete mixin type generated by <see cref="ConcreteTypeBuilder.GetConcreteMixinType"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Comparing <see cref="ConcreteMixinTypeIdentifier"/> instances requires a comparison of the sets of <see cref="ExternalOverriders"/> and
  /// <see cref="WrappedProtectedMembers"/> and should therefore not be performed in tight loops. Getting the hash code is, however, quite fast, as it
  /// is cached.
  /// </para>
  /// </remarks>
  public sealed class ConcreteMixinTypeIdentifier
  {
    /// <summary>
    /// Deserializes an <see cref="ConcreteMixinTypeIdentifier"/> from the given deserializer.
    /// </summary>
    /// <param name="deserializer">The deserializer to use.</param>
    public static ConcreteMixinTypeIdentifier Deserialize (IConcreteMixinTypeIdentifierDeserializer deserializer)
    {
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);

      var mixinType = deserializer.GetMixinType ();
      var externalOverriders = deserializer.GetExternalOverriders ();
      var wrappedProtectedMembers = deserializer.GetWrappedProtectedMembers (mixinType);

      return new ConcreteMixinTypeIdentifier (mixinType, externalOverriders, wrappedProtectedMembers);
    }

    private readonly Type _mixinType;
    private readonly HashSet<MethodInfo> _externalOverriders;
    private readonly HashSet<MethodInfo> _wrappedProtectedMembers;
    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcreteMixinTypeIdentifier"/> class.
    /// </summary>
    /// <param name="mixinType">The mixin type for which a concrete type was generated.</param>
    /// <param name="externalOverriders">Target class members that override members of the mixin. These are called by the concrete mixin type.</param>
    /// <param name="wrappedProtectedMembers">The protected members of the mixin for which public wrappers are required. These are called by
    /// the mixin's target classes.</param>
    public ConcreteMixinTypeIdentifier (Type mixinType, HashSet<MethodInfo> externalOverriders, HashSet<MethodInfo> wrappedProtectedMembers)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("externalOverriders", externalOverriders);
      ArgumentUtility.CheckNotNull ("wrappedProtectedMembers", wrappedProtectedMembers);

      _mixinType = mixinType;
      _externalOverriders = externalOverriders;
      _wrappedProtectedMembers = wrappedProtectedMembers;

      _cachedHashCode = MixinType.GetHashCode ()
          ^ EqualityUtility.GetXorHashCode (_externalOverriders)
          ^ EqualityUtility.GetXorHashCode (_wrappedProtectedMembers);
    }

    /// <summary>
    /// Gets the mixin type for which a concrete type was generated.
    /// </summary>
    /// <value>The mixin type for which a concrete type was generated.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    /// <summary>
    /// Gets the target class members that override members of the mixin. These are called by the concrete mixin type.
    /// </summary>
    /// <value>Target class members that override members of the mixin. These are called by the concrete mixin type.</value>
    public IEnumerable<MethodInfo> ExternalOverriders
    {
      get { return _externalOverriders; }
    }

    /// <summary>
    /// Gets the protected members of the mixin for which public wrappers are required. These are called by the mixin's target classes.
    /// </summary>
    /// <value>The protected members of the mixin for which public wrappers are required.</value>
    public IEnumerable<MethodInfo> WrappedProtectedMembers
    {
      get { return _wrappedProtectedMembers; }
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to this <see cref="ConcreteMixinTypeIdentifier"/>. Checks all 
    /// properties for equality, ignoring the order of the items in the <see cref="MemberInfo"/> sets.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="ConcreteMixinTypeIdentifier"/>.</param>
    /// <returns>
    /// true if the specified <see cref="T:System.Object"/> is an <see cref="ConcreteMixinTypeIdentifier"/> that corresponds to the same concrete
    /// mixin type; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      var other = obj as ConcreteMixinTypeIdentifier;
      return other != null 
          && other.MixinType == MixinType 
          && other._externalOverriders.SetEquals (_externalOverriders) 
          && other._wrappedProtectedMembers.SetEquals (_wrappedProtectedMembers);
    }

    /// <summary>
    /// Serves as a hash function for this <see cref="ConcreteMixinTypeIdentifier"/>, matching the <see cref="Equals"/> implementation.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="ConcreteMixinTypeIdentifier"/>.
    /// </returns>
    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    /// <summary>
    /// Serializes this object with the specified serializer.
    /// </summary>
    /// <param name="serializer">The serializer to use.</param>
    public void Serialize (IConcreteMixinTypeIdentifierSerializer serializer)
    {
      ArgumentUtility.CheckNotNull ("serializer", serializer);

      serializer.AddMixinType (MixinType);
      serializer.AddExternalOverriders (_externalOverriders);
      serializer.AddWrappedProtectedMembers (_wrappedProtectedMembers);
    }
  }
}