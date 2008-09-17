using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryPropertiesSearchServiceTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private DatabaseFixtures _dbFixtures;

    [SetUp]
    public void SetUp ()
    {
      try
      {
        BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
        
        _dbFixtures = new DatabaseFixtures ();
        _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
        throw;
      }
    }

    [TearDown]
    public void TearDown ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }
  }
}