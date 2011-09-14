// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web.Script.Services;
using System.Web.Services;
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.Services;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests
{
  [TestFixture]
  public class CreateScriptService
  {
    private interface IInvalidInterface
    {
    }

    private class InvalidBaseType
    {
    }

    private interface IValidScriptService
    {
      void ScriptMethod ();

      void MethodWithParameters (int param1, bool param2);
    }

    private interface IInvalidScriptServiceWithMissingScriptMethodAttribute
    {
      void MethodWithoutScriptMethodAttribute ();
    }

    [WebService]
    [ScriptService]
    private class TestScriptService : WebService, IValidScriptService, IInvalidScriptServiceWithMissingScriptMethodAttribute
    {
      [WebMethod]
      [ScriptMethod]
      public void ScriptMethod ()
      {
      }

      [WebMethod]
      public void MethodWithoutScriptMethodAttribute ()
      {
      }

      [WebMethod]
      [ScriptMethod]
      public void MethodWithParameters (int param1, bool param2)
      {
      }
    }

    private IBuildManager _buildManagerStub;
    private WebServiceFactory _webServiceFactory;

    [SetUp]
    public void SetUp ()
    {
      _buildManagerStub = MockRepository.GenerateStub<IBuildManager>();
      _webServiceFactory = new WebServiceFactory (_buildManagerStub);
    }

    [Test]
    public void Test ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      var service = _webServiceFactory.CreateScriptService<IValidScriptService> ("~/VirtualServicePath");

      Assert.That (service, Is.InstanceOf<TestScriptService>());
    }

    [Test]
    public void Test_InterfaceNotImplemented ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      Assert.That (
          () => _webServiceFactory.CreateScriptService<IInvalidInterface> ("~/VirtualServicePath"),
          Throws.TypeOf<ArgumentException>().And.Message.EqualTo (
              "Web service '~/VirtualServicePath' does not implement mandatory interface "
              + "'Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests.CreateScriptService+IInvalidInterface'."));
    }

    [Test]
    public void Test_BaseTypeNotImplemented ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      Assert.That (
          () => _webServiceFactory.CreateScriptService<InvalidBaseType> ("~/VirtualServicePath"),
          Throws.TypeOf<ArgumentException>().And.Message.EqualTo (
              "Web service '~/VirtualServicePath' is not based on type "
              + "'Remotion.Web.UnitTests.Core.Services.WebServiceFactoryTests.CreateScriptService+InvalidBaseType'."));
    }

    [Test]
    public void Test_FactoryChecksScriptServiceDeclaration ()
    {
      _buildManagerStub.Stub (stub => stub.GetCompiledType ("~/VirtualServicePath")).Return (typeof (TestScriptService));

      Assert.That (
          () => _webServiceFactory.CreateScriptService<IInvalidScriptServiceWithMissingScriptMethodAttribute> ("~/VirtualServicePath"),
          Throws.TypeOf<ArgumentException>().And.Message.ContainsSubstring (
              " does not have the 'System.Web.Script.Services.ScriptMethodAttribute' applied."));
    }
  }
}