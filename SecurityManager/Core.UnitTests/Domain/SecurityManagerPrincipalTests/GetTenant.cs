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
using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetTenant : DomainTest
  {
    private User _user;
    private Tenant _tenant;
    private SecurityManagerPrincipal _principal;

    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();

      _user = User.FindByUserName ("substituting.user");
      _tenant = _user.Tenant;

      _principal = new SecurityManagerPrincipal (_tenant.ID, _user.ID, null);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Test ()
    {
      var tenantProxy = _principal.Tenant;

      Assert.That (tenantProxy.ID, Is.EqualTo (_tenant.ID));
    }

    [Test]
    public void UsesCache ()
    {
      Assert.That (_principal.Tenant, Is.SameAs (_principal.Tenant));
    }

    [Test]
    public void DoesNotCacheDuringSerialization ()
    {
      var deserialized = Serializer.SerializeAndDeserialize (Tuple.Create (_principal, _principal.Tenant));
      SecurityManagerPrincipal deserialziedSecurityManagerPrincipal = deserialized.Item1;
      TenantProxy deserialziedTenant = deserialized.Item2;

      Assert.That (deserialziedSecurityManagerPrincipal.Tenant, Is.Not.SameAs (deserialziedTenant));
    }

    [Test]
    public void RefreshDoesNotResetCacheWithOldRevision ()
    {
      TenantProxy proxy = _principal.Tenant;
      _principal.Refresh();
      Assert.That (proxy, Is.SameAs (_principal.Tenant));
    }

    [Test]
    public void RefreshResetsCacheWithNewRevision ()
    {
      TenantProxy proxy = _principal.Tenant;
      Revision.IncrementRevision();
      _principal.Refresh();
      Assert.That (proxy, Is.Not.SameAs (_principal.Tenant));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;
      Revision.IncrementRevision();
      _principal.Refresh();

      var tenantProxy = _principal.Tenant;

      Assert.That (tenantProxy.ID, Is.EqualTo (_tenant.ID));
    }
  }
}