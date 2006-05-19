using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class MetadataCacheTest
  {
    // types

    // static members

    // member fields

    private MetadataCache _cache;

    // construction and disposing

    public MetadataCacheTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _cache = new MetadataCache ();
    }

    [Test]
    public void CacheSecurableClassInfos ()
    {
      Type fileType = typeof (File);
      Type paperFileType = typeof (PaperFile);

      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      Assert.IsFalse (_cache.ContainsSecurableClassInfo (fileType));
      Assert.IsNull (_cache.GetSecurableClassInfo (fileType));

      _cache.AddSecurableClassInfo (fileType, fileTypeInfo);
      Assert.AreSame (fileTypeInfo, _cache.GetSecurableClassInfo (fileType));
      Assert.IsFalse (_cache.ContainsSecurableClassInfo (paperFileType));
      Assert.IsNull (_cache.GetSecurableClassInfo (paperFileType));

      _cache.AddSecurableClassInfo (paperFileType, paperFileTypeInfo);
      Assert.AreSame (fileTypeInfo, _cache.GetSecurableClassInfo (fileType));
      Assert.AreSame (paperFileTypeInfo, _cache.GetSecurableClassInfo (paperFileType));
    }

    [Test]
    public void CacheStatePropertyInfos ()
    {
      PropertyInfo fileConfidentialityProperty = typeof (File).GetProperty ("Confidentiality");
      Assert.IsNotNull (fileConfidentialityProperty);

      PropertyInfo paperFileConfidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.IsNotNull (paperFileConfidentialityProperty);

      PropertyInfo paperFileStateProperty = typeof (PaperFile).GetProperty ("State");
      Assert.IsNotNull (paperFileStateProperty);

      StatePropertyInfo confidentialityPropertyInfo = new StatePropertyInfo ();
      StatePropertyInfo statePropertyInfo = new StatePropertyInfo ();

      Assert.IsFalse (_cache.ContainsStatePropertyInfo (fileConfidentialityProperty));
      Assert.IsNull (_cache.GetStatePropertyInfo (fileConfidentialityProperty));

      _cache.AddStatePropertyInfo (fileConfidentialityProperty, confidentialityPropertyInfo);
      Assert.AreSame (confidentialityPropertyInfo, _cache.GetStatePropertyInfo (fileConfidentialityProperty));
      Assert.AreSame (_cache.GetStatePropertyInfo (fileConfidentialityProperty), _cache.GetStatePropertyInfo (paperFileConfidentialityProperty));
      Assert.IsFalse (_cache.ContainsStatePropertyInfo (paperFileStateProperty));
      Assert.IsNull (_cache.GetStatePropertyInfo (paperFileStateProperty));

      _cache.AddStatePropertyInfo (paperFileStateProperty, statePropertyInfo);
      Assert.AreSame (confidentialityPropertyInfo, _cache.GetStatePropertyInfo (fileConfidentialityProperty));
      Assert.AreSame (statePropertyInfo, _cache.GetStatePropertyInfo (paperFileStateProperty));
    }

    [Test]
    public void CacheEnumValueInfos ()
    {
      EnumValueInfo fileStateNewEnumValueInfo = new EnumValueInfo (0, "New");
      EnumValueInfo fileStateNormalEnumValueInfo = new EnumValueInfo (1, "Normal");

      Assert.IsFalse (_cache.ContainsEnumValueInfo (FileState.New));
      Assert.IsNull (_cache.GetEnumValueInfo (FileState.New));

      _cache.AddEnumValueInfo (FileState.New, fileStateNewEnumValueInfo);
      Assert.AreSame (fileStateNewEnumValueInfo, _cache.GetEnumValueInfo (FileState.New));
      Assert.IsFalse (_cache.ContainsEnumValueInfo (FileState.Normal));
      Assert.IsNull (_cache.GetEnumValueInfo (FileState.Normal));

      _cache.AddEnumValueInfo (FileState.Normal, fileStateNormalEnumValueInfo);
      Assert.AreSame (fileStateNewEnumValueInfo, _cache.GetEnumValueInfo (FileState.New));
      Assert.AreSame (fileStateNormalEnumValueInfo, _cache.GetEnumValueInfo (FileState.Normal));
    }

    [Test]
    public void CacheAccessTypes ()
    {
      Assert.IsFalse (_cache.ContainsAccessType (DomainAccessType.Journalize));
      Assert.IsNull (_cache.GetAccessType (DomainAccessType.Journalize));

      _cache.AddAccessType (DomainAccessType.Journalize, AccessTypes.Journalize);
      Assert.AreSame (AccessTypes.Journalize, _cache.GetAccessType (DomainAccessType.Journalize));
      Assert.IsFalse (_cache.ContainsAccessType (DomainAccessType.Archive));
      Assert.IsNull (_cache.GetAccessType (DomainAccessType.Archive));

      _cache.AddAccessType (DomainAccessType.Archive, AccessTypes.Archive);
      Assert.AreSame (AccessTypes.Journalize, _cache.GetAccessType (DomainAccessType.Journalize));
      Assert.AreSame (AccessTypes.Archive, _cache.GetAccessType (DomainAccessType.Archive));
    }

    [Test]
    public void CacheAbstractRoles ()
    {
      Assert.IsFalse (_cache.ContainsAbstractRole (DomainAbstractRole.Clerk));
      Assert.IsNull (_cache.GetAbstractRole (DomainAbstractRole.Secretary));

      _cache.AddAbstractRole (DomainAbstractRole.Clerk, AbstractRoles.Clerk);
      Assert.AreSame (AbstractRoles.Clerk, _cache.GetAbstractRole (DomainAbstractRole.Clerk));
      Assert.IsFalse (_cache.ContainsAbstractRole (DomainAbstractRole.Secretary));
      Assert.IsNull (_cache.GetAbstractRole (DomainAbstractRole.Secretary));

      _cache.AddAbstractRole (DomainAbstractRole.Secretary, AbstractRoles.Secretary);
      Assert.AreSame (AbstractRoles.Clerk, _cache.GetAbstractRole (DomainAbstractRole.Clerk));
      Assert.AreSame (AbstractRoles.Secretary, _cache.GetAbstractRole (DomainAbstractRole.Secretary));
    }

    [Test]
    public void GetCachedSecurableClassInfos ()
    {
      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      _cache.AddSecurableClassInfo (typeof (File), fileTypeInfo);
      _cache.AddSecurableClassInfo (typeof (PaperFile), paperFileTypeInfo);

      List<SecurableClassInfo> infos = _cache.GetSecurableClassInfos ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (fileTypeInfo, infos);
      Assert.Contains (paperFileTypeInfo, infos);
    }

    [Test]
    public void GetCachedPropertyInfos ()
    {
      PropertyInfo confidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.IsNotNull (confidentialityProperty);

      PropertyInfo stateProperty = typeof (PaperFile).GetProperty ("State");
      Assert.IsNotNull (stateProperty);

      StatePropertyInfo confidentialityPropertyInfo = new StatePropertyInfo ();
      StatePropertyInfo statePropertyInfo = new StatePropertyInfo ();

      _cache.AddStatePropertyInfo (confidentialityProperty, confidentialityPropertyInfo);
      _cache.AddStatePropertyInfo (stateProperty, statePropertyInfo);

      List<StatePropertyInfo> infos = _cache.GetStatePropertyInfos ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (confidentialityPropertyInfo, infos);
      Assert.Contains (statePropertyInfo, infos);
    }

    [Test]
    public void GetCachedAccessTypes ()
    {
      _cache.AddAccessType (DomainAccessType.Journalize, AccessTypes.Journalize);
      _cache.AddAccessType (DomainAccessType.Archive, AccessTypes.Archive);

      List<EnumValueInfo> infos = _cache.GetAccessTypes ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (AccessTypes.Journalize, infos);
      Assert.Contains (AccessTypes.Archive, infos);
    }

    [Test]
    public void GetCachedAbstractRoles ()
    {
      EnumValueInfo valueDomainAbstractRoleClerk = new EnumValueInfo (0, "Clerk");
      EnumValueInfo valueDomainAbstractRoleSecretary = new EnumValueInfo (1, "Secretary");

      _cache.AddAbstractRole (DomainAbstractRole.Clerk, valueDomainAbstractRoleClerk);
      _cache.AddAbstractRole (DomainAbstractRole.Secretary, valueDomainAbstractRoleSecretary);

      List<EnumValueInfo> infos = _cache.GetAbstractRoles ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (valueDomainAbstractRoleClerk, infos);
      Assert.Contains (valueDomainAbstractRoleSecretary, infos);
    }
  }
}