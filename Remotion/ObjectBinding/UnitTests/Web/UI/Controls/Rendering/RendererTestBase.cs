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
using System.Collections;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using System.Web;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering
{
  [TestFixture]
  public class RendererTestBase
  {
    protected HttpContextBase HttpContext { get; private set; }
    protected HtmlHelper Html { get; private set; }

    protected RendererTestBase ()
    {
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
    }

    protected virtual void Initialize ()
    {
      Html = new HtmlHelper();

      HttpContext = MockRepository.GenerateMock<HttpContextBase>();
      HttpResponseBase response = MockRepository.GenerateMock<HttpResponseBase>();
      HttpContext.Stub (mock => mock.Response).Return (response);
      response.Stub (mock => mock.ContentType).Return ("text/html");

      HttpBrowserCapabilities browser = new HttpBrowserCapabilities();
      browser.Capabilities = new Hashtable();
      browser.Capabilities.Add ("browser", "IE");
      browser.Capabilities.Add ("majorversion", "7");

      var request = MockRepository.GenerateStub<HttpRequestBase>();
      request.Stub (stub => stub.Browser).Return (new HttpBrowserCapabilitiesWrapper (browser));

      HttpContext = MockRepository.GenerateStub<HttpContextBase>();
      HttpContext.Stub (stub => stub.Request).Return (request);
    }
  }
}
