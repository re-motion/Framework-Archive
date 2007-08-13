using System;
using System.Collections.Specialized;
using System.Threading;
using System.Web;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.ExecutionEngine;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Web
{
  [TestFixture]
  [CLSCompliant (false)]
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
    public void AutoEnlistingCreateRoot ()
    {
      SetDatabaseModifyable ();

      // TODO: WxeTransaction.OnExecutionStarted wirft eine Exception => ungültiger Zustand im RestorePreviousTransaction (ExecutionStarted == false)
      // TODO: Die Fehlersituation führt auch dazu, dass im ClientTransactionScope.Leave ein Fehler auftritt, weil
      // ClientTransactionScope.ActiveScope nicht gleich dem ist, dessen Leave-Methode ausgeführt wird => checken

      using (new ClientTransactionScope ())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject ();
        inParameter.DateTimeProperty = DateTime.Now;
        inParameter.DateProperty = DateTime.Now.Date;
        inParameter.Int32Property = 4;
        ClientTransactionScope.CurrentTransaction.Commit ();

        inParameter.Int32Property = 7;
        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateRoot,
            inParameter);
        function.Execute (Context);
        ClassWithAllDataTypes outParameter = function.OutParameter;

        Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreNotEqual (12, outParameter.Int32Property);
        Assert.AreEqual (9, outParameter.Int32Property);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
        + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType =  MessageMatch.Regex)]
    public void AutoEnlistingCreateRootThrowsWhenInvalidInParameter ()
    {
      using (new ClientTransactionScope ())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject ();
        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateRoot,
            inParameter);
        try
        {
          function.Execute (Context);
        }
        catch (WxeUnhandledException ex)
        {
          try
          {
            throw ex.InnerException;
          }
          catch (ArgumentException aex)
          {
            Assert.AreEqual ("InParameter", aex.ParamName);
            throw;
          }
        }
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
        + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateRootThrowsWhenInvalidOutParameter ()
    {
      using (new ClientTransactionScope ())
      {
        DomainObjectParameterInvalidOutTestTransactedFunction function = new DomainObjectParameterInvalidOutTestTransactedFunction (WxeTransactionMode.CreateRoot);
        try
        {
          function.Execute (Context);
        }
        catch (WxeUnhandledException ex)
        {
          try
          {
            throw ex.InnerException;
          }
          catch (ArgumentException aex)
          {
            Assert.AreEqual ("OutParameter", aex.ParamName);
            throw;
          }
        }
      }
    }

    [Test]
    public void AutoEnlistingCreateChild()
    {
      SetDatabaseModifyable ();
      DomainObjectParameterWithChildTestTransactedFunction function = new DomainObjectParameterWithChildTestTransactedFunction ();
      function.Execute (Context);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
        + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateChildWithInvalidInParameter()
    {
      DomainObjectParameterWithChildInvalidInTestTransactedFunction function = new DomainObjectParameterWithChildInvalidInTestTransactedFunction ();
      try
      {
        function.Execute (Context);
       }
      catch (WxeUnhandledException ex)
      {
        try
        {
          throw ex.InnerException;
        }
        catch (ArgumentException aex)
        {
          Assert.AreEqual ("InParameter", aex.ParamName);
          throw;
        }
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
        + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateChildWithInvalidOutParameter ()
    {
      DomainObjectParameterWithChildInvalidOutTestTransactedFunction function = new DomainObjectParameterWithChildInvalidOutTestTransactedFunction ();
      try
      {
        function.Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        try
        {
          throw ex.InnerException;
        }
        catch (ArgumentException aex)
        {
          Assert.AreEqual ("OutParameter", aex.ParamName);
          throw;
        }
      }
    }

    [Test]
    public void Serialization ()
    {
      SerializationTestTransactedFunction function = new SerializationTestTransactedFunction ();
      function.Execute (Context);
      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsTrue (function.SecondStepExecuted);

      SerializationTestTransactedFunction deserializedFunction = (SerializationTestTransactedFunction) Serializer.Deserialize (function.SerializedSelf);
      Assert.IsTrue (deserializedFunction.FirstStepExecuted);
      Assert.IsFalse (deserializedFunction.SecondStepExecuted);

      deserializedFunction.Execute (Context);

      Assert.IsTrue (deserializedFunction.FirstStepExecuted);
      Assert.IsTrue (deserializedFunction.SecondStepExecuted);
    }

    [Test]
    public void ThreadAbortException ()
    {
      ThreadAbortTestTransactedFunction function = new ThreadAbortTestTransactedFunction ();
      try
      {
        function.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsFalse (function.SecondStepExecuted);
      Assert.IsTrue (function.ThreadAborted);

      function.Execute (Context);

      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsTrue (function.SecondStepExecuted);
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunction ()
    {
      ThreadAbortTestTransactedFunction nestedFunction = new ThreadAbortTestTransactedFunction ();
      ClientTransactionScope originalScope = new ClientTransactionScope ();
      CreateRootWithChildTestTransactedFunction parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsFalse (nestedFunction.SecondStepExecuted);
      Assert.IsTrue (nestedFunction.ThreadAborted);

      parentFunction.Execute (Context);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsTrue (nestedFunction.SecondStepExecuted);

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      originalScope.Leave ();
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunctionWithThreadMigration ()
    {
      ThreadAbortTestTransactedFunction nestedFunction = new ThreadAbortTestTransactedFunction ();
      ClientTransactionScope originalScope = new ClientTransactionScope ();
      CreateRootWithChildTestTransactedFunction parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

      ThreadRunner.Run (delegate {
        Assert.IsTrue (nestedFunction.FirstStepExecuted);
        Assert.IsFalse (nestedFunction.SecondStepExecuted);
        Assert.IsTrue (nestedFunction.ThreadAborted);

        parentFunction.Execute (Context);

        Assert.IsTrue (nestedFunction.FirstStepExecuted);
        Assert.IsTrue (nestedFunction.SecondStepExecuted);

        Assert.AreNotSame (originalScope, ClientTransactionScope.ActiveScope); // new scope on new thread
        Assert.AreSame (originalScope.ScopedTransaction, ClientTransactionScope.CurrentTransaction); // but same transaction as on old thread
      });

      originalScope.Leave ();
    }
  }
}