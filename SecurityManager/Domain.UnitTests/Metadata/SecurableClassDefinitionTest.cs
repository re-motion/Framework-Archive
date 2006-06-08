using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
{
  [TestFixture]
  public class SecurableClassDefinitionTest : DomainTest
  {
    private ClientTransaction _transaction;
    private SecurableClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
      _classDefinition = new SecurableClassDefinition (_transaction);
    }

    [Test]
    public void AddAccessType ()
    {
      AccessTypeDefinition accessType = new AccessTypeDefinition (_transaction);

      _classDefinition.AddAccessType (accessType);

      Assert.AreEqual (1, _classDefinition.AccessTypeReferences.Count);
      AccessTypeReference reference = (AccessTypeReference) _classDefinition.AccessTypeReferences[0];
      Assert.AreSame (accessType, reference.AccessType);
    }

    [Test]
    public void AddStateProperty ()
    {
      StatePropertyDefinition stateProperty = new StatePropertyDefinition (_transaction);

      _classDefinition.AddStateProperty (stateProperty);

      Assert.AreEqual (1, _classDefinition.StatePropertyReferences.Count);
      StatePropertyReference reference = (StatePropertyReference) _classDefinition.StatePropertyReferences[0];
      Assert.AreSame (stateProperty, reference.StateProperty);
    }
  }
}
