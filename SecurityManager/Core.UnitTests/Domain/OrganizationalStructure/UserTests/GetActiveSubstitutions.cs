// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class GetActiveSubstitutions : UserTestBase
  {
    public override void SetUp ()
    {
      base.SetUp ();

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    public override void TearDown ()
    {
      base.TearDown ();

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Test ()
    {
      User substitutingUser = CreateUser();

      Substitution enabledSubstitution1 = Substitution.NewObject();
      enabledSubstitution1.SubstitutingUser = substitutingUser;
      enabledSubstitution1.SubstitutedUser = CreateUser();

      Substitution disabledFromFlagSubstitution = Substitution.NewObject();
      disabledFromFlagSubstitution.SubstitutingUser = substitutingUser;
      disabledFromFlagSubstitution.SubstitutedUser = CreateUser();
      disabledFromFlagSubstitution.IsEnabled = false;

      Substitution disabledFromDateSubstitution = Substitution.NewObject();
      disabledFromDateSubstitution.SubstitutingUser = substitutingUser;
      disabledFromDateSubstitution.SubstitutedUser = CreateUser();
      disabledFromDateSubstitution.BeginDate = DateTime.Today.AddDays (1);

      Substitution changedSubstitution = Substitution.NewObject();
      changedSubstitution.SubstitutedUser = CreateUser();
      changedSubstitution.IsEnabled = true;

      Substitution enabledSubstitution2 = Substitution.NewObject();
      enabledSubstitution2.SubstitutingUser = substitutingUser;
      enabledSubstitution2.SubstitutedUser = CreateUser();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        changedSubstitution.SubstitutingUser = substitutingUser;

        Assert.That (substitutingUser.GetActiveSubstitutions(), Is.EquivalentTo (new[] { enabledSubstitution1, enabledSubstitution2 }));
      }
    }

    [Test]
    public void Test_WithPermissionDeniedOnUser ()
    {
      User substitutingUser = CreateUser();

      Substitution substitution1 = Substitution.NewObject();
      substitution1.SubstitutingUser = substitutingUser;
      substitution1.SubstitutedUser = CreateUser();

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      securityProviderStub.Stub (
          stub => stub.GetAccess (Arg<SecurityContext>.Is.Anything, Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ClientTransaction.Current.Extensions.Add (new SecurityClientTransactionExtension());

        Assert.That (substitutingUser.GetActiveSubstitutions(), Is.Empty);
      }
    }
  }
}
