using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Design
{
  [TestFixture]
  public class DesignModeMappingReflectorTest
  {
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
  }
}