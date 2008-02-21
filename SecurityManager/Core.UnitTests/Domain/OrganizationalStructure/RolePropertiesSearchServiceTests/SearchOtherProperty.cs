using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchOtherProperty : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _tenantProperty;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new RolePropertiesSearchService();
      IBusinessObjectClass userClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (User));
      _tenantProperty = (IBusinessObjectReferenceProperty) userClass.GetPropertyDefinition ("Tenant");
      Assert.That (_tenantProperty, Is.Not.Null);
    }

    [Test]
    public void SupportsIdentity_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsIdentity (_tenantProperty), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = 
        "The property 'Tenant' is not supported by the 'Rubicon.SecurityManager.Domain.OrganizationalStructure.RolePropertiesSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {
      Role role = _testHelper.CreateRole (null, null, null);

      _searchService.Search (role, _tenantProperty, null);
    }
  }
}