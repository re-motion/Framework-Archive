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

    private IClassReflector _typeReflector;
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
      _typeReflector = new ClassReflector ();
      _reflector = new AssemblyReflector (_typeReflector);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_typeReflector, _reflector.ClassReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      _reflector.GetMetadata (typeof (File).Assembly, _cache);

      SecurableClassInfo paperFileTypeInfo = _cache.GetTypeInfo (typeof (PaperFile));
      Assert.IsNotNull (paperFileTypeInfo);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.PaperFile", paperFileTypeInfo.Name);
      
      SecurableClassInfo fileTypeInfo = _cache.GetTypeInfo (typeof (File));
      Assert.IsNotNull (fileTypeInfo);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", fileTypeInfo.Name);

      EnumValueInfo generalAccessTypeCreateEnumValueInfo = _cache.GetAccessType (GeneralAccessType.Create);
      Assert.IsNotNull (generalAccessTypeCreateEnumValueInfo);
      Assert.AreEqual ("Create", generalAccessTypeCreateEnumValueInfo.Name);
    }
  }
}