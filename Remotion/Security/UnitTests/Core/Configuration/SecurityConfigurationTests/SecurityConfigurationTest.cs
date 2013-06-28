// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class SecurityConfigurationTest: TestBase
  {
    [Test]
    public void GetSecurityConfigurationWithoutConfigurationSection()
    {
      SecurityConfiguration configuration = SecurityConfiguration.Current;

      Assert.That (configuration, Is.Not.Null);
      Assert.IsInstanceOf (typeof (NullSecurityProvider), configuration.SecurityProvider);
      Assert.IsInstanceOf (typeof (ThreadPrincipalProvider), configuration.PrincipalProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNamespace()
    {
      string xmlFragment = @"<remotion.security xmlns=""http://www.re-motion.org/Security/Configuration/2.0"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      // Succeeded
    }
  }
}
