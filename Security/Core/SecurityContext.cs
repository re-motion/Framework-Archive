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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>Collects all security-specific information for a securable object, and is passed as parameter during the permission check.</summary>
  [Serializable]
  public sealed class SecurityContext : ISecurityContext, IEquatable<SecurityContext>
  {
    /// <summary>
    /// Creates a new instance of the <see cref="SecurityContext"/> type initialized for a stateless scenario, i.e. before an actual instance of the
    /// specified <paramref name="type"/> is available to supply state information. One such occurance would be the creation of a new instance of 
    /// specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the securable class for which this <see cref="SecurityContext"/> is created. 
    /// Must implement the <see cref="ISecurableObject"/> interface and not be <see langword="null" />.
    /// </param>
    /// <returns>A new instance of the <see cref="SecurityContext"/> type.</returns>
    public static SecurityContext CreateStateless (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));

      return new SecurityContext (type, null, null, null, true, new Dictionary<string, EnumWrapper>(), new EnumWrapper[0]);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SecurityContext"/> type initialized for stateful scenarios, i.e. when a concrete instance of the 
    /// specified <paramref name="type"/> is available to supply state information (e.g. the <paramref name="owner"/>, ...). This overload 
    /// requires that all state and abstract role values must be defined by a dot.NET <see cref="Enum"/> type.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the securable class for which this <see cref="SecurityContext"/> is created. 
    /// Must implement the <see cref="ISecurableObject"/> interface and not be <see langword="null" />.
    /// </param>
    /// <param name="owner">The name of the user that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerGroup">The name of the group that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerTenant">The name of the tenant that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="states">
    /// A dictionary containing a combination of property names and values, each of which describe a single security relevant property of the 
    /// securable object for which this <see cref="SecurityContext"/> is created. Must not be <see langword="null" />. Each <see cref="Enum"/> value
    /// must be of an <see cref="Enum"/> type that has the <see cref="SecurityStateAttribute"/> applied.
    /// </param>
    /// <param name="abstractRoles">
    /// The list abstract roles the current user has in regards to the securable object for which this <see cref="SecurityContext"/> is created.
    /// Must not be <see langword="null" />. Each <see cref="Enum"/> value must be of an <see cref="Enum"/> type that has the 
    /// <see cref="AbstractRoleAttribute"/> applied.
    /// </param>
    /// <returns>A new instance of the <see cref="SecurityContext"/> type.</returns>
    public static SecurityContext Create (
        Type type, string owner, string ownerGroup, string ownerTenant, IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("states", states);
      ArgumentUtility.CheckNotNull ("abstractRoles", abstractRoles);

      return new SecurityContext (type, owner, ownerGroup, ownerTenant, false, InitializeStates (states), InitializeAbstractRoles (abstractRoles));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SecurityContext"/> type initialized for stateful scenarios, i.e. when a concrete instance of the 
    /// specified paramref name is available to supply state information (e.g. the <paramref name="owner"/>, ...). Use this overload to supply
    /// states and abstract roles that are not necessarily defined by a dot.NET <see cref="Enum"/> type.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the securable class for which this <see cref="SecurityContext"/> is created. 
    /// Must implement the <see cref="ISecurableObject"/> interface and not be <see langword="null" />.
    /// </param>
    /// <param name="owner">The name of the user that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerGroup">The name of the group that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerTenant">The name of the tenant that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="states">
    /// A dictionary containing a combination of property names and values, each of which describe a single security relevant property of the 
    /// securable object for which this <see cref="SecurityContext"/> is created. Must not be <see langword="null" />.
    /// </param>
    /// <param name="abstractRoles">
    /// The list abstract roles the current user has in regards to the securable object for which this <see cref="SecurityContext"/> is created.
    /// Must not be <see langword="null" />.
    /// </param>
    /// <returns>A new instance of the <see cref="SecurityContext"/> type.</returns>
    public static SecurityContext Create (
        Type type,
        string owner,
        string ownerGroup,
        string ownerTenant,
        IDictionary<string, EnumWrapper> states,
        ICollection<EnumWrapper> abstractRoles)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("states", states);
      ArgumentUtility.CheckNotNull ("abstractRoles", abstractRoles);

      return new SecurityContext (type, owner, ownerGroup, ownerTenant, false, new Dictionary<string, EnumWrapper> (states), abstractRoles.ToArray());
    }

    private static EnumWrapper[] InitializeAbstractRoles (ICollection<Enum> abstractRoles)
    {
      List<EnumWrapper> abstractRoleList = new List<EnumWrapper>();

      foreach (Enum abstractRole in abstractRoles)
      {
        Type roleType = abstractRole.GetType();
        if (!AttributeUtility.IsDefined<AbstractRoleAttribute> (roleType, false))
        {
          string message = string.Format (
              "Enumerated Type '{0}' cannot be used as an abstract role. Valid abstract roles must have the {1} applied.",
              roleType,
              typeof (AbstractRoleAttribute).FullName);

          throw new ArgumentException (message, "abstractRoles");
        }

        abstractRoleList.Add (new EnumWrapper (abstractRole));
      }
      return abstractRoleList.ToArray();
    }

    private static Dictionary<string, EnumWrapper> InitializeStates (IDictionary<string, Enum> states)
    {
      Dictionary<string, EnumWrapper> securityStates = new Dictionary<string, EnumWrapper>();

      foreach (KeyValuePair<string, Enum> valuePair in states)
      {
        Type stateType = valuePair.Value.GetType();
        if (!AttributeUtility.IsDefined<SecurityStateAttribute> (stateType, false))
        {
          string message = string.Format (
              "Enumerated Type '{0}' cannot be used as a security state. Valid security states must have the {1} applied.",
              stateType,
              typeof (SecurityStateAttribute).FullName);

          throw new ArgumentException (message, "states");
        }

        securityStates.Add (valuePair.Key, new EnumWrapper (valuePair.Value));
      }
      return securityStates;
    }

    private readonly string _class;
    private readonly string _owner;
    private readonly string _ownerGroup;
    private readonly string _ownerTenant;
    private readonly bool _isStateless;
    private readonly Dictionary<string, EnumWrapper> _states;
    private readonly EnumWrapper[] _abstractRoles;

    private SecurityContext (
        Type classType,
        string owner,
        string ownerGroup,
        string ownerTenant,
        bool isStateless,
        Dictionary<string, EnumWrapper> states,
        EnumWrapper[] abstractRoles)
    {
      _class = TypeUtility.GetPartialAssemblyQualifiedName (classType);
      _owner = StringUtility.NullToEmpty (owner);
      _ownerGroup = StringUtility.NullToEmpty (ownerGroup);
      _ownerTenant = StringUtility.NullToEmpty (ownerTenant);
      _isStateless = isStateless;
      _states = states;
      _abstractRoles = abstractRoles;
    }

    public string Class
    {
      get { return _class; }
    }

    public string Owner
    {
      get { return _owner; }
    }

    public string OwnerGroup
    {
      get { return _ownerGroup; }
    }

    public string OwnerTenant
    {
      get { return _ownerTenant; }
    }

    public EnumWrapper[] AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }

    public bool ContainsState (string propertyName)
    {
      return _states.ContainsKey (propertyName);
    }

    public bool IsStateless
    {
      get { return _isStateless; }
    }

    public int GetNumberOfStates ()
    {
      return _states.Count;
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_class, _owner, _ownerGroup, _ownerTenant);
    }

    public override bool Equals (object obj)
    {
      SecurityContext other = obj as SecurityContext;
      if (other == null)
        return false;
      return ((IEquatable<SecurityContext>) this).Equals (other);
    }

    bool IEquatable<ISecurityContext>.Equals (ISecurityContext other)
    {
      SecurityContext otherContext = other as SecurityContext;
      return otherContext != null && ((IEquatable<SecurityContext>) this).Equals (otherContext);
    }

    bool IEquatable<SecurityContext>.Equals (SecurityContext other)
    {
      if (other == null)
        return false;

      if (!this._class.Equals (other._class, StringComparison.Ordinal))
        return false;

      if (this._isStateless != other._isStateless)
        return false;

      if (!string.Equals (this._owner, other._owner, StringComparison.Ordinal))
        return false;

      if (!string.Equals (this._ownerGroup, other._ownerGroup, StringComparison.Ordinal))
        return false;

      if (!string.Equals (this._ownerTenant, other._ownerTenant, StringComparison.Ordinal))
        return false;

      if (!EqualsStates (this._states, other._states))
        return false;

      return EqualsAbstractRoles (this._abstractRoles, other._abstractRoles);
    }

    private bool EqualsStates (IDictionary<string, EnumWrapper> leftStates, IDictionary<string, EnumWrapper> rightStates)
    {
      if (leftStates.Count != rightStates.Count)
        return false;

      foreach (KeyValuePair<string, EnumWrapper> leftValuePair in leftStates)
      {
        EnumWrapper rightValue;
        if (!rightStates.TryGetValue (leftValuePair.Key, out rightValue))
          return false;
        if (!leftValuePair.Value.Equals (rightValue))
          return false;
      }

      return true;
    }

    private bool EqualsAbstractRoles (EnumWrapper[] leftAbstractRoles, EnumWrapper[] rightAbstractRoles)
    {
      if (leftAbstractRoles.Length != rightAbstractRoles.Length)
        return false;

      HashSet<EnumWrapper> left = new HashSet<EnumWrapper> (leftAbstractRoles);
      return left.SetEquals (rightAbstractRoles);
    }
  }
}