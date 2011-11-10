// This file is part of re-strict (www.re-motion.org)
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
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SecurityManagerPrincipalFactoryTest
  {
    [Test]
    public void CreateWithLocking ()
    {
      var factory = new SecurityManagerPrincipalFactory();

      var principal = factory.CreateWithLocking (
          new ObjectID (typeof (Tenant), Guid.NewGuid()),
          new ObjectID (typeof (User), Guid.NewGuid()),
          null);

      Assert.That (principal, Is.TypeOf<LockingSecurityManagerPrincipalDecorator> ());
      var innerPrincipal = PrivateInvoke.GetNonPublicField (principal, "_innerPrincipal");
      Assert.That (innerPrincipal, Is.TypeOf<SecurityManagerPrincipal>());
    }
  }
}