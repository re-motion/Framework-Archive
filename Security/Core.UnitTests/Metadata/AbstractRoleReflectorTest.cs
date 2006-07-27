using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rhino.Mocks;
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

    private MockRepository _mocks;
    private IEnumerationReflector _enumeratedTypeReflectorMock;
    private AbstractRoleReflector _abstractRoleReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AbstractRoleReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _enumeratedTypeReflectorMock = _mocks.CreateMock<IEnumerationReflector> ();
      _abstractRoleReflector = new AbstractRoleReflector (_enumeratedTypeReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IAbstractRoleReflector), _abstractRoleReflector);
      Assert.AreSame (_enumeratedTypeReflectorMock, _abstractRoleReflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetAbstractRoles ()
    {
      Dictionary<Enum, EnumValueInfo> domainAbstractRoles = new Dictionary<Enum, EnumValueInfo> ();
      domainAbstractRoles.Add (DomainAbstractRole.Clerk, AbstractRoles.Clerk);
      domainAbstractRoles.Add (DomainAbstractRole.Secretary, AbstractRoles.Secretary);

      Dictionary<Enum, EnumValueInfo> specialAbstractRoles = new Dictionary<Enum, EnumValueInfo> ();
      specialAbstractRoles.Add (SpecialAbstractRole.Administrator, AbstractRoles.Administrator);

      Expect.Call (_enumeratedTypeReflectorMock.GetValues (typeof (DomainAbstractRole), _cache)).Return (domainAbstractRoles);
      Expect.Call (_enumeratedTypeReflectorMock.GetValues (typeof (SpecialAbstractRole), _cache)).Return (specialAbstractRoles);
      _mocks.ReplayAll ();

      List<EnumValueInfo> values = _abstractRoleReflector.GetAbstractRoles (typeof (File).Assembly, _cache);

      _mocks.VerifyAll ();

      Assert.IsNotNull (values);
      Assert.AreEqual (3, values.Count);
      Assert.Contains (AbstractRoles.Clerk, values);
      Assert.Contains (AbstractRoles.Secretary, values);
      Assert.Contains (AbstractRoles.Administrator, values);
    }

    [Test]
    public void GetAbstractRolesFromCache ()
    {
      AbstractRoleReflector reflector = new AbstractRoleReflector ();
      List<EnumValueInfo> expectedAbstractRoles = reflector.GetAbstractRoles (typeof (File).Assembly, _cache);
      List<EnumValueInfo> actualAbstractRoles = _cache.GetAbstractRoles ();

      Assert.AreEqual (3, expectedAbstractRoles.Count);
      foreach (EnumValueInfo expected in expectedAbstractRoles)
        Assert.Contains (expected, actualAbstractRoles);
    }
  }
}