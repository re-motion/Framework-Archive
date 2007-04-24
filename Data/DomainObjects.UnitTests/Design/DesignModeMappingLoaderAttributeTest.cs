using System;
using System.ComponentModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Design;

namespace Rubicon.Data.DomainObjects.UnitTests.Design
{
  [TestFixture]
  public class DesignModeMappingLoaderAttributeTest
  {
    [Test]
    public void Initialize()
    {
      DesignModeMappingLoaderAttribute attribute = new DesignModeMappingLoaderAttribute (typeof (FakeDesignModeMappingLoader));

      Assert.That (attribute.Type, Is.SameAs (typeof (FakeDesignModeMappingLoader)));
    }

    [Test]
    public void CreateInstance ()
    {
      MockRepository mockRepository = new MockRepository ();
      ISite stubSite = mockRepository.CreateMock<ISite> ();
      DesignModeMappingLoaderAttribute attribute = new DesignModeMappingLoaderAttribute (typeof (FakeDesignModeMappingLoader));

      IMappingLoader mappingLoader = attribute.CreateInstance (stubSite);
      Assert.That (mappingLoader, Is.InstanceOfType (typeof (FakeDesignModeMappingLoader)));
      Assert.That (((FakeDesignModeMappingLoader) mappingLoader).Site, Is.SameAs (stubSite));
    }
  }
}