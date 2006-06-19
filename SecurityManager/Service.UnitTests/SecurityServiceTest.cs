using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NMock2;
using Rubicon.Security;

using Rubicon.SecurityManager.Service.UnitTests.TestDomain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using System.Security.Principal;

namespace Rubicon.SecurityManager.Service.UnitTests
{
  [TestFixture]
  public class SecurityServiceTest : DomainTest
  {
    [Test]
    public void GetAccess_WithoutAccess ()
    {
      Mockery mocks = new Mockery ();
      IAccessControlListFinder aclFinder = mocks.NewMock<IAccessControlListFinder> ();
      ISecurityTokenBuilder tokenBuilder = mocks.NewMock<ISecurityTokenBuilder> ();

      ClientTransaction transaction = new ClientTransaction ();
      SecurityService service = new SecurityService (transaction, aclFinder, tokenBuilder);
      SecurityContext context = new SecurityContext (typeof (Order), "Owner", "OwnerGroup", "OwnerClient", null, null);
      SecurityToken token = new SecurityToken (new List<AbstractRoleDefinition> ());
      AccessControlEntry ace = CreateAce (transaction);

      Expect.Once.On (tokenBuilder).Method ("CreateToken").With (transaction, context).Will (Return.Value (token));
      Expect.Once.On (aclFinder).Method ("Find").With (transaction, context).Will (Return.Value (CreateAcl (transaction, ace)));

      AccessType[] accessTypes = service.GetAccess (context, CreateUser ());

      Assert.AreEqual (0, accessTypes.Length);

      mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void GetAccess_WithReadAccess ()
    {
      Mockery mocks = new Mockery ();
      IAccessControlListFinder aclFinder = mocks.NewMock<IAccessControlListFinder> ();
      ISecurityTokenBuilder tokenBuilder = mocks.NewMock<ISecurityTokenBuilder> ();

      ClientTransaction transaction = new ClientTransaction ();
      SecurityService service = new SecurityService (transaction, aclFinder, tokenBuilder);
      SecurityContext context = new SecurityContext (typeof (Order), "Owner", "OwnerGroup", "OwnerClient", null, null);
      AccessControlEntry ace = CreateAce (transaction);

      List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition> ();
      roles.Add (ace.SpecificAbstractRole);
      SecurityToken token = new SecurityToken (roles);

      Expect.Once.On (tokenBuilder).Method ("CreateToken").With (transaction, context).Will (Return.Value (token));
      Expect.Once.On (aclFinder).Method ("Find").With (transaction, context).Will (Return.Value (CreateAcl (transaction, ace)));

      AccessType[] accessTypes = service.GetAccess (context, CreateUser ());

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (EnumWrapper.Parse ("Read|MyTypeName")), accessTypes);

      mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    private AccessControlList CreateAcl (ClientTransaction transaction, AccessControlEntry ace)
    {
      AccessControlList acl = new AccessControlList (transaction);
      acl.AccessControlEntries.Add (ace);

      return acl;
    }

    private AccessControlEntry CreateAce (ClientTransaction transaction)
    {
      AccessControlEntry ace = new AccessControlEntry (transaction);

      AbstractRoleDefinition abstractRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "QualityManager", 0);
      ace.SpecificAbstractRole = abstractRole;

      AccessTypeDefinition readAccessType = new AccessTypeDefinition (transaction, Guid.NewGuid (), "Read|MyTypeName", 0);
      AccessTypeDefinition writeAccessType = new AccessTypeDefinition (transaction, Guid.NewGuid (), "Write|MyTypeName", 1);
      AccessTypeDefinition deleteAccessType = new AccessTypeDefinition (transaction, Guid.NewGuid (), "Delete|MyTypeName", 2);

      ace.AttachAccessType (readAccessType);
      ace.AttachAccessType (writeAccessType);
      ace.AttachAccessType (deleteAccessType);

      ace.AllowAccess (readAccessType);

      return ace;
    }

    private IPrincipal CreateUser ()
    {
      return new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
    }
  }
}
