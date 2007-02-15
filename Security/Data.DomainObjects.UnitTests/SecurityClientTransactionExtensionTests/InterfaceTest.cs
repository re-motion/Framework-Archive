using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class InterfaceTest : BaseTest
  {

    [Test]
    public void TestInterfaceMembers ()
    {
      IClientTransactionExtension extension = new SecurityClientTransactionExtension ();
      extension.ObjectsLoaded (null);
      extension.ObjectDeleted (null);
      extension.PropertyValueRead (null, null, null, ValueAccess.Current);
      extension.PropertyValueChanged (null, null, null, null);
      extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
      extension.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
      extension.RelationChanged (null, null);
      extension.Committing (null);
      extension.Committed (null);
      extension.RollingBack (null);
      extension.RolledBack (null);
    }
  }
}