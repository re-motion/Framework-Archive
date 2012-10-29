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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  [Serializable]
  public class AppDomainAssemblyResolverTest
  {
    private string _testDllPath;
    private string _testExePath;
    private string _testInvalidDllPath;

    private string _domainBase;
    private AppDomain _appDomain;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      _testDllPath = CreateAssembly ("TestDll", "TestDll" + "." + "dll");
      _testExePath = CreateAssembly ("TestExe", "TestExe" + "." + "exe");
      _testInvalidDllPath = CreateAssembly ("TestDllWhatever", "TestDllInvalid" + "." + "dll");
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      FileUtility.DeleteAndWaitForCompletion (_testDllPath);
      FileUtility.DeleteAndWaitForCompletion (_testExePath);
      FileUtility.DeleteAndWaitForCompletion (_testInvalidDllPath);
    }

    [SetUp]
    public void SetUp ()
    {
      _domainBase = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "AppDomainAsselbyResolverTest");
      _appDomain = AppDomain.CreateDomain ("Test", null, _domainBase, null, false);
    }

    [TearDown]
    public void TearDown ()
    {
      AppDomain.Unload (_appDomain);
      
      if (Directory.Exists (_domainBase))
        Directory.Delete (_domainBase, true);
    }

    [Test]
    public void CreateInAppDomain ()
    {
      var resolver = AppDomainAssemblyResolver.CreateInAppDomain (_appDomain, AppDomain.CurrentDomain.BaseDirectory);
      Assert.That (RemotingServices.IsTransparentProxy (resolver), Is.True);
      Assert.That (resolver.AssemblyDirectory, Is.EqualTo (AppDomain.CurrentDomain.BaseDirectory));
    }

    [Test]
    public void CreateInAppDomain_FromShadowCopiedScenario ()
    {
      var setupInfo = AppDomain.CurrentDomain.SetupInformation;
      setupInfo.ShadowCopyFiles = "true";
      var shadowCopiedAppDomain = AppDomain.CreateDomain ("ShadowCopier", AppDomain.CurrentDomain.Evidence, setupInfo);
      try
      {
        shadowCopiedAppDomain.DoCallBack (
            delegate
            {
              var resolver = AppDomainAssemblyResolver.CreateInAppDomain (_appDomain, AppDomain.CurrentDomain.BaseDirectory);
              Assert.That (resolver, Is.Not.Null);
            });
      }
      finally
      {
        AppDomain.Unload (shadowCopiedAppDomain);
      }
    }

    [Test]
    public void Register_AllowsAssembliesToBeResolved()
    {
      var resolver = CreateResolver ();

      try
      {
        _appDomain.DoCallBack (delegate { });
        Assert.Fail ("Expected exception");
      }
      catch (FileNotFoundException)
      {
        // ok
      }

      resolver.Register (_appDomain);

      _appDomain.DoCallBack (delegate { });
    }

    [Test]
    public void Resolve_FindsDlls ()
    {
      var resolver = CreateResolver ();

      resolver.Register (_appDomain);

      _appDomain.DoCallBack (() => Assembly.Load ("TestDll"));
    }

    [Test]
    public void Resolve_FindsExes ()
    {
      var resolver = CreateResolver ();

      resolver.Register (_appDomain);

      _appDomain.DoCallBack (() => Assembly.Load ("TestExe"));
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException), ExpectedMessage = 
        "Could not load file or assembly 'TestTxt' or one of its dependencies. The system cannot find the file specified.")]
    public void Resolve_NonExistingAssembly ()
    {
      var resolver = CreateResolver ();

      resolver.Register (_appDomain);

      _appDomain.DoCallBack (() => Assembly.Load ("TestTxt"));
    }

    [Test]
    [ExpectedException (typeof (FileLoadException), ExpectedMessage =
        "Could not load file or assembly 'TestDllInvalid'. The located assembly's manifest definition does not match the assembly reference.")]
    public void Resolve_ManifestDoesntMatch ()
    {
      var resolver = CreateResolver ();

      resolver.Register (_appDomain);

      _appDomain.DoCallBack (() => Assembly.Load ("TestDllInvalid"));
    }

    private AppDomainAssemblyResolver CreateResolver ()
    {
      return AppDomainAssemblyResolver.CreateInAppDomain (_appDomain, AppDomain.CurrentDomain.BaseDirectory);
    }

    private string CreateAssembly (string assemblyName, string moduleName)
    {
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName (assemblyName), AssemblyBuilderAccess.Save);
      assemblyBuilder.DefineDynamicModule (moduleName);
      assemblyBuilder.Save (moduleName);
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, moduleName);
    }
  }
}