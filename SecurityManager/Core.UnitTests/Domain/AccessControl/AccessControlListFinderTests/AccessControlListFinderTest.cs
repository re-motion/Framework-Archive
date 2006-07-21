using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlListFinderTests
{
  [TestFixture]
  public class Find_BySecurableClassDefinition : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void Succeed_WithoutStateProperties ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);
      SecurityContext context = CreateStatelessContext ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Succeed_WithStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      AccessControlList acl = _testHelper.GetAclForDeliveredAndUnpaidStates (classDefinition);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (classDefinition, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Succeed_WithStatesAndStateless ()
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
    public void Fail_WithSecurityContextDoesNotContainAllStates ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      _testHelper.CreateAclsForOrderAndPaymentStates (classDefinition);
      SecurityContext context = CreateContextWithoutPaymentState ();

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (classDefinition, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException),
        "The state 'None' is not defined for the property 'State' of the securable class 'Rubicon.SecurityManager.UnitTests.TestDomain.Order, Rubicon.SecurityManager.UnitTests' or its base classes.")]
    public void Fail_WithSecurityContextContainsStateWithInvalidValue ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      _testHelper.CreateAclsForOrderAndPaymentStates (classDefinition);
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", PaymentState.None);
      SecurityContext context = new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (classDefinition, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException),
        "The ACL for the securable class 'Rubicon.SecurityManager.UnitTests.TestDomain.Order, Rubicon.SecurityManager.UnitTests' could not be found.")]
    public void Fail_WithSecurityContextContainsInvalidState ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinition ();
      _testHelper.CreateAclsForOrderAndPaymentStates (classDefinition);
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);
      states.Add ("New", PaymentState.None);
      SecurityContext context = new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList acl = aclFinder.Find (classDefinition, context);

      Assert.IsNull (acl);
    }

    [Test]
    public void Succeed_WithDerivedClassDefinitionAndPropertiesInBaseClass ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition specialOrderClass = _testHelper.CreateSpecialOrderClassDefinition (orderClass);
      AccessControlList acl = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (specialOrderClass, context);

      Assert.AreSame (acl, foundAcl);
    }

    [Test]
    public void Succeed_WithDerivedClassDefinitionAndSameProperties ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition specialOrderClass = _testHelper.CreateSpecialOrderClassDefinition (orderClass);
      AccessControlList aclForOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      AccessControlList aclForSpecialOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (specialOrderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (specialOrderClass, context);

      Assert.AreSame (aclForSpecialOrder, foundAcl);
    }

    [Test]
    public void Succeed_WithDerivedClassDefinitionAndAdditionalProperty ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition premiumOrderClass = _testHelper.CreatePremiumOrderClassDefinition (orderClass);
      AccessControlList aclForOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      AccessControlList aclForPremiumOrder = _testHelper.GetAclForDeliveredAndUnpaidAndDhlStates (premiumOrderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidAndDhlOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (premiumOrderClass, context);

      Assert.AreSame (aclForPremiumOrder, foundAcl);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The state 'Delivery' is missing in the security context.")]
    public void Fail_WithDerivedClassDefinitionAndMissingStatePropertyInSecurityContext ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition premiumOrderClass = _testHelper.CreatePremiumOrderClassDefinition (orderClass);
      AccessControlList aclForOrder = _testHelper.GetAclForDeliveredAndUnpaidStates (orderClass);
      AccessControlList aclForPremiumOrder = _testHelper.GetAclForDeliveredAndUnpaidAndDhlStates (premiumOrderClass);
      SecurityContext context = CreateContextForDeliveredAndUnpaidOrder (typeof (SpecialOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (premiumOrderClass, context);
    }

    private SecurityContext CreateStatelessContext ()
    {
      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidOrder ()
    {
      return CreateContextForDeliveredAndUnpaidOrder (typeof (Order));
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidOrder (Type type)
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);

      return new SecurityContext (type, "owner", "ownerGroup", "ownerClient", states, new Enum[0]);
    }

    private SecurityContext CreateContextForDeliveredAndUnpaidAndDhlOrder (Type type)
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);
      states.Add ("Payment", PaymentState.None);
      states.Add ("Delivery", Delivery.Dhl);

      return new SecurityContext (type, "owner", "ownerGroup", "ownerClient", states, new Enum[0]);
    }

    private SecurityContext CreateContextWithoutPaymentState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("State", OrderState.Delivered);

      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", states, new Enum[0]);
    }
  }
}
