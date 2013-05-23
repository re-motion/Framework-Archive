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
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class SecurableClassDefinitionData
  {
    private readonly string _baseClass;
    private readonly IDomainObjectHandle<StatelessAccessControlList> _statelessAccessControlList;
    private readonly ReadOnlyCollectionDecorator<StatefulAccessControlListData> _statefulAccessControlList;

    public SecurableClassDefinitionData (
        [CanBeNull] string baseClass,
        [CanBeNull] IDomainObjectHandle<StatelessAccessControlList> statelessAccessControlList,
        IEnumerable<StatefulAccessControlListData> statefulAccessControlList)
    {
      ArgumentUtility.CheckNotNull ("statefulAccessControlList", statefulAccessControlList);

      _baseClass = baseClass;
      _statelessAccessControlList = statelessAccessControlList;
      _statefulAccessControlList = statefulAccessControlList.ToArray().AsReadOnly();
    }

    [CanBeNull]
    public string BaseClass
    {
      get { return _baseClass; }
    }

    [CanBeNull]
    public IDomainObjectHandle<StatelessAccessControlList> StatelessAccessControlList
    {
      get { return _statelessAccessControlList; }
    }

    public ReadOnlyCollectionDecorator<StatefulAccessControlListData> StatefulAccessControlList
    {
      get { return _statefulAccessControlList; }
    }
  }

  public class StatefulAccessControlListData
  {
    private readonly IDomainObjectHandle<StatefulAccessControlList> _handle;
    private readonly State _state;

    public StatefulAccessControlListData ([NotNull] IDomainObjectHandle<StatefulAccessControlList> handle, [NotNull] State state)
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      ArgumentUtility.CheckNotNull ("state", state);

      _handle = handle;
      _state = state;
    }

    [NotNull]
    public IDomainObjectHandle<StatefulAccessControlList> Handle
    {
      get { return _handle; }
    }

    [NotNull]
    public State State
    {
      get { return _state; }
    }
  }

  public class State
  {
    private readonly string _propertyName;
    private readonly string _propertyValue;

    public State (string propertyName, string propertyValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyValue", propertyValue);
     
      _propertyName = propertyName;
      _propertyValue = propertyValue;
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public string PropertyValue
    {
      get { return _propertyValue; }
    }
  }

  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityContextRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class SecurityContextRepository : RepositoryBase<SecurityContextRepository.Data>, ISecurityContextRepository
  {
    public class Data : RevisionBasedData
    {
      public readonly Dictionary<string, IDomainObjectHandle<Tenant>> Tenants;
      public readonly Dictionary<string, IDomainObjectHandle<Group>> Groups;
      public readonly Dictionary<string, IDomainObjectHandle<User>> Users;
      public readonly Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> AbstractRoles;
      public readonly Dictionary<string, SecurableClassDefinitionData> Classes;

      internal Data (
          int revision,
          Dictionary<string, IDomainObjectHandle<Tenant>> tenants,
          Dictionary<string, IDomainObjectHandle<Group>> groups,
          Dictionary<string, IDomainObjectHandle<User>> users,
          Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> abstractRoles,
          Dictionary<string, SecurableClassDefinitionData> classes)
        :base (revision)
      {
        Tenants = tenants;
        Groups = groups;
        Users = users;
        AbstractRoles = abstractRoles;
        Classes = classes;
      }
    }

    public SecurityContextRepository (IRevisionProvider revisionProvider)
      : base (revisionProvider)
    {
    }

    public IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var cachedData = GetCachedData();
      var tenant = cachedData.Tenants.GetValueOrDefault (uniqueIdentifier);
      if (tenant == null)
        throw CreateAccessControlException ("The tenant '{0}' could not be found.", uniqueIdentifier);
      return tenant;
    }

    public IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var cachedData = GetCachedData();
      var group = cachedData.Groups.GetValueOrDefault (uniqueIdentifier);
      if (group == null)
        throw CreateAccessControlException ("The group '{0}' could not be found.", uniqueIdentifier);
      return group;
    }

    public IDomainObjectHandle<User> GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      var cachedData = GetCachedData();
      var user = cachedData.Users.GetValueOrDefault (userName);
      if (user == null)
        throw CreateAccessControlException ("The user '{0}' could not be found.", userName);
      return user;
    }

    public IDomainObjectHandle<AbstractRoleDefinition> GetAbstractRole (EnumWrapper name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      var cachedData = GetCachedData();
      var abstractRole = cachedData.AbstractRoles.GetValueOrDefault (name);
      if (abstractRole == null)
        throw CreateAccessControlException ("The abstract role '{0}' could not be found.", name);
      return abstractRole;
    }

    public SecurableClassDefinitionData GetClass (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var cachedData = GetCachedData();
      var @class = cachedData.Classes.GetValueOrDefault (name);
      if (@class == null)
        throw CreateAccessControlException ("The securable class '{0}' could not be found.", name);
      return @class;
    }

    protected override Data LoadData (int revision)
    {
      using (new SecurityFreeSection())
      {
        using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
        {
          var tenants = QueryFactory.CreateLinqQuery<Tenant>()
                                    .Select (t => new { Key = t.UniqueIdentifier, Value = t.ID })
                                    .ToDictionary (t => t.Key, t => t.Value.GetHandle<Tenant>());

          var groups = QueryFactory.CreateLinqQuery<Group>()
                                   .Select (g => new { Key = g.UniqueIdentifier, Value = g.ID })
                                   .ToDictionary (g => g.Key, g => g.Value.GetHandle<Group>());

          var users = QueryFactory.CreateLinqQuery<User>()
                                  .Select (u => new { Key = u.UserName, Value = u.ID })
                                  .ToDictionary (u => u.Key, u => u.Value.GetHandle<User>());

          var abstractRoles = QueryFactory.CreateLinqQuery<AbstractRoleDefinition>()
                                          .Select (r => new { Key = r.Name, Value = r.ID })
                                          .ToDictionary (r => EnumWrapper.Get (r.Key), r => r.Value.GetHandle<AbstractRoleDefinition>());

          PrefetchStateDefinitions();
          PrefetchStatePropertyDefinitions();
          var classes = QueryFactory.CreateLinqQuery<SecurableClassDefinition>().Select (c => c)
                                    .FetchOne (cd => cd.StatelessAccessControlList)
                                    .FetchMany (cd => cd.StatefulAccessControlLists)
                                    .ThenFetchMany (StatefulAccessControlList.SelectStateCombinations())
                                    .ThenFetchMany (StateCombination.SelectStateUsages()).
                                     ToDictionary (c => c.Name, CreateCachableData);

          return new Data (revision, tenants, groups, users, abstractRoles, classes);
        }
      }
    }

    private static void PrefetchStateDefinitions ()
    {
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
      QueryFactory.CreateLinqQuery<StateDefinition>().AsEnumerable().FirstOrDefault();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
    }
    
    private static void PrefetchStatePropertyDefinitions ()
    {
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
      QueryFactory.CreateLinqQuery<StatePropertyDefinition>().AsEnumerable().FirstOrDefault();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
    }

    private SecurableClassDefinitionData CreateCachableData (SecurableClassDefinition @class)
    {
      return new SecurableClassDefinitionData (
          @class.BaseClass != null ? @class.BaseClass.Name : null,
          @class.StatelessAccessControlList.GetSafeHandle(),
          @class.StatefulAccessControlLists.SelectMany (
              acl => acl.StateCombinations.SelectMany (sc => sc.GetStates())
                        .Select (sd => CreateStatefulAccessControlListData (acl, sd))));
    }

    private StatefulAccessControlListData CreateStatefulAccessControlListData (StatefulAccessControlList acl, StateDefinition stateDefinition)
    {
      return new StatefulAccessControlListData (acl.GetHandle(), new State (stateDefinition.StateProperty.Name, stateDefinition.Name));
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}