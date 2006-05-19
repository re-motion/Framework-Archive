using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NUnit.Framework;
using NMock2;

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

    private Mockery _mocks;
    private IClassReflector _classReflectorMock;
    private IAbstractRoleReflector _abstractRoleReflectorMock;
    private IAccessTypeReflector _accessTypeReflectorMock;
    private AssemblyReflector _assemblyReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AssemblyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _accessTypeReflectorMock = _mocks.NewMock<IAccessTypeReflector> ();
      _classReflectorMock = _mocks.NewMock < IClassReflector> ();
      _abstractRoleReflectorMock = _mocks.NewMock <IAbstractRoleReflector> ();
      _assemblyReflector = new AssemblyReflector (_accessTypeReflectorMock, _classReflectorMock, _abstractRoleReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_classReflectorMock, _assemblyReflector.ClassReflector);
      Assert.AreSame (_accessTypeReflectorMock, _assemblyReflector.AccessTypeReflector);
      Assert.AreSame (_abstractRoleReflectorMock, _assemblyReflector.AbstractRoleReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      Assembly securityAssembly = typeof (IAccessTypeReflector).Assembly;
      Assembly assembly = typeof (File).Assembly;

      Expect.Once.On (_accessTypeReflectorMock)
          .Method ("GetAccessTypesFromAssembly")
          .With (securityAssembly, _cache)
          .Will (Return.Value (new List<EnumValueInfo> (new EnumValueInfo[] {AccessTypes.Read, AccessTypes.Write})));

      Expect.Once.On (_accessTypeReflectorMock)
          .Method ("GetAccessTypesFromAssembly")
          .With (assembly, _cache)
          .Will (Return.Value (new List<EnumValueInfo> (new EnumValueInfo[] { AccessTypes.Journalize, AccessTypes.Archive })));

      Expect.Once.On (_abstractRoleReflectorMock)
          .Method ("GetAbstractRoles")
          .With (securityAssembly, _cache)
          .Will (Return.Value (new List<EnumValueInfo> ()));

      Expect.Once.On (_abstractRoleReflectorMock)
          .Method ("GetAbstractRoles")
          .With (assembly, _cache)
          .Will (Return.Value (new List<EnumValueInfo> (
              new EnumValueInfo[] { AbstractRoles.Clerk, AbstractRoles.Secretary, AbstractRoles.Administrator })));

      Expect.Once.On (_classReflectorMock)
          .Method ("GetMetadata")
          .With (typeof (File), _cache)
          .Will (Return.Value (new SecurableClassInfo()));

      Expect.Once.On (_classReflectorMock)
          .Method ("GetMetadata")
          .With (typeof (PaperFile), _cache)
          .Will (Return.Value (new SecurableClassInfo ()));

      _assemblyReflector.GetMetadata (assembly, _cache);

      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}