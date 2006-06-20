using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
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
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);
      SecurityContext context = CreateStatelessContext ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Find_SecurableClassDefinitionWithStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      AccessControlList acl = _testHelper.GetAclForDeliveredAndUnpaidStates (classDefinition);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Find_SecurableClassDefinitionWithStatesAndStateless ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      AccessControlList acl = _testHelper.GetAclForStateless (classDefinition);
      SecurityContext context = CreateStatelessContext ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The state 'Payment' is missing in the security context.")]
    public void Find_SecurityContextDoesNotContainAllStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      _testHelper.CreateAcls (classDefinition);
      SecurityContext context = CreateContextWithoutPaymentState ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (classDefinition, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The state 'None' is not defined for the property 'State' of securable class 'Rubicon.SecurityManager.UnitTests.TestDomain.Order'.")]
    public void Find_SecurityContextContainsStateWithInvalidValue ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      _testHelper.CreateAcls (classDefinition);
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", PaymentState.None);
      SecurityContext context = new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (classDefinition, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The security context contains at least one state not defined by securable class 'Rubicon.SecurityManager.UnitTests.TestDomain.Order'.")]
    public void Find_SecurityContextContainsInvalidState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      _testHelper.CreateAcls (classDefinition);
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);
      states.Add ("New", PaymentState.None);
      SecurityContext context = new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (classDefinition, context);
    }

    private SecurityContext CreateStatelessContext ()
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

    private SecurityContext CreateContextWithoutPaymentState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);

      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);
    }
  }
}
