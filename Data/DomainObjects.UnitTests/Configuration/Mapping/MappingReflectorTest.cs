using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingReflectorTest
  {
    [Test]
    public void GetResolveTypes()
    {
      MappingReflector mappingReflector = new MappingReflector();
      Assert.IsTrue (mappingReflector.ResolveTypes);
    }

    [Test]
    public void name()
    {
      Assembly testDomain = TestDomainFactory.ConfigurationMappingTestDomain;
      Assert.AreEqual (2, testDomain.GetTypes().Length);
    }
  }
}