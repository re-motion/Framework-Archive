using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.CodeGeneration;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using System.IO;
using Rubicon.Mixins;
using Rubicon.Reflection;
using Rubicon.Utilities;
using Order=Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order;
using Rubicon.Mixins.Context;

namespace Rubicon.Data.DomainObjects.UnitTests.Design
{
  [TestFixture]
  public class DesignModeMappingReflectorTest
  {
    [TearDown]
    public void TearDown ()
    {
      // reset mixin configuration to defaults
      MixinConfiguration.SetActiveContext (null);
      MixinConfiguration.ResetMasterConfiguration();
    }

    [Test]
    public void Initialize()
    {
      MockRepository mockRepository = new MockRepository();
      ISite mockSite = mockRepository.CreateMock<ISite>();
      ITypeDiscoveryService stubTypeDiscoveryService = mockRepository.CreateMock<ITypeDiscoveryService>();
      Expect.Call (mockSite.GetService (typeof (ITypeDiscoveryService))).Return (stubTypeDiscoveryService);

      mockRepository.ReplayAll();

      new DesignModeMappingReflector (mockSite);

      mockRepository.VerifyAll();
    }

    [Test]
    public void GetClassDefinitions ()
    {
      MockRepository mockRepository = new MockRepository ();
      ISite stubSite = mockRepository.CreateMock<ISite> ();
      ITypeDiscoveryService mockTypeDiscoveryService = mockRepository.CreateMock<ITypeDiscoveryService> ();
      SetupResult.For (stubSite.GetService (typeof (ITypeDiscoveryService))).Return (mockTypeDiscoveryService);
      Expect.Call (mockTypeDiscoveryService.GetTypes (typeof (DomainObject), false)).Return (new Type[] {typeof (Company)});
      mockRepository.ReplayAll ();

      DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (stubSite);
      ClassDefinitionCollection classDefinitionCollection = mappingReflector.GetClassDefinitions();

      mockRepository.VerifyAll ();

      Assert.That (classDefinitionCollection.Count, Is.EqualTo (1));
      Assert.That (classDefinitionCollection.Contains (typeof (Company)));
    }

    [Test]
    public void DesignModeMappingReflector_SetsEmptyMixinConfigurationIfNoneExists ()
    {
      ISite mockSite = GetMockSite();

      MixinConfiguration.SetActiveContext (null);
      DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (mockSite);
      mappingReflector.GetClassDefinitions ();

      Assert.IsTrue (MixinConfiguration.HasActiveContext);
      Assert.AreEqual (0, MixinConfiguration.ActiveContext.ClassContextCount);
    }

    [Test]
    public void DesignModeMappingReflector_KeepsExistingMixinConfiguration ()
    {
      ISite mockSite = GetMockSite ();

      ApplicationContext context = new ApplicationContext ();
      MixinConfiguration.SetActiveContext (context);

      DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (mockSite);
      mappingReflector.GetClassDefinitions ();

      Assert.IsTrue (MixinConfiguration.HasActiveContext);
      Assert.AreSame (context, MixinConfiguration.ActiveContext);
    }

    [Test]
    public void DesignModeMappingReflectorWorksFine_WithDelaySignAssemblyInAppBase ()
    {
      AppDomainRunner.Run (delegate
      {
        try
        {
          Compile (@"Design\DelaySignAssembly", @"Design.Dlls\Rubicon.Data.DomainObjects.UnitTests.Design.DelaySignAssembly.dll");
          Assert.Fail ("Expected FileLoadException");
        }
        catch (FileLoadException)
        {
          // expected
        }
        catch (AssemblyCompilationException)
        {
          // file gets locked on multiple executions
          Assert.IsTrue (System.IO.File.Exists (@"Design.Dlls\Rubicon.Data.DomainObjects.UnitTests.Design.DelaySignAssembly.dll"));
        }
      });

      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"Design.Dlls";
      setup.DynamicBase = Path.GetTempPath ();
      new AppDomainRunner (setup, delegate
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
        
        PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
        persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute (), DatabaseTest.c_testDomainProviderID));

        MappingLoaderConfiguration mappingLoaderConfiguration = new MappingLoaderConfiguration ();
        QueryConfiguration queryConfiguration = new QueryConfiguration ();
        DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (mappingLoaderConfiguration, persistenceConfiguration, queryConfiguration));

        Assert.IsFalse (MixinConfiguration.HasActiveContext);

        ISite mockSite = GetMockSite ();
        DesignModeMappingReflector mappingReflector = new DesignModeMappingReflector (mockSite);
        mappingReflector.GetClassDefinitions ();
      }, new object[0]).Run ();
    }

    private static void Compile (string sourceDirectory, string outputAssembly)
    {
      string outputAssemblyDirectory = Path.GetDirectoryName (Path.GetFullPath (outputAssembly));
      if (!Directory.Exists (outputAssemblyDirectory))
        Directory.CreateDirectory (outputAssemblyDirectory);

      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          new string[] { "Rubicon.Core.dll", "Rubicon.Data.DomainObjects.dll", "Rubicon.Mixins.dll" });

      compiler.Compile ();
    }

    private static ISite GetMockSite ()
    {
      MockRepository mockRepository = new MockRepository ();
      ISite mockSite = mockRepository.CreateMock<ISite> ();

      ITypeDiscoveryService stubTypeDiscoveryService = mockRepository.CreateMock<ITypeDiscoveryService> ();
      Expect.Call (mockSite.GetService (typeof (ITypeDiscoveryService))).Return (stubTypeDiscoveryService);
      Expect.Call (stubTypeDiscoveryService.GetTypes (typeof (DomainObject), false)).Return (new Type[] { typeof (Order) });

      mockRepository.ReplayAll ();
      return mockSite;
    }
  }
}