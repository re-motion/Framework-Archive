using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.UnitTests.Web
{
  [TestFixture]
  public class WxeTransactedFunctionTest : WxeFunctionBaseTest
  {
    [Test]
    public void WxeTransactedFunctionCreateRoot ()
    {
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootTestTransactedFunction (originalScope).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateChildIfParent ()
    {
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootWithChildTestTransactedFunction (originalScope.ScopedTransaction, new CreateChildIfParentTestTransactedFunction ()).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionNone ()
    {
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope  = ClientTransactionScope.ActiveScope;
        new CreateNoneTestTransactedFunction (originalScope).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewAutoCommit ()
    {
      SetDatabaseModifyable ();
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.NewTransaction ());

        new AutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransaction.NewTransaction ()));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewNoAutoCommit()
    {
      SetDatabaseModifyable ();
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.NewTransaction ());

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (5, GetInt32Property (ClientTransaction.NewTransaction ()));
      }
    }

    [Test]
    public void WxeTransactedFunctionNoneAutoCommit ()
    {
      SetDatabaseModifyable ();
      SetInt32Property (5, ClientTransaction.NewTransaction ());
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;

        new AutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransactionScope.CurrentTransaction));
      }

      Assert.AreEqual (5, GetInt32Property (ClientTransaction.NewTransaction ()));
    }

    [Test]
    public void WxeTransactedFunctionNoneNoAutoCommit ()
    {
      SetDatabaseModifyable ();
      SetInt32Property (5, ClientTransaction.NewTransaction ());
      using (new ClientTransactionScope ())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransactionScope.CurrentTransaction));
      }

      Assert.AreEqual (5, GetInt32Property (ClientTransaction.NewTransaction ()));
    }

    [Test]
    [ExpectedException (typeof (InconsistentClientTransactionScopeException),
        ExpectedMessage = "Somebody else has removed ClientTransactionScope.ActiveScope.")]
    public void RemoveCurrentScopeFromWithinFunctionThrows ()
    {
      ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
      try
      {
        new RemoveCurrentTransactionScopeFunction ().Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        throw ex.InnerException;
      }
      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
    }

    [Test]
    [ExpectedException (typeof (InconsistentClientTransactionScopeException),
        ExpectedMessage = "ClientTransactionScope.ActiveScope does not contain the expected transaction scope.")]
    public void RemoveCurrentScopeFromWithinFunctionThrowsWithPreviouslyExistingScope ()
    {
      try
      {
        new ClientTransactionScope ();
        new RemoveCurrentTransactionScopeFunction ().Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        throw ex.InnerException;
      }
    }

    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        objectWithAllDataTypes.Int32Property = value;

        clientTransaction.Commit ();
      }
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        return objectWithAllDataTypes.Int32Property;
      }
    }

    [Test]
    public void AutoEnlistingCreateNone ()
    {
      using (new ClientTransactionScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject();
        inParameter.Int32Property = 7;
        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.None, inParameter);
        function.Execute (Context);
        ClassWithAllDataTypes outParameter = function.OutParameter;
        Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (12, outParameter.Int32Property);
      }
    }

    [Test]
    [Ignore ("TODO: FS - Implement automatic parameter enlisting")]
    public void AutoEnlistingCreateRoot ()
    {
      using (new ClientTransactionScope ())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject ();
        inParameter.Int32Property = 7;
        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateRoot,
            inParameter);
        function.Execute (Context);
        ClassWithAllDataTypes outParameter = function.OutParameter;
        Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreNotEqual (12, outParameter.Int32Property);
        Assert.AreEqual (7, outParameter.Int32Property);
      }
    }

    [Test]
    public void AutoEnlistingCreateChild()
    {
      SetDatabaseModifyable ();
      DomainObjectParameterWithChildTestTransactedFunction function = new DomainObjectParameterWithChildTestTransactedFunction ();
      function.Execute (Context);
    }
  }
}