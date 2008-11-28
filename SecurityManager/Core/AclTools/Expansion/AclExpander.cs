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
using System.Diagnostics;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  // TODO AE: Remove commented code. (Do not commit.)
  public class AclExpander
  {
    private readonly IUserRoleAclAceCombinations _userRoleAclAceCombinations;

    // IEqualityComparer for value based comparison of AclExpansionEntry|s.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryEqualityComparer =
      new CompoundValueEqualityComparer<AclExpansionEntry> (a => new object[] {
          //a.User, a.Role, a.Class, a.StateCombinations,
          a.User, a.Role, a.Class, a.AccessControlList is StatefulAccessControlList ? a.StateCombinations : null,
          a.AccessConditions.AbstractRole,
          a.AccessConditions.GroupHierarchyCondition,
          a.AccessConditions.IsOwningUserRequired,
          a.AccessConditions.OwningGroup,
          a.AccessConditions.OwningTenant,
          a.AccessConditions.TenantHierarchyCondition,
          EnumerableEqualsWrapper.New (a.AllowedAccessTypes),
          EnumerableEqualsWrapper.New (a.DeniedAccessTypes)
      }
    );

    public AclExpander (IUserRoleAclAceCombinations userRoleAclAceCombinations)
    {
      ArgumentUtility.CheckNotNull ("userRoleAclAceCombinations", userRoleAclAceCombinations);
      _userRoleAclAceCombinations = userRoleAclAceCombinations;
    }

    // TODO AE: Delegate to other ctor.
    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      ArgumentUtility.CheckNotNull ("userFinder", userFinder);
      ArgumentUtility.CheckNotNull ("accessControlListFinder", accessControlListFinder);
      _userRoleAclAceCombinations = new UserRoleAclAceCombinations (userFinder, accessControlListFinder);
    }

    /// <summary>
    /// Default behavior is to use all <see cref="User"/>|s and all <see cref="AccessControlList"/>|s.
    /// </summary>
    // TODO AE: Test-only ctor, consider dropping.
    public AclExpander () : this (new AclExpanderUserFinder (), new AclExpanderAclFinder ()) {}

    // TODO AE: Consider returning IEnumerable<T> to avoid an unnecessary copy.
    public List<AclExpansionEntry> GetAclExpansionEntryListSortedAndDistinct ()
    {
      return (from AclExpansionEntry aclExpansionEntry in GetAclExpansionEntryList ()
              orderby aclExpansionEntry.User.DisplayName, aclExpansionEntry.Role.DisplayName
              select aclExpansionEntry).Distinct (_aclExpansionEntryEqualityComparer).ToList ();
    }


    /// <summary>
    /// Returns the expansion of <see cref="AccessTypeDefinition"/>|s for 
    /// all the <see cref="User"/>|s and all <see cref="AccessControlList"/>|s  
    /// supplied in the ctor as a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>. 
    /// </summary>
    /// <returns></returns>
    // TODO AE: Consider returning IEnumerable<T> and using yield return to allow for lazier evaluation and more parallelism.
    public List<AclExpansionEntry> GetAclExpansionEntryList ()
    {
      var aclExpansionEntries = new List<AclExpansionEntry> ();

      foreach (UserRoleAclAceCombination userRoleAclAce in _userRoleAclAceCombinations)
      {
        AddAclExpansionEntry (aclExpansionEntries, userRoleAclAce);
      }

      return aclExpansionEntries;
    }


    // TODO AE: Consider changing this into a method returning an AclExpansionEntry.
    // TODO AE: Consider making a GetAclExpansionEntry method public. Does not break encapsulation, and would enable you to test it separately.
    private void AddAclExpansionEntry (List<AclExpansionEntry> aclExpansionEntries, UserRoleAclAceCombination userRoleAclAce)
    {
      //To.ConsoleLine.s ("~~~~~ AddAclExpansionEntry ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      AclProbe aclProbe;
      AccessTypeStatistics accessTypeStatistics;
      AccessInformation accessInformation = GetAccessTypes(userRoleAclAce, out aclProbe, out accessTypeStatistics);


      // Create an AclExpansionEntry, if the current probe ACE contributed to the result and returned allowed access types.
      if (accessTypeStatistics.IsInAccessTypesContributingAces (userRoleAclAce.Ace) && accessInformation.AllowedAccessTypes.Length > 0)
      {
        var aclExpansionEntry = new AclExpansionEntry (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Acl, aclProbe.AccessConditions,
          accessInformation.AllowedAccessTypes, accessInformation.DeniedAccessTypes);
        aclExpansionEntries.Add (aclExpansionEntry);
      }
    }



    // TODO AE: Avoid out parameters if possible. Consider creating a compound type holding the results.
    // TODO AE: No fine-grained unit tests exist. (Only integration tests with GetAclExpansionEntryList.) Fine-grained unit tests would reduce the
    // number of integration tests needed.
    public AccessInformation GetAccessTypes (UserRoleAclAceCombination userRoleAclAce, 
      out AclProbe aclProbe, out AccessTypeStatistics accessTypeStatistics)
    {
      const bool probeForCurrentRoleOnly = true;

      aclProbe = AclProbe.CreateAclProbe (userRoleAclAce.User, userRoleAclAce.Role, userRoleAclAce.Ace);

      // Note: The aclProbe created above will NOT always match the ACE it was designed to probe; the reason for this
      // is that its SecurityToken is only designed to match the non-decideable access conditions
      // (abstract role and owning tenant, group, etc) of the ACE
      // (the "non-decideable" refers to the information context of the AclExpander, which is lacking some information).
      // For decideable access conditons (e.g. specific tenant or specific group), the created SecurityToken
      // is not guaranteed to match. The AccessTypeStatistics returned by Acl.GetAccessTypes are used to filter out these cases.
      //
      // Note also that it does not suffice to get the access types for the current ACE only, since these rights might be denied
      // by another matching ACE in the current ACL. 
      accessTypeStatistics = new AccessTypeStatistics ();

      // Create a discarding sub-transaction so we can change the roles of the current user below without side effects.
      // TODO AE: ClientTransaction.Current could be null. Consider checking at the beginning of the method and throw an InvalidOperationException.
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        // Set roles of user to contain only the role we currently probe for.
        // If we don't do that another role of the user can match the ACE.SpecificPosition
        // for case GroupSelection.All or GroupSelection.OwningGroup, giving access rights
        // which the user does not have due to the currently tested role.
        // (Note that the user is in fact always in all roles at the same time, so he will
        // have the access rights returned if the user's roles are not artificially reduced
        // to contain only the role probed for; it's just not the information we want to present in the 
        // ACL-expansion, where we want to distinguish which role gives rise
        // to what access rights).

        if (probeForCurrentRoleOnly) // TODO AE: Remove if and constant.
        {
          // TODO AE: Roles has no setter anyway, so the following comment seems unnecessary.
          // Exchanging the User.Roles-collection with a new one containing only the current Role would not work (MK),
          // so we empty the collection, then add back the current Role.
          aclProbe.SecurityToken.Principal.Roles.Clear();
          aclProbe.SecurityToken.Principal.Roles.Add (userRoleAclAce.Role);
        }
        AccessInformation accessInformation = userRoleAclAce.Acl.GetAccessTypes (aclProbe.SecurityToken, accessTypeStatistics);
        
        // Non-contributing-ACE debugging
        //NonContributingAceDebugging (ace, aclProbe, accessTypeStatistics, userRoleAclAce.Acl);

       return accessInformation;
      }
    }

  }
}