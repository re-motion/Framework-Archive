using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Security;
using Rubicon.SecurityManager.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class FindAbstractRolesQueryBuilderTest : DomainTest
  {
    [Test]
    public void CreateQuery_OneRole ()
    {
      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      Query query = queryBuilder.CreateQuery (new EnumWrapper[] { new EnumWrapper (ProjectRoles.QualityManager) });

      Assert.AreEqual (1, query.Parameters.Count);
      Assert.IsTrue (query.Parameters.Contains ("@p0"));
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests", query.Parameters["@p0"].Value);
      Assert.AreEqual ("SELECT * FROM [AbstractRoleDefinitionView] WHERE [Name] = @p0", query.Statement);
    }
  }
}
