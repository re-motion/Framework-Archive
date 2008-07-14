/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.UnitTests.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class ContextAwareTypeDiscoveryUtilityTest
  {
    private MockRepository _mockRepository;
    private ITypeDiscoveryService _serviceMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _serviceMock = _mockRepository.CreateMock<ITypeDiscoveryService>();
      ContextAwareTypeDiscoveryUtility.SetDefaultService (null);
      DesignerUtility.ClearDesignMode ();
    }

    [TearDown]
    public void TearDown ()
    {
      ContextAwareTypeDiscoveryUtility.SetDefaultService (null);
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    public void AutoIntoDefaultService ()
    {
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryUtility.DefaultService;
      Assert.IsNotNull (defaultService);
      Assert.AreSame (defaultService, ContextAwareTypeDiscoveryUtility.DefaultService);
      Assert.AreSame (defaultService, ContextAwareTypeDiscoveryUtility.DefaultService);
      Assert.IsInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), defaultService);
      Assert.AreSame (ApplicationAssemblyFinderFilter.Instance, ((AssemblyFinderTypeDiscoveryService) defaultService).AssemblyFinder.Filter);
    }

    [Test]
    public void SetDefaultCurrent ()
    {
      ContextAwareTypeDiscoveryUtility.SetDefaultService (_serviceMock);
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryUtility.DefaultService);
    }

    [Test]
    public void SetDefaultCurrent_Null ()
    {
      ContextAwareTypeDiscoveryUtility.SetDefaultService (_serviceMock);
      ContextAwareTypeDiscoveryUtility.SetDefaultService (null);
      ITypeDiscoveryService defaultService = ContextAwareTypeDiscoveryUtility.DefaultService;
      Assert.IsNotNull (defaultService);
      Assert.AreNotSame (_serviceMock, defaultService);
    }

    [Test]
    public void StandardContext ()
    {
      ContextAwareTypeDiscoveryUtility.SetDefaultService (_serviceMock);
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryUtility.GetInstance ());
    }

    [Test]
    public void DesignModeContext ()
    {
      IDesignerHost designerHostMock = _mockRepository.CreateMock<IDesignerHost>();
      Expect.Call (designerHostMock.GetService (typeof (ITypeDiscoveryService))).Return (_serviceMock);

      _mockRepository.ReplayAll();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.AreSame (_serviceMock, ContextAwareTypeDiscoveryUtility.GetInstance ());

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetType_NormalMode_Success ()
    {
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType (typeof (ContextAwareTypeDiscoveryUtilityTest).AssemblyQualifiedName, true),
          Is.SameAs (typeof (ContextAwareTypeDiscoveryUtilityTest)));
    }

    [Test]
    public void GetType_NormalMode_Failure_Null ()
    {
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType ("dfgj", false), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    public void GetType_NormalMode_Failure_Exception ()
    {
      ContextAwareTypeDiscoveryUtility.GetType ("dfgj", true);
    }

    [Test]
    public void GetType_DesignMode_Success ()
    {
      IDesignerHost designerHostMock = _mockRepository.CreateMock<IDesignerHost> ();
      Expect.Call (designerHostMock.GetType("abc")).Return (typeof (int));

      _mockRepository.ReplayAll ();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType ("abc", true), Is.SameAs (typeof (int)));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetType_DesignMode_Failure_Null ()
    {
      IDesignerHost designerHostMock = _mockRepository.CreateMock<IDesignerHost> ();
      Expect.Call (designerHostMock.GetType ("abc")).Return (null);

      _mockRepository.ReplayAll ();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeDiscoveryUtility.GetType ("abc", false), Is.Null);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Type 'abc' could not be loaded by the designer host.")]
    public void GetType_DesignMode_Failure_Exception ()
    {
      IDesignerHost designerHostMock = _mockRepository.CreateMock<IDesignerHost> ();
      Expect.Call (designerHostMock.GetType ("abc")).Return (null);

      _mockRepository.ReplayAll ();

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      ContextAwareTypeDiscoveryUtility.GetType ("abc", true);
    }
  }
}
