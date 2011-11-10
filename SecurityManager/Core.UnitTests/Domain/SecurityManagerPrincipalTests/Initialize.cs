﻿// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Initialize : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Initialize_WithObjects ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      Assert.That (principal.Tenant.ID, Is.EqualTo (tenant.ID));
      Assert.That (principal.Tenant, Is.Not.SameAs (tenant));

      Assert.That (principal.User.ID, Is.EqualTo (user.ID));
      Assert.That (principal.User, Is.Not.SameAs (user));

      Assert.That (principal.Substitution.ID, Is.EqualTo (substitution.ID));
      Assert.That (principal.Substitution, Is.Not.SameAs (substitution));
    }

    [Test]
    public void Initialize_WithObjectIDs ()
    {
      User user = User.FindByUserName ("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (tenant.ID, user.ID, substitution.ID);

      Assert.That (principal.Tenant.ID, Is.EqualTo (tenant.ID));
      Assert.That (principal.Tenant, Is.Not.SameAs (tenant));

      Assert.That (principal.User.ID, Is.EqualTo (user.ID));
      Assert.That (principal.User, Is.Not.SameAs (user));

      Assert.That (principal.Substitution.ID, Is.EqualTo (substitution.ID));
      Assert.That (principal.Substitution, Is.Not.SameAs (substitution));
    }}
}