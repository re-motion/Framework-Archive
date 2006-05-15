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
    public void CacheTypeInfos ()
    {
      Type fileType = typeof (File);
      Type paperFileType = typeof (PaperFile);

      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      Assert.IsFalse (_cache.ContainsTypeInfo (fileType));
      Assert.IsNull (_cache.GetTypeInfo (fileType));

      _cache.AddTypeInfo (fileType, fileTypeInfo);
      Assert.AreEqual (fileTypeInfo, _cache.GetTypeInfo (fileType));
      Assert.IsFalse (_cache.ContainsTypeInfo (paperFileType));
      Assert.IsNull (_cache.GetTypeInfo (paperFileType));

      _cache.AddTypeInfo (paperFileType, paperFileTypeInfo);
      Assert.AreEqual (fileTypeInfo, _cache.GetTypeInfo (fileType));
      Assert.AreEqual (paperFileTypeInfo, _cache.GetTypeInfo (paperFileType));
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
      Assert.AreEqual (confidentialityPropertyInfo, _cache.GetStatePropertyInfo (fileConfidentialityProperty));
      Assert.AreEqual (_cache.GetStatePropertyInfo (fileConfidentialityProperty), _cache.GetStatePropertyInfo (paperFileConfidentialityProperty));
      Assert.IsFalse (_cache.ContainsStatePropertyInfo (paperFileStateProperty));
      Assert.IsNull (_cache.GetStatePropertyInfo (paperFileStateProperty));

      _cache.AddStatePropertyInfo (paperFileStateProperty, statePropertyInfo);
      Assert.AreEqual (confidentialityPropertyInfo, _cache.GetStatePropertyInfo (fileConfidentialityProperty));
      Assert.AreEqual (statePropertyInfo, _cache.GetStatePropertyInfo (paperFileStateProperty));
    }

    [Test]
    public void CacheEnumValueInfos ()
    {
      EnumValueInfo fileStateNewEnumValueInfo = new EnumValueInfo (0, "New");
      EnumValueInfo fileStateNormalEnumValueInfo = new EnumValueInfo (1, "Normal");

      Assert.IsFalse (_cache.ContainsEnumValueInfo (FileState.New));
      Assert.IsNull (_cache.GetEnumValueInfo (FileState.New));

      _cache.AddEnumValueInfo (FileState.New, fileStateNewEnumValueInfo);
      Assert.AreEqual (fileStateNewEnumValueInfo, _cache.GetEnumValueInfo (FileState.New));
      Assert.IsFalse (_cache.ContainsEnumValueInfo (FileState.Normal));
      Assert.IsNull (_cache.GetEnumValueInfo (FileState.Normal));

      _cache.AddEnumValueInfo (FileState.Normal, fileStateNormalEnumValueInfo);
      Assert.AreEqual (fileStateNewEnumValueInfo, _cache.GetEnumValueInfo (FileState.New));
      Assert.AreEqual (fileStateNormalEnumValueInfo, _cache.GetEnumValueInfo (FileState.Normal));
    }

    [Test]
    public void CacheAccessTypes ()
    {
      EnumValueInfo domainAccessTypeJournalize = new EnumValueInfo (0, "Journalize");
      EnumValueInfo domainAccessTypeArchive = new EnumValueInfo (1, "Archive");

      Assert.IsFalse (_cache.ContainsAccessType (DomainAccessType.Journalize));
      Assert.IsNull (_cache.GetAccessType (DomainAccessType.Journalize));

      _cache.AddAccessType (DomainAccessType.Journalize, domainAccessTypeJournalize);
      Assert.AreEqual (domainAccessTypeJournalize, _cache.GetAccessType (DomainAccessType.Journalize));
      Assert.IsFalse (_cache.ContainsAccessType (DomainAccessType.Archive));
      Assert.IsNull (_cache.GetAccessType (DomainAccessType.Archive));

      _cache.AddAccessType (DomainAccessType.Archive, domainAccessTypeArchive);
      Assert.AreEqual (domainAccessTypeJournalize, _cache.GetAccessType (DomainAccessType.Journalize));
      Assert.AreEqual (domainAccessTypeArchive, _cache.GetAccessType (DomainAccessType.Archive));
    }

    [Test]
    public void GetCachedTypeInfos ()
    {
      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      _cache.AddTypeInfo (typeof (File), fileTypeInfo);
      _cache.AddTypeInfo (typeof (PaperFile), paperFileTypeInfo);

      List<SecurableClassInfo> infos = _cache.GetTypeInfos ();

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
      EnumValueInfo domainAccessTypeJournalize = new EnumValueInfo (0, "Journalize");
      EnumValueInfo domainAccessTypeArchive = new EnumValueInfo (1, "Archive");

      _cache.AddAccessType (DomainAccessType.Journalize, domainAccessTypeJournalize);
      _cache.AddAccessType (DomainAccessType.Archive, domainAccessTypeArchive);

      List<EnumValueInfo> infos = _cache.GetAccessTypes ();

      Assert.IsNotNull (infos);
      Assert.AreEqual (2, infos.Count);
      Assert.Contains (domainAccessTypeJournalize, infos);
      Assert.Contains (domainAccessTypeArchive, infos);
    }
  }
}