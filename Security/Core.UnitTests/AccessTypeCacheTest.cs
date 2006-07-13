using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class AccessTypeCacheTest
  {
    private IAccessTypeCache<string> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new AccessTypeCache<string> ();
    }

    [Test]
    public void Get_WithResultNotInCache ()
    {
      Assert.IsNull (_cache.Get ("key1"));
    }

    [Test]
    public void Add_Get ()
    {
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (GeneralAccessType.Create)};

      _cache.Add ("key1", accessTypes);
      Assert.AreSame (accessTypes, _cache.Get ("key1"));
    }

    [Test]
    public void Add_Get_Clear_Get ()
    {
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (GeneralAccessType.Create) };

      _cache.Add ("key1", accessTypes);
      Assert.AreSame (accessTypes, _cache.Get ("key1"));
      _cache.Clear ();
      Assert.IsNull (_cache.Get ("key1"));
    }

  }
}