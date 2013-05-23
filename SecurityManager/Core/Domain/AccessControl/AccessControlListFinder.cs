// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlListFinder : IAccessControlListFinder
  {
    private readonly ISecurityContextRepository _securityContextRepository;

    public AccessControlListFinder (ISecurityContextRepository securityContextRepository)
    {
      ArgumentUtility.CheckNotNull ("securityContextRepository", securityContextRepository);
      
      _securityContextRepository = securityContextRepository;
    }

    /// <exception cref="AccessControlException">
    ///   The <see cref="SecurableClassDefinition"/> is not found.<br/>- or -<br/>
    ///   A matching <see cref="AccessControlList"/> is not found.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and a <see cref="StatePropertyDefinition"/> is missing.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and contains an invalid state for a <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public IDomainObjectHandle<AccessControlList> Find (ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      for (var @class = GetClass(context.Class); @class.BaseClass != null; @class = GetClass (@class.BaseClass))
      {
        var foundAccessControlList = FindAccessControlList (@class, context);
        if (foundAccessControlList != null)
          return foundAccessControlList;
      }

      throw CreateAccessControlException ("The ACL for the securable class '{0}' could not be found.", context.Class);
    }

    private SecurableClassDefinitionData GetClass (string className)
    {
      return _securityContextRepository.GetClass (className);
    }

    private IDomainObjectHandle<AccessControlList> FindAccessControlList (SecurableClassDefinitionData classData, ISecurityContext context)
    {
      if (context.IsStateless)
        return classData.StatelessAccessControlList;
      else
        return classData.StatefulAccessControlLists.Where (acl => MatchesStates (context, acl.States)).Select (acl => acl.Handle).FirstOrDefault();
    }

    private bool MatchesStates (ISecurityContext context, ICollection<State> states)
    {
      if (context.GetNumberOfStates() > states.Count)
        return false;

      return states.All (s => MatchesState (context, s));
    }

    private bool MatchesState (ISecurityContext context, State state)
    {
      if (!context.ContainsState (state.PropertyName))
        throw CreateAccessControlException ("The state '{0}' is missing in the security context.", state.PropertyName);

      var enumWrapper = context.GetState (state.PropertyName);

      var validStates = _securityContextRepository.GetStatePropertyValues (state.PropertyHandle);
      if (!validStates.Contains (enumWrapper.Name))
      {
        throw CreateAccessControlException (
            "The state '{0}' is not defined for the property '{1}' of the securable class '{2}' or its base classes.",
            enumWrapper.Name,
            state.PropertyName,
            context.Class);
      }

      return enumWrapper.Name.Equals (state.Value);
    }

    private StateCombination FindStateCombination (SecurableClassDefinition classDefinition, ISecurityContext context)
    {
      var states = GetStates (classDefinition.StateProperties, context);
      if (states == null)
        return null;

      return classDefinition.FindStateCombination (states);
    }

    private List<StateDefinition> GetStates (IList<StatePropertyDefinition> stateProperties, ISecurityContext context)
    {
      if (context.GetNumberOfStates() > stateProperties.Count)
        return null;

      return stateProperties.Select (stateProperty => GetState (stateProperty, context)).ToList();
    }

    private StateDefinition GetState (StatePropertyDefinition property, ISecurityContext context)
    {
      if (!context.ContainsState (property.Name))
        throw CreateAccessControlException ("The state '{0}' is missing in the security context.", property.Name);

      EnumWrapper enumWrapper = context.GetState (property.Name);

      if (!property.ContainsState (enumWrapper.Name))
      {
        throw CreateAccessControlException (
            "The state '{0}' is not defined for the property '{1}' of the securable class '{2}' or its base classes.",
            enumWrapper.Name,
            property.Name,
            context.Class);
      }

      return property.GetState (enumWrapper.Name);
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
