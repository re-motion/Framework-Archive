using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class StateDefinitionTest : DomainTest
  {
    [Test]
    public void Get_DisplayName ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      StateDefinition state = new StateDefinition (transaction);
      state.Name = "Value|Namespace.TypeName, Assembly";

      Assert.AreEqual ("Value|Namespace.TypeName, Assembly", state.DisplayName);
    }
  }
}
