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

    private IClassReflector _classReflector;
    private IAbstractRoleReflector _abstractRoleReflector;
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
      _classReflector = new ClassReflector ();
      _abstractRoleReflector = new AbstractRoleReflector ();
      _reflector = new AssemblyReflector (_classReflector, _abstractRoleReflector);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_classReflector, _reflector.ClassReflector);
      Assert.AreSame (_abstractRoleReflector, _reflector.AbstractRoleReflector);
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

      EnumValueInfo domainAccessTypeJournalizeEnumValueInfo = _cache.GetAccessType (DomainAccessType.Journalize);
      Assert.IsNotNull (domainAccessTypeJournalizeEnumValueInfo);
      Assert.AreEqual ("Journalize", domainAccessTypeJournalizeEnumValueInfo.Name);

      EnumValueInfo domainAbstractRoleClerkEnumValueInfo = _cache.GetAbstractRole (DomainAbstractRole.Clerk);
      Assert.IsNotNull (domainAbstractRoleClerkEnumValueInfo);
      Assert.AreEqual ("Clerk", domainAbstractRoleClerkEnumValueInfo.Name);

      EnumValueInfo specialAbstractRoleAdministratorEnumValueInfo = _cache.GetAbstractRole (SpecialAbstractRole.Administrator);
      Assert.IsNotNull (specialAbstractRoleAdministratorEnumValueInfo);
      Assert.AreEqual ("Administrator", specialAbstractRoleAdministratorEnumValueInfo.Name);
    }
  }
}