using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests
{
  public class DatabaseFixtures
  {
    public void CreateTwoAbstractRoleDefinitions ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();
      AbstractRoleDefinition qualityManagerRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "QualityManager|Rubicon.SecurityManager.Domain.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.Domain.UnitTests", 0);
      AbstractRoleDefinition developerRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "Developer|Rubicon.SecurityManager.Domain.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.Domain.UnitTests", 1);
      transaction.Commit ();
    }
  }
}
