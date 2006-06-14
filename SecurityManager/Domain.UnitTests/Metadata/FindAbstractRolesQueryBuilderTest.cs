using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
{
  [TestFixture]
  public class FindAbstractRolesQueryBuilderTest
  {
    [Test]
    public void CreateQuery_OneRole ()
    {
      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      Query query = queryBuilder.CreateQuery (new EnumWrapper[] { new EnumWrapper (ProjectRole.QualityManager) });

      Assert.AreEqual (1, query.Parameters.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.Domain.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.Domain.UnitTests", query.Parameters[0].Value);
    }
  }
}
