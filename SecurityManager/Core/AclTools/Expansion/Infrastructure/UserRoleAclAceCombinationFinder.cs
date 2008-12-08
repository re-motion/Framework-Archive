// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// Supplies enumeration over all <see cref="Role"/>|s of <see cref="User"/>|s (taken from <see cref="IAclExpanderUserFinder"/>) and 
  /// <see cref="AccessControlEntry"/>|s of <see cref="AccessControlList"/>|s (taken from <see cref="IAclExpanderAclFinder"/>)
  /// combinations. 
  /// </summary>
  public class UserRoleAclAceCombinationFinder : IUserRoleAclAceCombinationFinder
  {
    private readonly IAclExpanderUserFinder _userFinder;
    private readonly IAclExpanderAclFinder _accessControlListFinder;


    public UserRoleAclAceCombinationFinder (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      _userFinder = userFinder;
      _accessControlListFinder = accessControlListFinder;
    }

    public IEnumerator<UserRoleAclAceCombination> GetEnumerator ()
    {
      var users = _userFinder.FindUsers ();
      var acls = _accessControlListFinder.FindAccessControlLists ();

      foreach (var user in users)
      {
        //To.ConsoleLine.e (() => user);
        foreach (var role in user.Roles)
        {
          //To.ConsoleLine.s ("\t").e (() => role);
          foreach (var acl in acls)
          {
            //To.ConsoleLine.s ("\t\t").e (() => acl);
            foreach (var ace in acl.AccessControlEntries)
            {
              //To.ConsoleLine.s ("\t\t\t").e (() => ace);
              yield return new UserRoleAclAceCombination (role, ace);
            }
          }
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
