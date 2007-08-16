using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using File=System.IO.File;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception
{
  [TestFixture]
  public class InterceptedDomainObjectFactoryTest : ClientTransactionBaseTest
  {
    private InterceptedDomainObjectFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new InterceptedDomainObjectFactory ();
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsAssignableType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.IsTrue (typeof (Order).IsAssignableFrom (concreteType));
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsDifferentType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreNotEqual (typeof (Order), concreteType);
    }

    [Test]
    public void SaveReturnsPathOfGeneratedAssemblySigned ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      string[] paths = _factory.SaveGeneratedAssemblies ();
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "Rubicon.Data.DomainObjects.Generated.Signed.dll"), paths[0]);
      Assert.IsTrue (File.Exists (paths[0]));
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSave ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      _factory.SaveGeneratedAssemblies ();
      _factory.GetConcreteDomainObjectType (typeof (OrderItem));
      _factory.SaveGeneratedAssemblies ();
      _factory.GetConcreteDomainObjectType (typeof (ClassWithAllDataTypes));
    }

  }
}