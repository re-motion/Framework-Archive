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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino_Is = Rhino.Mocks.Constraints.Is;
using System.Linq;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  [Serializable]
  public class AssemblyFinderIntegrationTest
  {
    private const string c_testAssemblySourceDirectoryRoot = @"Reflection\TypeDiscovery\TestAssemblies";
    private AssemblyCompilerTestHelper _baseDirectoryHelper;
    private AssemblyCompilerTestHelper _dynamicDirectoryHelper;
    private AssemblyCompilerTestHelper _searchPathForDllsHelper;
    private AssemblyCompilerTestHelper _searchPathForExesHelper;

    private string _markedAssemblyPath;
    private string _markedExeAssemblyPath;
    private string _markedAssemblyWithDerivedAttributePath;
    private string _markedReferencedAssemblyPath;

    private string _markedAssemblyInSearchPathPath;
    private string _markedExeAssemblyInSearchPathPath;
    private string _markedAssemblyInSearchPathWithNameMismatchPath;

    private string _markedAssemblyInDynamicDirectoryPath;
    private string _markedExeAssemblyInDynamicDirectoryPath;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      var searchPathForDlls = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Dlls");
      var searchPathForExes = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Exes");
      var dynamicBase = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Dynamic");

      _baseDirectoryHelper = CreateAssemblyCompilerTestHelper (AppDomain.CurrentDomain.BaseDirectory);
      _dynamicDirectoryHelper = CreateAssemblyCompilerTestHelper (dynamicBase);
      _searchPathForDllsHelper = CreateAssemblyCompilerTestHelper (searchPathForDlls);
      _searchPathForExesHelper = CreateAssemblyCompilerTestHelper (searchPathForExes);

      _markedReferencedAssemblyPath = _baseDirectoryHelper.CompileAssemblyInSeparateAppDomain ("MarkedReferencedAssembly.dll");
      _markedAssemblyPath = _baseDirectoryHelper.CompileAssemblyInSeparateAppDomain ("MarkedAssembly.dll", _markedReferencedAssemblyPath);
      _markedExeAssemblyPath = _baseDirectoryHelper.CompileAssemblyInSeparateAppDomain ("MarkedExeAssembly.dll");
      _markedAssemblyWithDerivedAttributePath = _baseDirectoryHelper.CompileAssemblyInSeparateAppDomain ("MarkedAssemblyWithDerivedAttribute.dll");
      _baseDirectoryHelper.CompileAssemblyInSeparateAppDomain ("UnmarkedAssembly.dll");

      _markedAssemblyInSearchPathPath = _searchPathForDllsHelper.CompileAssemblyInSeparateAppDomain ("MarkedAssemblyInRelativeSearchPath.dll");
      _markedExeAssemblyInSearchPathPath = _searchPathForExesHelper.CompileAssemblyInSeparateAppDomain ("MarkedExeAssemblyInRelativeSearchPath.exe");

      _markedAssemblyInSearchPathWithNameMismatchPath =
          _searchPathForDllsHelper.CompileAssemblyInSeparateAppDomain ("MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");
      _markedAssemblyInSearchPathWithNameMismatchPath = _searchPathForDllsHelper.RenameGeneratedAssembly (
          "MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll", "_MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");

      _markedAssemblyInDynamicDirectoryPath = _dynamicDirectoryHelper.CompileAssemblyInSeparateAppDomain ("MarkedAssemblyInDynamicDirectory.dll");
      _markedExeAssemblyInDynamicDirectoryPath = _dynamicDirectoryHelper.CompileAssemblyInSeparateAppDomain (
          "MarkedExeAssemblyInDynamicDirectory.exe");
    }

    [TestFixtureTearDown]
    public void TeastFixtureTearDown ()
    {
      _baseDirectoryHelper.Dispose();
      _dynamicDirectoryHelper.Dispose();
      _searchPathForDllsHelper.Dispose ();
      _searchPathForExesHelper.Dispose ();
    }

    [Test]
    public void FindRootAssemblies_ForAppDomain_WithConsiderDynamicDirectoryTrue ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory ("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory ("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory ("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory ();

        FilteringAssemblyLoader loader = CreateLoaderForMarkedAssemblies ();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies (loader);

        Assert.That (rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That (rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That (
            rootAssemblies.Select (root => root.Assembly).ToArray(),
            Is.EquivalentTo (
                new[]
                {
                    Load (_markedAssemblyPath),
                    Load (_markedExeAssemblyPath),
                    Load (_markedAssemblyWithDerivedAttributePath),
                    Load (_markedReferencedAssemblyPath),
                    Load (_markedAssemblyInSearchPathPath),
                    Load (_markedExeAssemblyInSearchPathPath),
                    Load (_markedAssemblyInDynamicDirectoryPath),
                    Load (_markedExeAssemblyInDynamicDirectoryPath),
                    LoadFile (_markedAssemblyInSearchPathWithNameMismatchPath)
                }));
      });
    }

    [Test]
    public void FindRootAssemblies_WithConsiderDynamicDirectoryFalse ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory ("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory ("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory ("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory ();

        FilteringAssemblyLoader loader = CreateLoaderForMarkedAssemblies ();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (false);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies (loader);

        Assert.That (rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That (rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That (
            rootAssemblies.Select (root => root.Assembly).ToArray (),
            Is.EquivalentTo (
                new[]
                {
                    Load (_markedAssemblyPath),
                    Load (_markedExeAssemblyPath),
                    Load (_markedAssemblyWithDerivedAttributePath),
                    Load (_markedReferencedAssemblyPath),
                    Load (_markedAssemblyInSearchPathPath),
                    Load (_markedExeAssemblyInSearchPathPath),
                    LoadFile (_markedAssemblyInSearchPathWithNameMismatchPath)
                }));
      });
    }

    [Test]
    public void FindAssemblies_References ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        Assembly markedAssembly = Load (_markedAssemblyPath);

        var loader = CreateLoaderForMarkedAssemblies ();
        var rootAssemblyFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
        rootAssemblyFinderStub.Stub (stub => stub.FindRootAssemblies (loader)).Return (new[] { new RootAssembly (markedAssembly, true) });
        rootAssemblyFinderStub.Replay ();

        var assemblyFinder = new AssemblyFinder (rootAssemblyFinderStub, loader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies ();
        Assert.That (assemblies, Is.EquivalentTo (new[] { markedAssembly, Load (_markedReferencedAssemblyPath) }));
      });
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyFalse ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        InitializeDynamicDirectory ();

        var filterMock = new MockRepository ().StrictMock<IAssemblyLoaderFilter> ();
        filterMock.Expect (mock => mock.ShouldConsiderAssembly (Arg<AssemblyName>.Is.Anything)).Return (false).Repeat.AtLeastOnce ();
        filterMock.Replay ();

        var loader = new FilteringAssemblyLoader (filterMock);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true);
        var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, loader);
        
        Assembly[] assemblies = assemblyFinder.FindAssemblies ();

        filterMock.VerifyAllExpectations ();
        Assert.That (assemblies, Is.Empty);
      });
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyTrueIncludeAssemblyFalse ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        InitializeDynamicDirectory ();

        var filterMock = new MockRepository ().StrictMock<IAssemblyLoaderFilter> ();
        filterMock.Expect (mock => mock.ShouldConsiderAssembly (Arg<AssemblyName>.Is.NotNull)).Return (true).Repeat.AtLeastOnce ();
        filterMock.Expect (mock => mock.ShouldIncludeAssembly (Arg<Assembly>.Is.NotNull)).Return (false).Repeat.AtLeastOnce ();
        filterMock.Replay ();

        var loader = new FilteringAssemblyLoader (filterMock);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true);
        var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, loader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies ();

        filterMock.VerifyAllExpectations();
        Assert.That (assemblies, Is.Empty);
      });
    }

    private void ExecuteInSeparateAppDomain (CrossAppDomainDelegate test)
    {
      AppDomain appDomain = null;

      try
      {
        var appDomainSetup = new AppDomainSetup ();
        appDomainSetup.ApplicationName = "Test";
        appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        appDomainSetup.PrivateBinPath = _searchPathForDllsHelper.BuildOutputDirectory + ";" + _searchPathForExesHelper.BuildOutputDirectory;
        appDomainSetup.DynamicBase = _dynamicDirectoryHelper.BuildOutputDirectory;
        appDomainSetup.ShadowCopyFiles = AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles;

        appDomain = AppDomain.CreateDomain ("Test", null, appDomainSetup);

        appDomain.DoCallBack (test);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);
      }
    }

    private void InitializeDynamicDirectory ()
    {
      _dynamicDirectoryHelper.CopyAllGeneratedAssembliesToNewDirectory (AppDomain.CurrentDomain.DynamicDirectory);
    }

    private Assembly CompileTestAssemblyInMemory (string assemblyName, params string[] referencedAssemblies)
    {
      AssemblyCompiler assemblyCompiler = AssemblyCompiler.CreateInMemoryAssemblyCompiler (
          c_testAssemblySourceDirectoryRoot + "\\" + assemblyName,
          ArrayUtility.Combine (new[] { typeof (MarkerAttribute).Module.Name }, 
          referencedAssemblies));
      assemblyCompiler.Compile ();
      return assemblyCompiler.CompiledAssembly;
    }

    private AssemblyCompilerTestHelper CreateAssemblyCompilerTestHelper (string buildOutputDirectory)
    {
      var createBuildOutputDirectory = buildOutputDirectory != AppDomain.CurrentDomain.BaseDirectory;
      return new AssemblyCompilerTestHelper (
          buildOutputDirectory, createBuildOutputDirectory, c_testAssemblySourceDirectoryRoot, typeof (MarkerAttribute).Module.Name);
    }

    private FilteringAssemblyLoader CreateLoaderForMarkedAssemblies ()
    {
      var markerAttributeType = typeof (MarkerAttribute);
      var attributeFilter = new AttributeAssemblyLoaderFilter (markerAttributeType);
      return new FilteringAssemblyLoader (attributeFilter);
    }

    private Assembly Load (string assemblyPath)
    {
      var assemblyName = Path.GetFileNameWithoutExtension (assemblyPath);
      return Assembly.Load (assemblyName);
    }

    private Assembly LoadFile (string assemblyPath)
    {
      return Assembly.LoadFile (assemblyPath);
    }
  }
}
