using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class AssemblyReflectorTest
  {
    // types

    // static members

    // member fields

    private ITypeReflector _typeReflector;
    private AssemblyReflector _reflector;
    private MetadataCache _cache;

    // construction and disposing

    public AssemblyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _typeReflector = new TypeReflector ();
      _reflector = new AssemblyReflector (_typeReflector);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_typeReflector, _reflector.TypeReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      _reflector.GetMetadata (typeof (File).Assembly, _cache);

      SecurableTypeInfo paperFileTypeInfo = _cache.GetTypeInfo (typeof (PaperFile));
      Assert.IsNotNull (paperFileTypeInfo);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.PaperFile", paperFileTypeInfo.Name);
      
      SecurableTypeInfo fileTypeInfo = _cache.GetTypeInfo (typeof (File));
      Assert.IsNotNull (fileTypeInfo);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", fileTypeInfo.Name);
    }
  }
}