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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  /// <summary>
  /// Implements the <see cref="IPermissionProvider"/> for a reflection-based security declaration.
  /// </summary>
  public class PermissionReflector : ExtendedProviderBase, IPermissionProvider
  {
    private const string c_memberNotFoundMessage = "The member '{0}' could not be found.";
    private const string c_memberHasMultipleAttributesMessage = "The member '{0}' has multiple {1} defined.";

    private const string c_memberPermissionsOnlyInBaseClassMessage =
        "The {2} must not be defined on members overriden or redefined in derived classes. A member '{0}' exists in class '{1}' and its base class.";

    private class CacheKey : IEquatable<CacheKey>
    {
      private readonly Type _attributeType;
      private readonly Type _type;
      private readonly string _memberName;
      private readonly BindingFlags _bindingFlags;

      public CacheKey (Type attributeType, Type type, string memberName, BindingFlags bindingFlags)
      {
        Assertion.DebugAssert (attributeType != null, "Parameter 'attributeType' is null.");
        Assertion.DebugAssert (type != null, "Parameter 'type' is null.");
        Assertion.DebugAssert (!string.IsNullOrEmpty (memberName), "Parameter 'memberName' is null or empty.");
        
        _attributeType = attributeType;
        _type = type;
        _memberName = memberName;
        _bindingFlags = bindingFlags;
      }

      public Type Type
      {
        get { return _type; }
      }

      public string MemberName
      {
        get { return _memberName; }
      }

      public BindingFlags BindingFlags
      {
        get { return _bindingFlags; }
      }

      public override int GetHashCode ()
      {
        return _type.GetHashCode() ^ _memberName[0];
      }

      public bool Equals (CacheKey other)
      {
        return EqualityUtility.NotNullAndSameType (this, other)
               && _attributeType.Equals (other._attributeType)
               && _type.Equals (other._type)
               && string.Equals (_memberName, other._memberName)
               && _bindingFlags == other._bindingFlags;
      }
    }

    private static readonly ICache<CacheKey, Enum[]> s_cache = new InterlockedCache<CacheKey, Enum[]>();

    public PermissionReflector ()
        : this ("Reflection", new NameValueCollection())
    {
    }

    public PermissionReflector (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      //IMemberInformation mi = _memberResolver.GetMethodInformation (type, methodName);
      //return GetPermissionsFromCache<DemandMethodPermissionAttribute> (mi);
      return GetPermissionsFromCache<DemandMethodPermissionAttribute> (type, methodName, BindingFlags.Public | BindingFlags.Instance);
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return GetPermissionsFromCache<DemandMethodPermissionAttribute> (
          type, methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
    }

    public Enum[] GetRequiredPropertyReadPermissions (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return GetPermissionsFromCache<DemandPropertyReadPermissionAttribute> (
          type, propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public Enum[] GetRequiredPropertyWritePermissions (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return GetPermissionsFromCache<DemandPropertyWritePermissionAttribute> (
          type, propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public Enum[] GetPermissions<TAttribute> (MemberInfo methodInfo) where TAttribute: BaseDemandPermissionAttribute
    {
      var permissionAttribute = AttributeUtility.GetCustomAttribute<TAttribute> (methodInfo, true);
      if (permissionAttribute == null)
        return new Enum[0];

      var permissions = new List<Enum>();
      foreach (Enum accessTypeEnum in permissionAttribute.GetAccessTypes())
      {
        if (!permissions.Contains (accessTypeEnum))
          permissions.Add (accessTypeEnum);
      }

      return permissions.ToArray();
    }

    private Enum[] GetPermissionsFromCache<TAttribute> (Type type, string memberName, BindingFlags bindingFlags)
        where TAttribute: BaseDemandPermissionAttribute
    {
      var cacheKey = new CacheKey (typeof (TAttribute), type, memberName, bindingFlags);
      return s_cache.GetOrCreateValue (cacheKey, key => GetPermissions<TAttribute> (key.Type, key.MemberName, key.BindingFlags));
    }

    private Enum[] GetPermissions<TAttribute> (Type type, string memberName, BindingFlags bindingFlags)
        where TAttribute: BaseDemandPermissionAttribute
    {
      MemberTypes memberType = GetApplicableMemberTypesFromAttributeType (typeof (TAttribute));
      string attributeName = typeof (TAttribute).Name;

      if (!TypeHasMember (type, memberType, memberName, bindingFlags))
        throw new ArgumentException (string.Format (c_memberNotFoundMessage, memberName), "memberName");

      var foundMembers = new List<MemberInfo>();
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        foundMembers.AddRange (
            currentType.FindMembers (memberType, bindingFlags | BindingFlags.DeclaredOnly, IsSecuredMember<TAttribute>, memberName));
      }
      if (foundMembers.Count == 0)
        return new Enum[0];

      MemberInfo foundMember = foundMembers[0];
      if (type.BaseType != null && foundMember.DeclaringType == type && TypeHasMember (type.BaseType, memberType, memberName, bindingFlags))
      {
        throw new ArgumentException (
            string.Format (c_memberPermissionsOnlyInBaseClassMessage, memberName, type.FullName, attributeName), "memberName");
      }

      if (foundMembers.Count > 1)
        throw new ArgumentException (string.Format (c_memberHasMultipleAttributesMessage, memberName, attributeName), "memberName");

      return GetPermissions<TAttribute> (foundMember);
    }

    private MemberTypes GetApplicableMemberTypesFromAttributeType (Type attributeType)
    {
      var attributeUsageAttributes = (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), false);
      AttributeTargets targets = attributeUsageAttributes[0].ValidOn;

      MemberTypes memberTypes = 0;

      if ((targets & AttributeTargets.Method) != 0)
        memberTypes |= MemberTypes.Method;

      if ((targets & AttributeTargets.Property) != 0)
        memberTypes |= MemberTypes.Property;

      return memberTypes;
    }

    private bool TypeHasMember (Type type, MemberTypes memberType, string methodName, BindingFlags bindingFlags)
    {
      MemberInfo[] existingMembers = type.GetMember (methodName, memberType, bindingFlags);
      return existingMembers.Length > 0;
    }

    private bool IsSecuredMember<TAttribute> (MemberInfo member, object filterCriteria) where TAttribute: BaseDemandPermissionAttribute
    {
      string memberName = (string) filterCriteria;
      return member.Name == memberName && member.IsDefined (typeof (TAttribute), false);
    }
  }
}