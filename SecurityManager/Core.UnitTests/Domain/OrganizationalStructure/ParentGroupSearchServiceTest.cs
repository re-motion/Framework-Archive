using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class ParentGroupSearchServiceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _parentGroupProperty;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.NewTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new ParentGroupSearchService();
      IBusinessObjectClass groupClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (Group));
      _parentGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Parent");
      Assert.That (_parentGroupProperty, Is.Not.Null);
    }

    [Test]
    public void SupportsIdentity ()
    {
      Assert.That (_searchService.SupportsIdentity (_parentGroupProperty), Is.True);
    }

    [Test]
    public void Search ()
    {
      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      List<Group> expectedParentGroups = group.GetPossibleParentGroups (group.Tenant.ID);
      Assert.That (expectedParentGroups, Is.Not.Empty);

      IBusinessObject[] actualParentGroups = _searchService.Search (group, _parentGroupProperty, null);

      Assert.That (actualParentGroups, Is.EquivalentTo (expectedParentGroups));
    }
  }
}