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
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.Security.Configuration;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class BindableObjectWithSecurityTest : BindableObjectTestBase
  {
    [SetUp]
    public void SetUp ()
    {
      SecurityConfiguration.Current.SecurityProvider = new StubSecurityProvider();
      SecurityConfiguration.Current.PrincipalProvider = new ThreadPrincipalProvider();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
      var clientTransaction = new SecurityClientTransactionFactory().CreateRootTransaction();
      clientTransaction.To<ClientTransaction>().EnterDiscardingScope();
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityProvider = null;
      SecurityConfiguration.Current.PrincipalProvider = null;
      ClientTransactionScope.ResetActiveScope();
    }

    [Test]
    public override void BusinessObject_Property_IsAccessible ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for BusinessObject_Property_IsAccessible on reference system: ~2.5 �s (release build), ~6.7 �s (debug build)");

      base.BusinessObject_Property_IsAccessible();

      Console.WriteLine ();
    }

    [Test]
    public override void BusinessObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for BusinessObject_GetProperty on reference system: ~8.6 �s (release build), ~20.0 �s (debug build)");

      base.BusinessObject_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DynamicMethod_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for DynamicMethod_GetProperty on reference system: ~6.3 �s (release build), ~13.7 �s (debug build)");

      base.DynamicMethod_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for DomainObject_GetProperty on reference system: ~6.2 �s (release build), ~13.6 �s (debug build)");

      base.DomainObject_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void BusinessObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for BusinessObject_SetProperty on reference system: ~8.2 �s (release build), ~17.4 �s (debug build)");

      base.BusinessObject_SetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithSecurityTest for DomainObject_SetProperty on reference system: ~7.6 �s (release build), ~17.1 �s (debug build)");

      base.DomainObject_SetProperty ();

      Console.WriteLine ();
    }
  }
}