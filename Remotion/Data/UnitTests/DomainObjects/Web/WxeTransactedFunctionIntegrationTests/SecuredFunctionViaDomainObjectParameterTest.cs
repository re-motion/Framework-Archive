﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class SecuredFunctionViaDomainObjectParameterTest : SecuredFunctionTestBase
  {
    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectParameter_WithObjectHasAccessTrue_Succeeds ()
    {
      var wxeFunction = CreateWxeFunctionWithSecurityOnDomainObject();
      ObjectSecurityStrategyStub.Stub (stub => stub.HasAccess (SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue)).Return (true);

      wxeFunction.Execute (Context);
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectParameter_WithObjectHasAccessFalse_Fails ()
    {
      var wxeFunction = CreateWxeFunctionWithSecurityOnDomainObject ();
      ObjectSecurityStrategyStub.Stub (stub => stub.HasAccess (SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue)).Return (false);

      Assert.That (() => wxeFunction.Execute (Context), Throws.TypeOf<WxeUnhandledException>().With.InnerException.TypeOf<PermissionDeniedException>());
    }

    [Test]
    public void HasAccess_ViaDomainObjectParameter_WithFunctionalHasAccessTrue_ReturnsTrue ()
    {
      FunctionalSecurityStrategyStub
          .Stub (stub => stub.HasAccess (typeof (SecurableDomainObject), SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue))
          .Return (true);

      Assert.That (WxeFunction.HasAccess (typeof (FunctionWithSecuredDomainObjectParameter)), Is.True);
    }

    [Test]
    public void HasAccess_ViaDomainObjectParameter_WithFunctionalHasAccessFalse_ReturnsFalse ()
    {
      FunctionalSecurityStrategyStub
          .Stub (stub => stub.HasAccess (typeof (SecurableDomainObject), SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue))
          .Return (false);

      Assert.That (WxeFunction.HasAccess (typeof (FunctionWithSecuredDomainObjectParameter)), Is.False);
    }
    
    private FunctionWithSecuredDomainObjectParameter CreateWxeFunctionWithSecurityOnDomainObject ()
    {
      var securableDomainObject = CreateSecurableDomainObject();

      var wxeFunction = new FunctionWithSecuredDomainObjectParameter (WxeTransactionMode<ClientTransactionFactory>.CreateRoot);
      wxeFunction.SecurableParameter = securableDomainObject;
      return wxeFunction;
    }
  }
}