using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.Domain.UnitTests.AccessControl
{
  [TestFixture]
  public class AccessControlListFinderTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void Find_SecurableClassDefinitionWithoutStateProperties ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateSecurableClassDefinition ();
      AccessControlList acl = _testHelper.CreateAccessControlList (classDefinition);
      SecurityContext context = CreateContextForGeneralOrder ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Find_SecurableClassDefinitionWithStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateSecurableClassDefinition ();
      AccessControlList acl = GetAclForDeliveredAndUnpaidOrder (classDefinition);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    private SecurityContext CreateContextForGeneralOrder ()
    {
      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidOrder ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);

      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);
    }

    private AccessControlList GetAclForDeliveredAndUnpaidOrder (SecurableClassDefinition classDefinition)
    {
      List<AccessControlList> acls = _testHelper.CreateAccessControlLists (classDefinition);
      return acls[2];
    }
  }
}
