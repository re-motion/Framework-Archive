using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class AccessTypeCacheClientTransactionExtensionTest
  {
    [Test]
    public void Get_Cache ()
    {
      AccessTypeCacheClientTransactionExtension extension = new AccessTypeCacheClientTransactionExtension ();
      Assert.IsNotNull (extension.Cache);
    }

    [Test]
    public void TestInterfaceMembers ()
    {
      IClientTransactionExtension extension = new AccessTypeCacheClientTransactionExtension ();
      extension.NewObjectCreating (null);
      extension.ObjectsLoaded (null);
      extension.ObjectDeleting (null);
      extension.ObjectDeleted (null);
      extension.PropertyValueReading (null, null, ValueAccess.Current);
      extension.PropertyValueRead (null, null,null, ValueAccess.Current);
      extension.PropertyValueChanging (null, null, null, null);
      extension.PropertyValueChanged (null, null, null, null);
      extension.RelationReading (null, null, ValueAccess.Current);
      extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
      extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
      extension.RelationChanging (null, null, null, null);
      extension.RelationChanged (null, null);
      extension.FilterQueryResult (null, null);
      extension.Committing (null);
      extension.Committed (null);
      extension.RollingBack (null);
      extension.RolledBack (null);
    }
  }
}