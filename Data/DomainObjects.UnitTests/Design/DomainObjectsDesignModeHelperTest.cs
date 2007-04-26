using System;
using System.ComponentModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Design;
using Rubicon.Development.UnitTesting.IO;

namespace Rubicon.Data.DomainObjects.UnitTests.Design
{
  [TestFixture]
  public class DomainObjectsDesignModeHelperTest
  {
    [Test]
    public void InitializeConfiguration()
    {
      using (TempFile configFile = new TempFile ())
      {
        System.Configuration.Configuration configuration = ConfigurationFactory.LoadConfigurationFromFile (
            configFile, ResourceManager.GetDomainObjectsConfigurationWithFakeMappingLoader());

        MockRepository mockRepository = new MockRepository();
        IDesignModeHelper mockDesignModeHelper = mockRepository.CreateMock<IDesignModeHelper>();
        ISite stubSite = mockRepository.CreateMock<ISite>();
        Expect.Call (mockDesignModeHelper.GetConfiguration ()).Return (configuration);
        SetupResult.For (mockDesignModeHelper.Site).Return (stubSite);

        mockRepository.ReplayAll();

        ConfigurationWrapper oldConfigurationWrapper = ConfigurationWrapper.Current;
        IDomainObjectsConfiguration oldDomainObjectsConfiguration = DomainObjectsConfiguration.Current;
        MappingConfiguration oldMappingConfiguration = MappingConfiguration.Current;

        Assert.That (oldConfigurationWrapper, Is.Not.Null);
        Assert.That (oldDomainObjectsConfiguration, Is.Not.Null);
        Assert.That (oldMappingConfiguration, Is.Not.Null);

        DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (mockDesignModeHelper);
        domainObjectsDesignModeHelper.InitializeConfiguration();

        mockRepository.VerifyAll();

        Assert.That (oldConfigurationWrapper, Is.Not.SameAs (ConfigurationWrapper.Current));
        Assert.That (oldDomainObjectsConfiguration, Is.Not.SameAs (DomainObjectsConfiguration.Current));
        Assert.That (oldMappingConfiguration, Is.Not.SameAs (MappingConfiguration.Current));
        Assert.That (MappingConfiguration.Current.ClassDefinitions.Contains ("Fake"), Is.True);
      }
    }

    [Test]
    public void InitializeConfiguration_WithNoConfiguration ()
    {
        MockRepository mockRepository = new MockRepository ();
        IDesignModeHelper mockDesignModeHelper = mockRepository.CreateMock<IDesignModeHelper> ();
        Expect.Call (mockDesignModeHelper.GetConfiguration ()).Return (null);

        mockRepository.ReplayAll ();

        ConfigurationWrapper oldConfigurationWrapper = ConfigurationWrapper.Current;
        IDomainObjectsConfiguration oldDomainObjectsConfiguration = DomainObjectsConfiguration.Current;
        MappingConfiguration oldMappingConfiguration = MappingConfiguration.Current;

        Assert.That (oldConfigurationWrapper, Is.Not.Null);
        Assert.That (oldDomainObjectsConfiguration, Is.Not.Null);
        Assert.That (oldMappingConfiguration, Is.Not.Null);

        DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (mockDesignModeHelper);
        domainObjectsDesignModeHelper.InitializeConfiguration ();

        mockRepository.VerifyAll ();

        Assert.That (oldConfigurationWrapper, Is.SameAs (ConfigurationWrapper.Current));
        Assert.That (oldDomainObjectsConfiguration, Is.SameAs (DomainObjectsConfiguration.Current));
        Assert.That (oldMappingConfiguration, Is.SameAs (MappingConfiguration.Current));
    }
  }
}