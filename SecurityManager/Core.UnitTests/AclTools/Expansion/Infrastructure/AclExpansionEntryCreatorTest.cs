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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.Infrastructure
{
  [TestFixture]
  public class AclExpansionEntryCreatorTest : AclToolsTestBase
  {
    [Test]
    public void GetAccessTypesTest ()
    {
      var ace = TestHelper.CreateAceWithNoMatchingRestrictions ();
      AttachAccessTypeReadWriteDelete (ace, true, false, true);
      Assert.That (ace.Validate ().IsValid);
      TestHelper.CreateStatefulAcl (ace);

      var aclExpansionEntryCreator = new AclExpansionEntryCreator();
      //AclProbe aclProbe;
      //AccessTypeStatistics accessTypeStatistics;
      var accessTypesResult = 
        aclExpansionEntryCreator.GetAccessTypes (new UserRoleAclAceCombination (Role2, ace)); //, out aclProbe, out accessTypeStatistics);

      //To.ConsoleLine.e (accessInformation.AllowedAccessTypes);
      //To.ConsoleLine.e (accessInformation.DeniedAccessTypes);

      Assert.That (accessTypesResult.AccessInformation.AllowedAccessTypes, Is.EquivalentTo (new[] { ReadAccessType, DeleteAccessType }));
      Assert.That (accessTypesResult.AccessInformation.DeniedAccessTypes, Is.EquivalentTo (new[] { WriteAccessType }));
    }

    [Test]
    public void GetAccessTypesUsesDiscardingScopeTest ()
    {
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var aclExpansionEntryCreator = new AclExpansionEntryCreator ();
        //AclProbe aclProbe;
        //AccessTypeStatistics accessTypeStatistics;
        User.Roles.Add (Role2);
        //To.ConsoleLine.e (User.Roles);
        var userRoleAclAceCombination = new UserRoleAclAceCombination (Role, Ace);
        var accessTypesResult =
          aclExpansionEntryCreator.GetAccessTypes (userRoleAclAceCombination); //, out aclProbe, out accessTypeStatistics);
        Assert.That (User.Roles, Is.EquivalentTo (new[] { Role, Role2 }));
        accessTypesResult.AclProbe.SecurityToken.Principal.Roles.Clear ();
        accessTypesResult.AclProbe.SecurityToken.Principal.Roles.Add (userRoleAclAceCombination.Role);
        Assert.That (User.Roles, Is.Not.EquivalentTo (new[] { Role, Role2 }));
      }
    }


    [Test]
    public void CreateAclExpansionEntryTest ()
    {
      var userRoleAclAce = new UserRoleAclAceCombination (Role, Ace);
      var allowedAccessTypes = new[] { WriteAccessType, DeleteAccessType };
      var deniedAccessTypes = new[] { ReadAccessType };
      AccessInformation accessInformation = new AccessInformation (allowedAccessTypes, deniedAccessTypes);

      var mocks = new MockRepository ();
      var aclExpansionEntryCreatorMock = mocks.PartialMock<AclExpansionEntryCreator> ();
      var aclProbe = AclProbe.CreateAclProbe (User, Role, Ace);
      var accessTypeStatisticsMock = mocks.StrictMock<AccessTypeStatistics>();
      accessTypeStatisticsMock.Expect (x => x.IsInAccessTypesContributingAces (userRoleAclAce.Ace)).Return(true);

      aclExpansionEntryCreatorMock.Expect (x => x.GetAccessTypes (userRoleAclAce)).
        Return (new AclExpansionEntryCreator_GetAccessTypesResult (accessInformation, aclProbe, accessTypeStatisticsMock));

      aclExpansionEntryCreatorMock.Replay ();
      accessTypeStatisticsMock.Replay ();

      var aclExpansionEntry = aclExpansionEntryCreatorMock.CreateAclExpansionEntry (userRoleAclAce);

      aclExpansionEntryCreatorMock.VerifyAllExpectations ();
      accessTypeStatisticsMock.VerifyAllExpectations();

      Assert.That (aclExpansionEntry.User, Is.EqualTo (userRoleAclAce.User));
      Assert.That (aclExpansionEntry.Role, Is.EqualTo (userRoleAclAce.Role));
      Assert.That (aclExpansionEntry.AccessControlList, Is.EqualTo (userRoleAclAce.Acl));
      Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (aclProbe.AccessConditions));
      Assert.That (aclExpansionEntry.AllowedAccessTypes, Is.EqualTo (allowedAccessTypes));
      Assert.That (aclExpansionEntry.DeniedAccessTypes, Is.EqualTo (deniedAccessTypes));
    }





  }
}
