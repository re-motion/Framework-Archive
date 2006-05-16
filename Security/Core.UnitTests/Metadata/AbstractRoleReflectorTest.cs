using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class AbstractRoleReflectorTest
  {
    // types

    // static members

    // member fields

    private Mockery _mocks;
    private IEnumerationReflector _enumeratedTypeReflectorMock;
    private AbstractRoleReflector _reflector;
    private MetadataCache _cache;
    private EnumValueInfo _valueDomainAbstractRoleClerk;
    private EnumValueInfo _valueDomainAbstractRoleSecretary;
    private EnumValueInfo _valueSpeicalAbstractRoleAdministrator;

    // construction and disposing

    public AbstractRoleReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _enumeratedTypeReflectorMock = _mocks.NewMock<IEnumerationReflector> ();
      _reflector = new AbstractRoleReflector (_enumeratedTypeReflectorMock);
      _cache = new MetadataCache ();

      _valueDomainAbstractRoleClerk = new EnumValueInfo (0, "Clerk");
      _valueDomainAbstractRoleSecretary = new EnumValueInfo (1, "Secretary");
      _valueSpeicalAbstractRoleAdministrator = new EnumValueInfo (0, "Administrator");
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_enumeratedTypeReflectorMock, _reflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetAbstractRoles ()
    {
      Dictionary<Enum, EnumValueInfo> domainAbstractRoles = new Dictionary<Enum, EnumValueInfo> ();
      domainAbstractRoles.Add (DomainAbstractRole.Clerk, _valueDomainAbstractRoleClerk);
      domainAbstractRoles.Add (DomainAbstractRole.Secretary, _valueDomainAbstractRoleSecretary);

      Dictionary<Enum, EnumValueInfo> specialAbstractRoles = new Dictionary<Enum, EnumValueInfo> ();
      specialAbstractRoles.Add (SpecialAbstractRole.Administrator, _valueSpeicalAbstractRoleAdministrator);

      Expect.Once.On (_enumeratedTypeReflectorMock)
          .Method ("GetValues")
          .With (typeof (DomainAbstractRole), _cache)
          .Will (Return.Value (domainAbstractRoles));

      Expect.Once.On (_enumeratedTypeReflectorMock)
          .Method ("GetValues")
          .With (typeof (SpecialAbstractRole), _cache)
          .Will (Return.Value (specialAbstractRoles));

      List<EnumValueInfo> values = _reflector.GetAbstractRoles (typeof (File).Assembly, _cache);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsNotNull (values);
      Assert.AreEqual (3, values.Count);
      Assert.Contains (_valueDomainAbstractRoleClerk, values);
      Assert.Contains (_valueDomainAbstractRoleSecretary, values);
      Assert.Contains (_valueSpeicalAbstractRoleAdministrator, values);
    }

    [Test]
    public void GetAbstractRolesFromCache ()
    {
      AbstractRoleReflector reflector = new AbstractRoleReflector ();
      List<EnumValueInfo> expectedAbstractRoles = reflector.GetAbstractRoles (typeof (File).Assembly, _cache);
      List<EnumValueInfo> actualAbstractRoles = _cache.GetAbstractRoles();
     
      Assert.AreEqual (3, expectedAbstractRoles.Count);
      foreach (EnumValueInfo expected in expectedAbstractRoles)
        Assert.Contains (expected, actualAbstractRoles);
    }
  }
}