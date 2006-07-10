using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlListTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void FindMatchingEntries_WithMatchingAce ()
    {
      AccessControlEntry entry = new AccessControlEntry (_testHelper.Transaction);
      AccessControlList acl = _testHelper.CreateAcl (entry);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      AccessControlEntry[] foundEntries = acl.FindMatchingEntries (token);

      Assert.AreEqual (1, foundEntries.Length);
      Assert.Contains (entry, foundEntries);
    }

    [Test]
    public void FindMatchingEntries_WithoutMatchingAce ()
    {
      AccessControlList acl = _testHelper.CreateAcl (_testHelper.CreateAceWithAbstractRole ());
      SecurityToken token = _testHelper.CreateEmptyToken ();

      AccessControlEntry[] foundEntries = acl.FindMatchingEntries (token);

      Assert.AreEqual (0, foundEntries.Length);
    }

    [Test]
    public void FindMatchingEntries_WithMultipleMatchingAces ()
    {
      AccessControlEntry ace1 = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace1, null);

      AbstractRoleDefinition role2 = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = new AccessControlEntry (_testHelper.Transaction);
      ace2.SpecificAbstractRole = role2;
      _testHelper.AttachAccessType (ace2, readAccessType, null);
      _testHelper.AttachAccessType (ace2, writeAccessType, true);
      _testHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (role2);

      AccessControlEntry[] entries = acl.FindMatchingEntries (token);

      Assert.AreEqual (2, entries.Length);
      Assert.Contains (ace2, entries);
      Assert.Contains (ace1, entries);
    }

    [Test]
    public void GetAccessTypes_WithMatchingAce ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace, null);
      AccessControlList acl = _testHelper.CreateAcl (ace);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
    }

    [Test]
    public void GetAccessTypes_WithoutMatchingAce ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithAbstractRole ();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace, null);
      AccessControlList acl = _testHelper.CreateAcl (ace);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAccessTypes_WithMultipleMatchingAces ()
    {
      AbstractRoleDefinition role1 = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "QualityManager", 0);
      AccessControlEntry ace1 = new AccessControlEntry (_testHelper.Transaction);
      ace1.SpecificAbstractRole = role1;
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace1, null);

      AbstractRoleDefinition role2 = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = new AccessControlEntry (_testHelper.Transaction);
      ace2.SpecificAbstractRole = role2;
      _testHelper.AttachAccessType (ace2, readAccessType, true);
      _testHelper.AttachAccessType (ace2, writeAccessType, true);
      _testHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (role1, role2);

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (2, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
      Assert.Contains (writeAccessType, accessTypes);
    }

    [Test]
    public void GetAccessTypes_WithMultipleMatchingAcesButDifferentPriorities ()
    {
      AbstractRoleDefinition role1 = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "QualityManager", 0);
      AccessControlEntry ace1 = new AccessControlEntry (_testHelper.Transaction);
      ace1.SpecificAbstractRole = role1;
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace1, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace1, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace1, null);

      AbstractRoleDefinition role2 = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "SoftwareDeveloper", 1);
      AccessControlEntry ace2 = new AccessControlEntry (_testHelper.Transaction);
      ace2.SpecificAbstractRole = role2;
      ace2.Priority = 42;
      _testHelper.AttachAccessType (ace2, readAccessType, null);
      _testHelper.AttachAccessType (ace2, writeAccessType, true);
      _testHelper.AttachAccessType (ace2, deleteAccessType, null);

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (role1, role2);

      AccessTypeDefinition[] accessTypes = acl.GetAccessTypes (token);

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (writeAccessType, accessTypes);
    }

    [Test]
    public void FilterAcesByPriority_Empty ()
    {
      AccessControlList acl = _testHelper.CreateAcl ();
      AccessControlEntry[] aces = new AccessControlEntry[0];

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (0, entries.Length);
    }

    [Test]
    public void FilterAcesByPriority_OnlyOne ()
    {
      AccessControlEntry ace1 = new AccessControlEntry (_testHelper.Transaction);

      AccessControlList acl = _testHelper.CreateAcl (ace1);
      AccessControlEntry[] aces = new AccessControlEntry[] { ace1 };

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (1, entries.Length);
      Assert.Contains (ace1, entries);
    }

    [Test]
    public void FilterAcesByPriority_Multiple ()
    {
      AccessControlEntry ace1 = new AccessControlEntry (_testHelper.Transaction);
      ace1.Priority = 24;
      AccessControlEntry ace2 = new AccessControlEntry (_testHelper.Transaction);
      ace2.Priority = 42;

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2);
      AccessControlEntry[] aces = new AccessControlEntry[] { ace1, ace2 };

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (1, entries.Length);
      Assert.Contains (ace2, entries);
    }

    [Test]
    public void FilterAcesByPriority_MultipleAndMultipleFiltered ()
    {
      AccessControlEntry ace1 = new AccessControlEntry (_testHelper.Transaction);
      ace1.Priority = 24;
      AccessControlEntry ace2 = new AccessControlEntry (_testHelper.Transaction);
      ace2.Priority = 42;
      AccessControlEntry ace3 = new AccessControlEntry (_testHelper.Transaction);
      ace3.Priority = 42;

      AccessControlList acl = _testHelper.CreateAcl (ace1, ace2, ace3);
      AccessControlEntry[] aces = new AccessControlEntry[] { ace1, ace2, ace3 };

      AccessControlEntry[] entries = acl.FilterAcesByPriority (aces);

      Assert.AreEqual (2, entries.Length);
      Assert.Contains (ace2, entries);
      Assert.Contains (ace3, entries);
    }

    [Test]
    public void CreateStateCombination ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinitionWithProperties ();
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);

      StateCombination stateCombination = acl.CreateStateCombination ();

      Assert.AreSame (acl, stateCombination.AccessControlList);
      Assert.AreEqual (acl.Class, stateCombination.Class);
      Assert.IsEmpty (stateCombination.StateUsages);
    }

    [Test]
    public void CreateAccessControlEntry ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateOrderClassDefinitionWithProperties ();
      AccessTypeDefinition readAccessType = _testHelper.AttachAccessType (classDefinition, Guid.NewGuid(), "Read", 0);
      AccessTypeDefinition deleteAccessType = _testHelper.AttachAccessType (classDefinition, Guid.NewGuid (), "Delete", 1);
      AccessControlList acl = _testHelper.CreateAcl (classDefinition);

      AccessControlEntry entry = acl.CreateAccessControlEntry ();

      Assert.AreSame (acl, entry.AccessControlList);
      Assert.AreEqual (2, entry.Permissions.Count);
      Assert.AreSame (readAccessType, ((Permission) entry.Permissions[0]).AccessType);
      Assert.AreSame (entry, ((Permission) entry.Permissions[0]).AccessControlEntry);
      Assert.AreSame (deleteAccessType, ((Permission) entry.Permissions[1]).AccessType);
      Assert.AreSame (entry, ((Permission) entry.Permissions[1]).AccessControlEntry);
    }
  }
}
